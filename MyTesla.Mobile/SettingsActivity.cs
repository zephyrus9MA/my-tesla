﻿using System;
using Android.App;
using Android.OS;
using Android.Content;
using Android.Preferences;
using Android.Content.PM;
using Android.Content.Res;
//using Android.Support.V7.Preferences;

//android.support.v7.preference.PreferenceFragmentCompat

//using Android.Support.V7.Preferences;


namespace MyTesla.Mobile
{
    [Activity(Label = "MyTesla.Android Settings", Icon = "@drawable/ic_settings_white_48dp", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            FragmentManager.BeginTransaction().Replace(Android.Resource.Id.Content, new MyPreferenceFragment())
                                              .AddToBackStack(null)
                                              .Commit();
        }


        public class MyPreferenceFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
        //public class MyPreferenceFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener
        {
            public override void OnCreate(Bundle savedInstanceState) {
                base.OnCreate(savedInstanceState);
                AddPreferencesFromResource(Resource.Xml.preferences);
            }


            public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key) {
                var pref = FindPreference(key);

                if (pref is ListPreference) {
                    ListPreference listPref = (ListPreference)pref;
                    listPref.Summary = listPref.Entry;
                }
            }


            public override void OnResume() {
                base.OnResume();
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
            }


            public override void OnPause() {
                PreferenceManager.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
                base.OnPause();
            }

            //public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey) {
            //    AddPreferencesFromResource(Resource.Xml.preferences);

            //    OnSharedPreferenceChanged(PreferenceManager.SharedPreferences, Constants.PrefKeys.VEHICLE_LOCATION);
            //}

        }

    }
}
