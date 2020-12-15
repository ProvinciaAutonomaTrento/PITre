using System;
using System.Web;
using System.Web.UI;
using System.Collections;
using log4net;

namespace SAAdminTool.SiteNavigation
{
	/// <summary>
	/// Classe per la gestione dello stack dei contesti di chiamata
	/// </summary>
	public sealed class CallContextStack
	{
        private static ILog logger = LogManager.GetLogger(typeof(CallContextStack));
		/// <summary>
		/// Chiave di sessione relativa allo stack
		/// </summary>
		private const string CONTEXT_STACK_SESSION_KEY="CallContextStack.contextStack";

		/// <summary>
		/// Chiave di sessione relativa al contesto corrente
		/// </summary>
		private const string CURRENT_CONTEXT_SESSION_KEY="CallContextStack.CurrentContext";

		private CallContextStack()
		{
		}

		#region Private static methods

		/// <summary>
		/// Stack di contesti
		/// </summary>
		private static Stack ContextStack
		{
			get
			{	
				if (HttpContext.Current.Session[CONTEXT_STACK_SESSION_KEY]==null)
					HttpContext.Current.Session.Add(CONTEXT_STACK_SESSION_KEY,new Stack());

				return (Stack) HttpContext.Current.Session[CONTEXT_STACK_SESSION_KEY];
			}
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Verifica se lo stack di chiamate è vuoto o meno
		/// </summary>
		public static bool IsEmpty
		{
			get
			{
				return (ContextStack.Count == 0); // && Context.CurrentContext == null);
			}
		}

		/// <summary>
		/// Rimozione stack contesto di chiamate
		/// </summary>
		public static void Clear()
		{
			CallContextStack.CurrentContext=null;

			foreach (CallContext context in ContextStack)
			{
				context.Dispose();
			}

			ContextStack.Clear();
		}

		/// <summary>
		/// Contesto corrente
		/// </summary>
		public static CallContext CurrentContext
		{
			get
			{
				CallContext current=null;

				if (HttpContext.Current.Session[CURRENT_CONTEXT_SESSION_KEY]!=null)
					current=HttpContext.Current.Session[CURRENT_CONTEXT_SESSION_KEY] as CallContext;

				return current;

			}
			set
			{
				HttpContext.Current.Session[CURRENT_CONTEXT_SESSION_KEY]=value;
			}
		}

		/// <summary>
		/// Reperimento contesto chiamante senza rimuoverlo dallo stack
		/// </summary>
		public static CallContext CallerContext
		{
			get
			{
				CallContext current=null;

				Stack stack=CallContextStack.ContextStack;

				if (stack.Count>0)
				{
					current=(CallContext) stack.Peek();
				}

				return current;
			}
		}

		/// <summary>
		/// Ripristiono contesto chiamante
		/// </summary>
		/// <returns></returns>
		public static CallContext RestoreCaller()
		{	
			CallContext contextToRestore=null;

			Stack stack=CallContextStack.ContextStack;

			if (stack.Count>0)
			{
                if (CallContextStack.CurrentContext != null)
                {
                    // Deallocazione risorse contesto
                    CallContextStack.CurrentContext.Dispose();
                }

				// Il contesto chiamante viene eliminato dallo stack
				contextToRestore=(CallContext) CallContextStack.ContextStack.Pop();
                
				// Il contesto chiamante diventa il contesto corrente
				CallContextStack.CurrentContext=contextToRestore;
				
				// Ripristino stato di sessione
				CallContext.RestoreContextSessionState(contextToRestore);

				// Impostazione parametro "back" in QueryString 
                contextToRestore.IsBack = true;
                
                // Log dell'url del contesto da ripristinare
                logger.Debug("ContextName: " + contextToRestore.ContextName  + " - BackUrl: " + contextToRestore.Url);
			}

			return contextToRestore;
		}

        /// <summary>
        /// Impostazione contesto corrente
        /// </summary>
        /// <param name="context">
        /// Nuovo contesto da inserire
        /// </param>
        /// <param name="forceInsert">
        /// Se true, viene forzato l'inserimento del contesto 
        /// </param>
        /// <returns></returns>
        public static bool SetCurrentContext(CallContext context, bool forceInsert)
        {
            bool inserted = false;

            CallContext currentContext = CallContextStack.CurrentContext;

            bool canInsert = true;

            if (currentContext != null)
            {
                if (forceInsert)
                    // Forzare l'inserimento del contesto
                    canInsert = true;
                else
                    // Verifica se il contesto da inserire nello stack sia lo stesso di quello corrente
                    canInsert = (!currentContext.Equals(context));
            }

            if (canInsert)
            {
                // Impostazione del contesto corrente
                CallContextStack.CurrentContext = context;

                if (currentContext != null)
                {
                    // Impostazione nello stack di contesti
                    CallContextStack.ContextStack.Push(currentContext);
                }

                // Contesto inserito
                inserted = true;
            }

            return inserted;
        }

		/// <summary>
		/// Impostazione contesto corrente
		/// </summary>
		/// <param name="context"></param>
        /// <returns>
        /// True se il contesto è stato inserito correttamente
        /// nello stack ed è diventato il contesto corrente
        /// False se il contesto non è stato inserito in quanto
        /// è lo stesso del contesto corrente
        /// </returns>
		public static bool SetCurrentContext(CallContext context)
		{
            return SetCurrentContext(context, false);
		}

		/// <summary>
		/// Reperimento lista contesti disponibili
		/// </summary>
		/// <returns></returns>
		public static CallContext[] GetContextList()
		{
			ArrayList list=new ArrayList();

			Stack stack=CallContextStack.ContextStack;

			IEnumerator enumerator=stack.GetEnumerator();

			while (enumerator.MoveNext())
			{
				list.Add((CallContext) enumerator.Current);
			}

			return (CallContext[]) list.ToArray(typeof(CallContext));
		}

		#endregion
	}
}
