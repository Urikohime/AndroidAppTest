using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using TestAppAndroid;
using System.Threading.Tasks;

namespace AppTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            OpenStartScreen();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async void OpenStartScreen()
        {
            await SetContentViewTo(Resource.Layout.activity_main);

            TextView editText = FindViewById<TextView>(Resource.Id.textView1);
            Button register_btn = FindViewById<Button>(Resource.Id.register_btn);
            Button login_btn = FindViewById<Button>(Resource.Id.login_btn);



            if (!Connector.IsLoggedIn())
            {
                register_btn.Click += (sender, e) =>
                {
                    OpenRegisterScreen();
                };

                login_btn.Click += (sender, e) =>
                {

                };
            }
            else if (Connector.IsLoggedIn())
            {
                login_btn.Visibility = Android.Views.ViewStates.Invisible;
                register_btn.Visibility = Android.Views.ViewStates.Invisible;
            }
        }

        private async void OpenRegisterScreen()
        {
            await SetContentViewTo(Resource.Layout.register_screen);

            EditText username_in = FindViewById<EditText>(Resource.Id.username_input);
            EditText password_in = FindViewById<EditText>(Resource.Id.password_input);
            EditText email_in = FindViewById<EditText>(Resource.Id.email_input);
            EditText swcode_in = FindViewById<EditText>(Resource.Id.swcode_input);
            EditText passwordcheck_in = FindViewById<EditText>(Resource.Id.password_check);
            TextView username_err = FindViewById<TextView>(Resource.Id.username_error);
            TextView password_err = FindViewById<TextView>(Resource.Id.password_error);
            TextView passwordcheck_err = FindViewById<TextView>(Resource.Id.passwordcheck_error);
            TextView swcode_err = FindViewById<TextView>(Resource.Id.swcode_error);
            TextView email_err = FindViewById<TextView>(Resource.Id.email_error);
            Button regscreen_back_btn = FindViewById<Button>(Resource.Id.regscreen_back_btn);
            Button regscreen_submit_btn = FindViewById<Button>(Resource.Id.submit_btn);


            username_err.Text = "";
            password_err.Text = "";
            passwordcheck_err.Text = "";
            swcode_err.Text = "";
            email_err.Text = "";

            regscreen_back_btn.Click += (sender, e) =>
            {
                OpenStartScreen();
            };

            username_in.TextChanged += (sender, e) =>
            {
                username_in.Text.Trim().Replace(' ', '_');
                if (!DoSomeThings.IsUsernameLongEnough(username_in.Text) && username_in.Text != "")
                {
                    username_err.Text = "Username has to be at least 5 characters";
                }
                else
                {
                    username_err.Text = "";
                }
            };

            password_in.TextChanged += (sender, e) =>
            {
                if (!DoSomeThings.IsPasswordStrongEnough(password_in.Text) && password_in.Text != "")
                {
                    password_err.Text = "Password has to be least 10 characters long and contain one uppercase letter and a symbol";
                }
                else
                {
                    password_err.Text = "";
                }
            };

            passwordcheck_in.TextChanged += (sender, e) =>
            {
                if (passwordcheck_in.Text != password_in.Text && passwordcheck_in.Text != "")
                {
                    passwordcheck_err.Text = "Passwords do not match";
                }
                else
                {
                    passwordcheck_err.Text = "";
                }
            };

            swcode_in.TextChanged += (sender, e) =>
            {
                if (swcode_in.Text != "" && !DoSomeThings.IsSwCodeValid(swcode_in.Text))
                {
                    swcode_err.Text = "SW-Code is not valid";
                }
                else
                {
                    swcode_err.Text = "";
                }
            };

            email_in.TextChanged += (sender, e) =>
            {
                if (email_in.Text != "" && !DoSomeThings.IsEmailValid(email_in.Text))
                {
                    email_err.Text = "Email is not valid";
                }
                else
                {
                    email_err.Text = "";
                }
            };

            regscreen_submit_btn.Click += (sender, e) =>
            {
                if (!Connector.IsCodeTaken(swcode_in.Text) && DoSomeThings.IsUsernameLongEnough(username_in.Text) &&
                DoSomeThings.IsPasswordStrongEnough(password_in.Text) && DoSomeThings.IsSwCodeValid(swcode_in.Text) &&
                DoSomeThings.IsEmailValid(email_in.Text) && !Connector.IsNameTaken(username_in.Text))
                {
                    if(Encripter.Encode(password_in.Text) != null)
                    {
                        OpenConnectionLoadingScreen(username_in.Text, Encripter.Encode(password_in.Text), swcode_in.Text, email_in.Text);
                    }
                }
                else if(Connector.IsCodeTaken(swcode_in.Text) || Connector.IsNameTaken(username_in.Text))
                {
                    if (Connector.IsCodeTaken(swcode_in.Text))
                    { swcode_err.Text = "SW-Code already in use, if this is a mistake,\nplease contact us at kontakt@abyteoflove.de"; }
                    if (Connector.IsNameTaken(username_in.Text))
                    { username_err.Text = "Username already in use"; }
                }
            };
        }

        private async void OpenLoginScreen()
        {
            await SetContentViewTo(Resource.Layout.login_screen);

            EditText username_in = FindViewById<EditText>(Resource.Id.username_input);
            EditText password_in = FindViewById<EditText>(Resource.Id.password_input);
            TextView general_err = FindViewById<TextView>(Resource.Id.general_error);
            Button submit_btn = FindViewById<Button>(Resource.Id.submit_btn);
            Button back_btn = FindViewById<Button>(Resource.Id.back_btn);

            submit_btn.Click += (sender, e) =>
            {

            };

            back_btn.Click += (sender, e) =>
            {

            };
        }

        private async void OpenConnectionLoadingScreen(String user, String pass, String swcode, String email)
        {
            await SetContentViewTo(Resource.Layout.connection_loading);
            try
            {
                await Connector.Register(user, pass, swcode, email);
                OpenRegisterSuccessScreen();
            }
            catch (Exception e)
            {
                await Task.Delay(100);
                OpenConnectionFailureScreen(e.Message);
            }
        }

        private async void OpenConnectionFailureScreen(String err)
        {
            await SetContentViewTo(Resource.Layout.connection_failed);

            Button connfailure_back_btn = FindViewById<Button>(Resource.Id.connection_failure_back_btn);
            TextView error_msg = FindViewById<TextView>(Resource.Id.connection_failure_info_body);

            error_msg.Text += "\n" + err;

            connfailure_back_btn.Click += (sender, e) =>
            {
                OpenStartScreen();
            };
        }

        private async void OpenRegisterSuccessScreen()
        {
            await SetContentViewTo(Resource.Layout.register_success);

            Button regsuccess_back_btn = FindViewById<Button>(Resource.Id.regscreen_success_back_btn);

            regsuccess_back_btn.Click += (sender, e) =>
            {
                OpenStartScreen();
            };

            await AutoRedirect();
        }

        private async void OpenConnectionScreen()
        {
            try
            {
                await SetContentViewTo(Resource.Layout.connection_loading);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task SetContentViewTo(int layoutId)
        {
            SetContentView(layoutId);
            await Task.Delay(100);
        }

        private async Task AutoRedirect()
        {
            await Task.Delay(3000);
            OpenStartScreen();
        }
    }
}