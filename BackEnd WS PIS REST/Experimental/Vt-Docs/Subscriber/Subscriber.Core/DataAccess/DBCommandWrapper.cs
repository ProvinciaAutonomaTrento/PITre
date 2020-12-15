using System;
using System.Data;

namespace Subscriber.DataAccess
{
    /// <summary>
    /// <para>Represents a wrapper for <see cref="IDbCommand"/> types. This class is abstract.</para>
    /// <seealso cref="IDbCommand"/>
    /// </summary>
    public abstract class DBCommandWrapper : IDisposable
    {
        /// <summary>
        /// <para>When overridden in a derived class, gets the underlying <see cref="IDbCommand"/>.</para>
        /// </summary>
        /// <value>
        /// <para>The underlying <see cref="IDbCommand"/>. The default is <see langword="null"/>.</para>
        /// </value>
        public abstract IDbCommand Command { get; }

        /// <summary>
        /// <para>When overridden in a derived class, gets or sets the rows affected by this command.</para>
        /// </summary>
        /// <value>
        /// <para>The rows affected by this command.</para>
        /// </value>
        public abstract int RowsAffected { get; set; }

        /// <summary>
        /// <para>When overridden in a derived class, gets or sets the wait time before terminating the attempt to execute a command and generating an error.</para>
        /// </summary>
        /// <value>
        /// <para>The wait time before terminating the attempt to execute a command and generating an error.</para>
        /// </value>
        public abstract int CommandTimeout { get; set; }

        /// <summary>
        /// <para>When overridden in a derived class, adds a new instance of an <see cref="IDataParameter"/> object to the command.</para>
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
        public abstract void AddParameter(string name, DbType dbType, int size, ParameterDirection direction,
            bool nullable, byte precision, byte scale, string sourceColumn,
            DataRowVersion sourceVersion, object value);

        /// <summary>
        /// <para>When overridden in a derived class, adds a new instance of an <see cref="IDataParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>
        public abstract void AddParameter(string name, DbType dbType, ParameterDirection direction,
            string sourceColumn, DataRowVersion sourceVersion, object value);

        /// <summary>
        /// <para>When overridden in a derived class, adds a new instance of an <see cref="IDataParameter"/> object to the command set as <see cref="ParameterDirection"/> value of Output.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        public abstract void AddOutParameter(string name, DbType dbType, int size);

        /// <summary>
        /// <para>When overridden in a derived class, adds a new instance of an <see cref="IDataParameter"/> object to the command set as <see cref="ParameterDirection"/> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        public abstract void AddInParameter(string name, DbType dbType);

        /// <summary>
        /// <para>When overridden in a derived class,  adds a new instance of an <see cref="IDataParameter"/> object to the command set as <see cref="ParameterDirection"/> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public abstract void AddInParameter(string name, DbType dbType, object value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void AddClobParameter(string name, object value);

        /// <summary>
        /// <para>When overridden in a derived class, adds a new instance of an <see cref="IDataParameter"/> object to the command set as <see cref="ParameterDirection"/> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        public abstract void AddInParameter(string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion);

        /// <summary>
        /// <para>When overridden in a derived class, returns the value of the parameter for the given <paramref name="name"/>.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter to get the value.</para></param>
        /// <returns><para>The value of the parameter.</para></returns>
        public abstract object GetParameterValue(string name);

        /// <summary>
        /// <para>When overridden in a derived class, sets the value of a parameter for the given <paramref name="name"/>.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter to set the value.</para></param>
        /// <param name="value"><para>The new value of the parameter.</para></param>
        public abstract void SetParameterValue(string name, object value);

        /// <summary>
        /// Populate the parameter collection for a stored procedure by querying the database
        /// or loading a cached parameter set.
        /// </summary>
        internal void DiscoverParameters(char parameterToken)
        {
            DoDiscoverParameters(parameterToken);
        }

        /// <summary>
        /// Determine if the parameters collection needs to be populated 
        /// using parameter discovery
        /// </summary>
        /// <returns>true if parameter discovery is needed</returns>
        internal bool IsFurtherPreparationNeeded()
        {
            return DoIsFurtherPreparationNeeded();
        }

        /// <summary>
        /// Assign values to parameters in positional orders
        /// </summary>
        internal void AssignParameterValues()
        {
            DoAssignParameterValues();
        }

        /// <summary>
        /// <para>When overridden in a derived class, discover the parameters for a stored procedure using a separate connection and command.</para>
        /// </summary>
        /// <param name="parameterToken"><para>The parameter delimeter for database commands.</para></param>
        protected abstract void DoDiscoverParameters(char parameterToken);

        /// <summary>
        /// <para>When overridden in a derived class, assign the values provided by a user to the command parameters discovered in positional order.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The number of parameters does not match number of values for stored procedure.</para>
        /// </exception>
        protected abstract void DoAssignParameterValues();

        /// <summary>
        /// <para>When overridden in a derived class, determine if a stored procedure is using parameter discovery.</para>
        /// </summary>
        /// <returns>
        /// <para><see langword="true"/> if further preparation is needed.</para>
        /// </returns>
        protected abstract bool DoIsFurtherPreparationNeeded();

        /// <summary>
        /// <para>When overridden in a derived class, clean up resources.</para>
        /// </summary>
        public abstract void Dispose();
    }
}