using System;
using Foundation;

namespace InformaticaTrentinaPCL.iOS.Helper
{

    public class Preferences
    {
        #region Fields & Properties
        private Lazy<NSUserDefaults> userDefaults = new Lazy<NSUserDefaults>(() =>
        {
            return NSUserDefaults.StandardUserDefaults;
        }, true);

        private NSUserDefaults UserDefaults
        {
            get
            {
                return this.userDefaults.Value;
            }
        }
        #endregion

        #region Methods


        public bool GetBoolean(string key, bool defValue = false)
        {
            return this.UserDefaults.BoolForKey(key);
        }

        public int GetInt(string key, int defValue = 0)
        {
            return (int)this.UserDefaults.IntForKey(key);
        }

        public string GetString(string key, string defValue = null)
        {
            return this.UserDefaults.StringForKey(key);
        }

        public Preferences Set(string key, bool value)
        {
            this.UserDefaults.SetBool(value, key);
            this.UserDefaults.Synchronize();
            return this;
        }

        public Preferences Set(string key, int value)
        {
            this.UserDefaults.SetInt(value, key);
            this.UserDefaults.Synchronize();
            return this;
        }

        public Preferences Set(string key, string value)
        {
            this.UserDefaults.SetString(value, key);
            this.UserDefaults.Synchronize();

            return this;
        }
        #endregion Methods
    }

}
   
