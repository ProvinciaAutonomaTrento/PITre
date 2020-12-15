// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using InformaticaTrentinaPCL.iOS.Helper;
using UIKit;

namespace InformaticaTrentinaPCL.iOS.TabBar.Document.Action.Root.Storyboard
{
    public partial class ControllerPinOtp : UIViewController,IUITextFieldDelegate
	{

        Action<String> CallbackPin;
        Action<String> CallbackOTP;
        Action<String> CallbackButtonOTP;
        Action<String> CallbackRememberPin;
        bool isEnabledButton = true;
        bool isHiddenButton = false;
        bool isOtpButton = false;

        public ControllerPinOtp (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Refresh();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

          // if (Utility.IsTablet())
           // rightConstraint.Constant = labelButtonOtp.Frame.Width+10;
            
            if (isHiddenButton)
            {
                labelButtonOtp.Hidden = true;
                buttonOtp.Hidden = true;
                labelButtonOtp.Text = "";
                this.View.RemoveConstraint(otpRight);
               // rightConstraint.Constant = 20;
            }

            if (!this.isOtpButton)
            {
                this.View.RemoveConstraint(pinRight);
                buttonRemberPin.Hidden = true;
                labelRememberPin.Hidden = true;
            }
        }

        [Export("textFieldShouldReturn:")]
        public bool ShouldReturn(UITextField textField)
        {
            textField.EndEditing(true);
            if (textField == textPin && CallbackPin != null)
            {
                CallbackPin(textField.Text);
            }
            else if (textField == texOtp && CallbackOTP != null)
            {
                textField.Text = textField.Text.ToUpper();
                CallbackOTP(textField.Text);
            }
            return true;
        }

        partial void ActionButtonOtp(NSObject sender)
        {
            if (CallbackButtonOTP != null)
            {
                CallbackButtonOTP(texOtp.Text);
            }
        }

        partial void ActionbuttonRememberPin(NSObject sender)
        {
            if (this.CallbackRememberPin != null)
            {
                this.CallbackRememberPin(textPin.Text);
            }
        }

        [Export("textFieldShouldBeginEditing:")]
        public bool ShouldBeginEditing(UITextField textField)
        {
            return true;
        }

        [Export("textField:shouldChangeCharactersInRange:replacementString:")]
        public bool ShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
        {
            if (replacementString != "\n")
            {
                NSString newString = ((NSString)textField.Text).Replace(range, (NSString)replacementString);
                textField.Text = newString;
            }

            if (textField == textPin && CallbackPin != null)
            {
                CallbackPin(textField.Text);
            }
            else if (textField == texOtp && CallbackOTP != null)
            {
                textField.Text = textField.Text.ToUpper();
                CallbackOTP(textField.Text);
            }

            if (replacementString == "\n")
                textField.EndEditing(true);

            return false;
        }

        public void SetClass
        (   bool isEnabledButton,
            bool isHiddenButton,
         bool isOtpButton,
            Action<String> CallbackPin,
            Action<String> CallbackOTP,
            Action<String> CallbackButtonOTP,
            Action<String> CallbackRememberPin
        )
        {
            this.isEnabledButton = isEnabledButton;
            this.isHiddenButton = isHiddenButton;
            this.isOtpButton = isOtpButton;
            this.CallbackOTP = CallbackOTP;
            this.CallbackPin = CallbackPin;
            this.CallbackButtonOTP = CallbackButtonOTP;
            this.CallbackRememberPin = CallbackRememberPin;
        }

        public void Refresh()
        {
            texOtp.Delegate = this;
            textPin.Delegate = this;
            footer1.BackgroundColor = Colors.COLOR_SEARCH_FOOTER;
            footer2.BackgroundColor = Colors.COLOR_SEARCH_FOOTER;
            textPin.Placeholder = Utility.LanguageConvert("editText_pin").ToUpper();
            texOtp.Placeholder = Utility.LanguageConvert("editText_otp").ToUpper();
            textPin.SecureTextEntry = true;
            texOtp.SecureTextEntry = true;
            Font.SetCustomStyleFont(textPin, Font.INPUT);
            Font.SetCustomStyleFont(texOtp, Font.INPUT);
            Font.SetCustomStyleFont(labelButtonOtp, Font.LINK_TITLE_BUTTON_BLUE, UITextAlignment.Left);
            Font.SetCustomStyleFont(labelRememberPin, Font.LINK_TITLE_BUTTON_GRAY,UITextAlignment.Left);
            labelButtonOtp.Text = Utility.LanguageConvert("button_otp").ToUpper();
            labelButtonOtp.Lines = 2;
            labelRememberPin.Lines = 2;
            labelRememberPin.Text = Utility.LanguageConvert("button_pin");
            buttonOtp.Enabled = isEnabledButton;
            buttonOtp.Hidden =  isHiddenButton;
        }

	}
}
