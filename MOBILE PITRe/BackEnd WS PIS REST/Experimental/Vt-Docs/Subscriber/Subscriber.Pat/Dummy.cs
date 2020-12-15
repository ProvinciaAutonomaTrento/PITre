//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Subscriber.Pat.Rules
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class Dummy : Subscriber.Rules.BaseRule
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public override string RuleName
//        {
//            get
//            {
//                return "Dummy";
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public override string[] GetSubRules()
//        {
//            return new string[0];
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        protected override void InternalExecute()
//        {
//            bool computed = false;

//            try
//            {
//                if (this.AreEquals(this.ListenerRequest.EventInfo.PublishedObject.ObjectType, "Documento"))
//                {
//                    Property pFile = this.FindProperty("File");

//                    if (pFile != null)
//                    {
//                        Property pFileName = FindProperty("FileName");

//                        System.IO.File.WriteAllBytes(string.Format(@"c:\temp\publisher\{0}", pFileName.Value.ToString()), pFile.BinaryValue);

//                        computed = true;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                computed = false;

//                this.Response.Rule.Error = new ErrorInfo
//                {
//                    Id = Subscriber.ErrorCodes.UNHANDLED_ERROR,
//                    Message = ex.Message,
//                    Stack = ex.ToString()
//                };
//            }
//            finally
//            {
//                this.Response.Rule.Computed = computed;
//                this.Response.Rule.ComputeDate = DateTime.Now;

//                // Scrittura elemento di pubblicazione nell'history
//                RuleHistoryInfo historyInfo = RuleHistoryInfo.CreateInstance(this.Response.Rule);
//                historyInfo.Author = this.ListenerRequest.EventInfo.Author;
//                historyInfo.ObjectSnapshot = this.ListenerRequest.EventInfo.PublishedObject;
//                historyInfo = DataAccess.RuleHistoryDataAdapter.SaveHistoryItem(historyInfo);
//            }
//        }
//    }
//}
