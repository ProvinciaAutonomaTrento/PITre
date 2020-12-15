using System;
using System.Collections;
using System.Data;
using System.Globalization;
using OracleDataAccess = Oracle.DataAccess;

namespace Dpa.DataAccess.OracleClient
{
    /// <summary>
    /// <para>Represents a SQL statement or stored procedure to execute against an Oracle database.</para>
    /// </summary>   
    public class OracleCommandWrapper : DBCommandWrapper
    {
        private OracleDataAccess.Client.OracleCommand command;
        private int rowsAffected;
        private Hashtable guidParameters;
        private object[] parameterValues;
        private bool parameterDiscoveryRequired;
        private bool isRefCursorAdded;
        private char parameterToken;

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="OracleCommandWrapper"/> class with the text of a query and the command type.</para>
        /// </summary>        
        /// <param name="commandText"><para>The stored procedure name or SQL sting the command represents.</para></param>
        /// <param name="commandType"><para>One of the <see crer="CommandType"/> values.</para></param>
        /// <param name="parameterToken"><para>The parameter delimeter for database commands.</para></param>
        internal OracleCommandWrapper(string commandText, CommandType commandType, char parameterToken)
        {
            this.parameterToken = parameterToken;
            this.command = CreateCommand(commandText, commandType);
            this.guidParameters = new Hashtable();
        }

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="OracleCommandWrapper"/> class with the text of a query the command type, and the parameter values.</para>
        /// </summary>        
        /// <param name="commandText"><para>The stored procedure name or SQL sting the command represents.</para></param>
        /// <param name="commandType"><para>One of the <see crer="CommandType"/> values.</para></param>
        /// <param name="parameterToken"><para>The parameter delimeter for database commands.</para></param>
        /// <param name="parameterValues"><para>The parameter values to assign in positional order.</para></param>
        internal OracleCommandWrapper(string commandText, CommandType commandType, char parameterToken, object[] parameterValues)
            : this(commandText, commandType, parameterToken)
        {
            this.parameterValues = parameterValues;
            if (commandType == CommandType.StoredProcedure)
            {
                this.parameterDiscoveryRequired = true;
            }
        }

        /// <devdoc>
        /// Determines if parameter discovery is required.
        /// </devdoc>
        internal bool ParameterDiscoveryRequired
        {
            get { return parameterDiscoveryRequired; }
        }

        /// <summary>
        /// <para>Gets the underlying <see cref="IDbCommand"/>.</para>
        /// </summary>
        /// <value>
        /// <para>The underlying <see cref="IDbCommand"/>. The default is <see langword="null"/>.</para>
        /// </value>
        /// <seealso cref="OracleCommand"/>
        public override IDbCommand Command
        {
            get { return this.command; }
        }

        /// <summary>
        /// <para>Gets or sets the rows affected by this command.</para>
        /// </summary>
        /// <value>
        /// <para>The rows affected by this command.</para>
        /// </value>
        public override int RowsAffected
        {
            get { return this.rowsAffected; }
            set { this.rowsAffected = value; }
        }

        /// <summary>
        /// <para>Gets or sets the wait time before terminating the attempt to execute a command and generating an error.</para>
        /// </summary>
        /// <value>
        /// <para>The wait time before terminating the attempt to execute a command and generating an error.</para>
        /// </value>
        /// <remarks>
        /// <para>The inner <see cref="OracleCommand"/> does not implement a command timeout.</para>
        /// </remarks>
        public override int CommandTimeout
        {
            get { return -1; }
            set
            {
            }
        }

        internal bool IsRefCursorAdded
        {
            get { return isRefCursorAdded; }
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="OracleParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts null values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>        
        public override void AddParameter(string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            this.command.Parameters.Add(CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value));
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="OracleParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="oracleType"><para>One of the <see cref="OracleType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts null values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public void AddParameter(string name, OracleDataAccess.Client.OracleDbType oracleType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            OracleDataAccess.Client.OracleParameter param = CreateParameter(name, DbType.AnsiString, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            if (oracleType == OracleDataAccess.Client.OracleDbType.RefCursor)
            {   
                isRefCursorAdded = true;
            }
            param.OracleDbType = oracleType;
            this.command.Parameters.Add(param);
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="OracleParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public override void AddParameter(string name, DbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            this.command.Parameters.Add(CreateParameter(name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value));
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="OracleParameter"/> object to the command set as <see cref="ParameterDirection"/> value of Output.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        public override void AddOutParameter(string name, DbType dbType, int size)
        {
            this.command.Parameters.Add(CreateParameter(name, dbType, size, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value));
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="OracleParameter"/> object to the command set as <see cref="ParameterDirection"></see> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        public override void AddInParameter(string name, DbType dbType)
        {
            this.command.Parameters.Add(CreateParameter(name, dbType, 0, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, null));
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="OracleParameter"/> object to the command set as <see cref="ParameterDirection"/> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public override void AddInParameter(string name, DbType dbType, object value)
        {
            this.command.Parameters.Add(CreateParameter(name, dbType, 0, ParameterDirection.Input, false, 0, 0, String.Empty, DataRowVersion.Default, value));
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="OracleParameter"/> object to the command set as <see cref="ParameterDirection"/> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        public override void AddInParameter(string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            this.command.Parameters.Add(CreateParameter(name, dbType, 0, ParameterDirection.Input, false, 0, 0, sourceColumn, sourceVersion, null));
        }

        /// <summary>
        /// <para>Returns the value of the parameter for the given <paramref name="name"/>.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter to get the value.</para></param>
        /// <returns><para>The value of the parameter.</para></returns>
        public override object GetParameterValue(string name)
        {
            string parameterName = name;
            OracleDataAccess.Client.OracleParameter parameter = command.Parameters[parameterName];
            //check for DBNull
            if (parameter.Value is DBNull)
            {
                return DBNull.Value;
            }
            // cast the parameter as Guid if it is a guid parameter
            if (guidParameters.Contains(parameterName))
            {
                byte[] buffer = (byte[])parameter.Value;
                if (buffer.Length == 0)
                {
                    return DBNull.Value;
                }
                else
                {
                    return new Guid(buffer);
                }
            }
            // cast the parameter as Boolean if it is a boolean parameter
            else if (parameter.DbType == DbType.Boolean)
            {
                return Convert.ToBoolean(parameter.Value, CultureInfo.InvariantCulture);
            }
            return parameter.Value;
        }

        /// <summary>
        /// <para>Sets the value of a parameter for the given <paramref name="name"/>.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter to set the value.</para></param>
        /// <param name="value"><para>The new value of the parameter.</para></param>
        public override void SetParameterValue(string name, object value)
        {
            OracleDataAccess.Client.OracleParameter parameter = command.Parameters[name];
            if (value is Guid)
            {
                parameter.Value = ((Guid)value).ToByteArray();
            }
            else
            {
                string tmpVal = value as string;
                if ((tmpVal != null) && (tmpVal.Length == 0))
                {
                    parameter.Value = Convert.DBNull;
                }
                parameter.Value = (value == null) ? DBNull.Value : value;
            }
        }

        /// <summary>
        /// <para>Clean up resources.</para>
        /// </summary>
        public override void Dispose()
        {
            this.command.Dispose();
        }

        /// <summary>
        /// <para>Dicover the parameters for a stored procedure using a separate connection and command.</para>
        /// </summary>
        /// <param name="parameterToken"><para>The parameter delimeter for database commands.</para></param>
        protected override void DoDiscoverParameters(char parameterToken)
        {
            this.parameterToken = parameterToken;
            using (OracleDataAccess.Client.OracleCommand newCommand = CreateNewCommandAndConnectionForDiscovery())
            {
                OracleDataAccess.Client.OracleCommandBuilder.DeriveParameters(newCommand);

                foreach (IDataParameter parameter in newCommand.Parameters)
                {
                    IDataParameter cloneParameter = (IDataParameter)((ICloneable)parameter).Clone();
                    cloneParameter.ParameterName = cloneParameter.ParameterName;
                    this.command.Parameters.Add(cloneParameter);
                }

                newCommand.Connection.Close();
            }
        }

        /// <summary>
        /// <para>Assign the values provided by a user to the command parameters discovered in positional order.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The number of parameters does not match number of values for stored procedure.</para>
        /// </exception>
        protected override void DoAssignParameterValues()
        {
            if (SameNumberOfParametersAndValues() == false)
            {
                throw new InvalidOperationException("ExceptionMessageParameterMatchFailure");
            }

            int returnParameter = 0;
            for (int i = 0; i < this.parameterValues.Length; i++)
            {
                IDataParameter parameter = this.command.Parameters[i + returnParameter];

                // There used to be code here that checked to see if the parameter was input or input/output
                // before assigning the value to it. We took it out because of an operational bug with
                // deriving parameters for a stored procedure. It turns out that output parameters are set
                // to input/output after discovery, so any direction checking was unneeded. Should it ever
                // be needed, it should go here, and check that a parameter is input or input/output before
                // assigning a value to it.
                SetParameterValue(parameter.ParameterName, this.parameterValues[i]);
            }
        }

        /// <summary>
        /// <para>Determine if a stored procedure is using parameter discovery.</para>
        /// </summary>
        /// <returns>
        /// <para><see langword="true"/> if further preparation is needed.</para>
        /// </returns>
        protected override bool DoIsFurtherPreparationNeeded()
        {
            return this.parameterDiscoveryRequired;
        }

        private OracleDataAccess.Client.OracleParameter CreateParameter(string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            OracleDataAccess.Client.OracleParameter param = this.command.CreateParameter();
            param.ParameterName = name;
            param.DbType = dbType;
            param.Size = size;
            param.Value = (value == null) ? DBNull.Value : value;
            // modify parameter type and value for special cases
            switch (dbType)
            {
                // for Guid, change to value to byte array
                case DbType.Guid:
                    guidParameters.Add(param.ParameterName, "System.Guid");
                    param.OracleDbType = OracleDataAccess.Client.OracleDbType.Raw;
                    param.Size = 16;
                    // convert Guid value to byte array only if not null
                    if ((value is DBNull) || (value == null))
                    {
                        param.Value = Convert.DBNull;
                    }
                    else
                    {
                        param.Value = ((Guid)value).ToByteArray();
                    }
                    break;
                default:
                    break;
            }
            param.Direction = direction;
            param.IsNullable = nullable;
            param.Precision = precision;
            param.Scale = scale;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }

        private bool SameNumberOfParametersAndValues()
        {
            int returnParameterCount = 0;
            int numberOfParametersToStoredProcedure = this.command.Parameters.Count - returnParameterCount;
            int numberOfValuesProvidedForStoredProcedure = this.parameterValues.Length;
            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }

        /// <devdoc>
        /// Discovery has to be done on its own connection to allow for the case of the
        /// connection being used being enrolled in a transaction. The OracleCommandBuilder.DeriveParameters
        /// method creates a new OracleCommand internally to communicate to the database, and it
        /// reuses the same connection that is passed in on the command object. If this command
        /// object has a connection that is enrolled in a transaction, the DeriveParameters method does not
        /// honor that transaction, and the call fails. To avoid this, create your own connection and
        /// command, and use them. 
        /// 
        /// You then have to clone each of the IDataParameter objects before it can be transferred to 
        /// the original command, or another exception is thrown.
        /// </devdoc>
        private OracleDataAccess.Client.OracleCommand CreateNewCommandAndConnectionForDiscovery()
        {
            OracleDataAccess.Client.OracleConnection clonedConnection = (OracleDataAccess.Client.OracleConnection)((ICloneable)this.command.Connection).Clone();
            clonedConnection.Open();
            OracleDataAccess.Client.OracleCommand newCommand = CreateCommand(this.command.CommandText, this.command.CommandType);
            newCommand.Connection = clonedConnection;
            return newCommand;
        }

        private static OracleDataAccess.Client.OracleCommand CreateCommand(string commandText, CommandType commandType)
        {
            OracleDataAccess.Client.OracleCommand newCommand = new OracleDataAccess.Client.OracleCommand();
            newCommand.CommandText = commandText;
            newCommand.CommandType = commandType;
            return newCommand;
        }
    }
}