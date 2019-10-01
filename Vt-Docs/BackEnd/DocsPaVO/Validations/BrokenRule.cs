using System;

namespace DocsPaVO.Validations
{
	/// <summary>
	/// Codice, descrizione e livello di gravita di una "BusinessRule" non validata
	/// </summary>
    [Serializable()]
	public class BrokenRule
	{
		public BrokenRule()
		{
		}

		public BrokenRule(string id,string description) : this(id,description,BrokenRule.BrokenRuleLevelEnum.Error)
		{	
		}

		public BrokenRule(string id,string description,BrokenRule.BrokenRuleLevelEnum level)
		{
			this.ID=id;
			this.Description=description;
			this.Level=level;
		}

		/// <summary>
		/// Codice della "BusinessRule" non validata
		/// </summary>
		public string ID=string.Empty;

		/// <summary>
		/// Descrizione della "BusinessRule" non validata
		/// </summary>
		public string Description=string.Empty;

		/// <summary>
		/// Livello di gravità della "BusinessRule" non validata
		/// </summary>
		public BrokenRule.BrokenRuleLevelEnum Level=BrokenRule.BrokenRuleLevelEnum.Error;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Description;
        }

		/// <summary>
		/// Enumerazione, tipi di livelli di gravità della "BusinessRule" non validata
		/// </summary>
		public enum BrokenRuleLevelEnum
		{
			Error,
			Warning
		}
	}
}
