using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using Subscriber.DataAccess;

namespace Subscriber.DataAccess.Sql
{
    /// <summary>
    /// <para>Represents a Sql Server Database.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses Sql Server .NET Managed Provider from Microsoft (System.Data.SqlClient) to connect to the database.
    /// </para>  
    /// </remarks>
    public class SqlDatabase : Database
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="SqlDatabase"/> class.
        /// </summary>
        public SqlDatabase(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// <para>Gets the parameter token used to delimit parameters for the Sql Database.</para>
        /// </summary>
        /// <value>
        /// <para>The '@' symbol.</para>
        /// </value>
        protected override char ParameterToken
        {
            get { return '@'; }
        }

        /// <summary>
        /// <para>Get the connection for this database.</para>
        /// <seealso cref="IDbConnection"/>
        /// <seealso cref="SqlConnection"/>
        /// </summary>
        /// <returns>
        /// <para>The <see cref="SqlConnection"/> for this database.</para>
        /// </returns>
        public override IDbConnection GetConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }

        /// <summary>
        /// <para>Create a <see cref="SqlCommandWrapper"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="SqlCommandWrapper"/> for the stored procedure.</para></returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="storedProcedureName"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="storedProcedureName"/> hast not been initialized.</para>
        /// </exception>
        public override DBCommandWrapper GetStoredProcCommandWrapper(string storedProcedureName)
        {
            return new SqlCommandWrapper(storedProcedureName, CommandType.StoredProcedure, ParameterToken);
        }

        /// <summary>
        /// <para>Create an <see cref="SqlCommandWrapper"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="SqlCommandWrapper"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="storedProcedureName"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// <para>- or -</para>
        /// <para><paramref name="parameterValues"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="storedProcedureName"/> hast not been initialized.</para>
        /// </exception>
        public override DBCommandWrapper GetStoredProcCommandWrapper(string storedProcedureName, params object[] parameterValues)
        {
            return new SqlCommandWrapper(storedProcedureName, CommandType.StoredProcedure, ParameterToken, parameterValues);
        }

        /// <summary>
        /// <para>Create an <see cref="SqlCommandWrapper"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="SqlCommandWrapper"/> for the SQL query.</para></returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="query"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="query"/> hast not been initialized.</para>
        /// </exception>
        public override DBCommandWrapper GetSqlStringCommandWrapper(string query)
        {
            return new SqlCommandWrapper(query, CommandType.Text, ParameterToken);
        }

        /// <summary>
        /// <para>Create a <see cref="SqlDataAdapter"/> with the given update behavior and connection.</para>
        /// </summary>
        /// <param name="updateBehavior">
        /// <para>One of the <see cref="UpdateBehavior"/> values.</para>
        /// </param>
        /// <param name="connection">
        /// <para>The open connection to the database.</para>
        /// </param>
        /// <returns>An <see cref="SqlDataAdapter"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="connection"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        protected override DbDataAdapter GetDataAdapter(UpdateBehavior updateBehavior, IDbConnection connection)
        {
            string queryStringToBeFilledInLater = String.Empty;
            SqlDataAdapter adapter = new SqlDataAdapter(queryStringToBeFilledInLater, (SqlConnection)connection);

            if (updateBehavior == UpdateBehavior.Continue)
            {
                adapter.RowUpdated += new SqlRowUpdatedEventHandler(OnSqlRowUpdated);
            }
            return adapter;
        }

        /// <summary>
        /// <para>Executes the <see cref="SqlCommandWrapper"/> and returns an <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommandWrapper"/> to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(SqlCommandWrapper command)
        {
            IDbConnection connection = OpenConnection();
            PrepareCommand(command, connection);
            SqlCommand sqlCommand = command.Command as SqlCommand;
            return DoExecuteXmlReader(sqlCommand);
        }

        /// <summary>
        /// <para>Executes the <see cref="SqlCommandWrapper"/> in a transaction and returns an <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommandWrapper"/> to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(SqlCommandWrapper command, IDbTransaction transaction)
        {
            PrepareCommand(command, transaction);

            SqlCommand sqlCommand = command.Command as SqlCommand;
            return DoExecuteXmlReader(sqlCommand);
        }

        /// <devdoc>
        /// Execute the actual Xml Reader call.
        /// </devdoc>        
        private XmlReader DoExecuteXmlReader(SqlCommand sqlCommand)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                XmlReader reader = sqlCommand.ExecuteXmlReader();
                return reader;
            }
            catch
            {
                throw;
            }
        }

        /// <devdoc>
        /// Listens for the RowUpdate event on a dataadapter to support UpdateBehavior.Continue
        /// </devdoc>
        private void OnSqlRowUpdated(object sender, SqlRowUpdatedEventArgs rowThatCouldNotBeWritten)
        {
            if (rowThatCouldNotBeWritten.RecordsAffected == 0)
            {
                if (rowThatCouldNotBeWritten.Errors != null)
                {
                    rowThatCouldNotBeWritten.Row.RowError = "ExceptionMessageUpdateDataSetRowFailure";
                    rowThatCouldNotBeWritten.Status = UpdateStatus.SkipCurrentRow;
                }
            }
        }
    }
}