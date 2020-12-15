using System.Data;

namespace Dpa.DataAccess
{
    /// <summary>
    /// <para>
    /// Provides parameter caching services for dynamic parameter discovery of stored procedures.
    /// Eliminates the round-trip to the database to derive the parameters and types when a command
    /// is executed more than once.
    /// </para>
    /// </summary>
    public class ParameterCache
    {
        private ParameterCachingMechanism cache = new ParameterCachingMechanism();

        /// <summary>
        /// <para>
        /// Populates the parameter collection for a command wrapper from the cache 
        /// or performs a round-trip to the database to query the parameters
        /// </para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to add the parameters.</para>
        /// </param>
        /// <param name="parameterToken">
        /// <para>The token used to delimit parameters.</para>
        /// </param>
        public void FillParameters(DBCommandWrapper command, char parameterToken)
        {
            if (AlreadyCached(command))
            {
                AddParametersFromCache(command);
            }
            else
            {
                command.DiscoverParameters(parameterToken);
                IDataParameter[] copyOfParameters = CreateParameterCopy(command);

                this.cache.AddParameterSetToCache(command.Command, copyOfParameters);
            }

            command.AssignParameterValues();
        }

        /// <summary>
        /// <para>Empty the parameter cache</para>
        /// </summary>
        internal void Clear()
        {
            this.cache.Clear();
        }

        /// <summary>
        /// <para>Checks to see if a cache entry exists for a specific command on a specific connection</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to check.</para>
        /// </param>
        /// <returns>True if the parameters are already cached for the provided command, false otherwise</returns>
        protected virtual bool AlreadyCached(DBCommandWrapper command)
        {
            return this.cache.IsParameterSetCached(command.Command);
        }

        /// <summary>
        /// <para>Adds parameters to a command using the cache</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to add the parameters.</para>
        /// </param>
        protected virtual void AddParametersFromCache(DBCommandWrapper command)
        {
            IDataParameter[] parameters = this.cache.GetCachedParameterSet(command.Command);

            foreach (IDataParameter p in parameters)
            {
                command.Command.Parameters.Add(p);
            }
        }

        private static IDataParameter[] CreateParameterCopy(DBCommandWrapper command)
        {
            IDataParameterCollection parameters = command.Command.Parameters;
            IDataParameter[] parameterArray = new IDataParameter[parameters.Count];
            parameters.CopyTo(parameterArray, 0);

            return ParameterCachingMechanism.CloneParameters(parameterArray);
        }
    }
}