using System.Collections.Generic;

namespace perf
{
   public static class Localization
   {
      public static bool isEnglish { get; set; }

      // drawable

      public static string btn_abort                  { get { return localizeDrawable("btn_abort.png");                      } }
      public static string btn_agree                  { get { return localizeDrawable("btn_agree.png");                      } }
      public static string btn_agree_inactive         { get { return localizeDrawable("btn_agree_inactive.png");             } }
      public static string btn_calibration_done       { get { return localizeDrawable("btn_calibration_done.png");           } }
      public static string btn_calibration_run        { get { return localizeDrawable("btn_calibration_run.png");            } }
      public static string btn_delete                 { get { return localizeDrawable("btn_delete_test.png");                } }
      public static string btn_delete_inactive        { get { return localizeDrawable("btn_delete_test_inactive.png");       } }
      public static string btn_height_active          { get { return localizeDrawable("btn_height_active.png");              } }
      public static string btn_height                 { get { return localizeDrawable("btn_height.png");                     } }
      public static string btn_mode1_new              { get { return localizeDrawable("btn_mode1_new.png");                  } }
      public static string btn_mode1_results          { get { return localizeDrawable("btn_mode1_results.png");              } }
      public static string btn_mode1_run              { get { return localizeDrawable("btn_mode1_run.png");                  } }
      public static string btn_mode2_new              { get { return localizeDrawable("btn_mode2_new.png");                  } }
      public static string btn_mode2_results          { get { return localizeDrawable("btn_mode2_results.png");              } }
      public static string btn_mode2_run              { get { return localizeDrawable("btn_mode2_run.png");                  } }
      public static string btn_mode3_results          { get { return localizeDrawable("btn_mode3_results.png");              } }
      public static string btn_mode3_new              { get { return localizeDrawable("btn_mode3_new.png");                  } }
      public static string btn_mode3_run              { get { return localizeDrawable("btn_mode3_run.png");                  } }
      public static string btn_profile_calibration    { get { return localizeDrawable("btn_profile_calibration.png");        } }
      public static string btn_profile_create         { get { return localizeDrawable("btn_profile_create.png");             } }
      public static string btn_profile_delete         { get { return localizeDrawable("btn_profile_delete.png");             } }
      public static string btn_profile_edit           { get { return localizeDrawable("btn_profile_edit.png");               } }
      public static string btn_profile_vehicle        { get { return localizeDrawable("btn_profile_vehicle.png");            } }
      public static string btn_run_inactive           { get { return localizeDrawable("btn_run_inactive.png");               } }
      public static string btn_settings_filter_medium { get { return localizeDrawable("btn_settings_filter_medium.png");     } }
      public static string btn_settings_filter_off    { get { return localizeDrawable("btn_settings_filter_off.png");        } }
      public static string btn_settings_filter_strong { get { return localizeDrawable("btn_settings_filter_strong.png");     } }
      public static string btn_settings_kph           { get { return localizeDrawable("btn_settings_kph.png");               } }
      public static string btn_settings_meter         { get { return localizeDrawable("btn_settings_meter.png");             } }
      public static string btn_settings_feet          { get { return localizeDrawable("btn_settings_feet.png");              } }
      public static string btn_settings_mph           { get { return localizeDrawable("btn_settings_mph.png");               } }
      public static string btn_settings_sound_off     { get { return localizeDrawable("btn_settings_sound_off.png");         } }
      public static string btn_settings_sound_on      { get { return localizeDrawable("btn_settings_sound_on.png");          } }
      public static string btn_settings_tip_no        { get { return localizeDrawable("btn_settings_tip_no.png");            } }
      public static string btn_settings_tip_yes       { get { return localizeDrawable("btn_settings_tip_yes.png");           } }
      public static string btn_setup_add              { get { return localizeDrawable("btn_setup_add.png");                  } }
      public static string btn_setup_add_inactive     { get { return localizeDrawable("btn_setup_add_inactive.png");         } }
      public static string btn_setup_default          { get { return localizeDrawable("btn_setup_default.png");              } }
      public static string btn_setup_remove           { get { return localizeDrawable("btn_setup_remove.png");               } }
      public static string btn_setup_remove_inactive  { get { return localizeDrawable("btn_setup_remove_inactive.png");      } }
      public static string btn_setup_start            { get { return localizeDrawable("btn_setup_start.png");                } }
      public static string btn_setup_stop             { get { return localizeDrawable("btn_setup_stop.png");                 } }
      public static string btn_setup_switch_distance  { get { return localizeDrawable("btn_setup_switch_distance.png");      } }
      public static string btn_setup_switch_speed     { get { return localizeDrawable("btn_setup_switch_speed.png");         } }
      public static string btn_speeddist_active       { get { return localizeDrawable("btn_speeddist_active.png");           } }
      public static string btn_speeddist              { get { return localizeDrawable("btn_speeddist.png");                  } }
      public static string btn_speedtime_active       { get { return localizeDrawable("btn_speedtime_active.png");           } }
      public static string btn_speedtime              { get { return localizeDrawable("btn_speedtime.png");                  } }
      public static string stop_vehicle               { get { return localizeDrawable("stop_vehicle.png");                   } }
      public static string tab_acceleration_active    { get { return localizeDrawable("tab_acceleration_active.png");        } }
      public static string tab_acceleration           { get { return localizeDrawable("tab_acceleration.png");               } }
      public static string tab_acceleration_results   { get { return localizeDrawable("tab_acceleration_results.png");       } }
      public static string tab_brake_active           { get { return localizeDrawable("tab_brake_active.png");               } }
      public static string tab_brake                  { get { return localizeDrawable("tab_brake.png");                      } }
      public static string tab_brake_results          { get { return localizeDrawable("tab_brake_results.png");              } }
      public static string tab_results                { get { return localizeDrawable("tab_results.png");                    } }
      public static string tab_zero_active            { get { return localizeDrawable("tab_zero_active.png");                } }
      public static string tab_zero                   { get { return localizeDrawable("tab_zero.png");                       } }
      public static string tab_zero_results           { get { return localizeDrawable("tab_zero_results.png");               } }
      public static string ovr_purchase               { get { return localizeDrawable("ovr_purchase.png");                   } }
      public static string ovr_purchase_error         { get { return localizeDrawable("ovr_purchase_error.png");             } }

      static string localizeDrawable(string s)
      {
         s = "@drawable/" + s;

         if (isEnglish == false)
         {
            int i = s.LastIndexOf(".", System.StringComparison.Ordinal); // ordinal = use ascii code

            if (i >= 0)
            {
               s = s.Insert(i, "_de");
            }
         }

         return s;
      }

      // string

      public static string tip01                      { get { return localizeString("tip01");                     } }
      public static string tip02                      { get { return localizeString("tip02");                     } }
      public static string tip03                      { get { return localizeString("tip03");                     } }
      public static string tip04                      { get { return localizeString("tip04");                     } }
      public static string tip05                      { get { return localizeString("tip05");                     } }
      public static string tip06                      { get { return localizeString("tip06");                     } }
      public static string tip07                      { get { return localizeString("tip07");                     } }
      public static string tip08                      { get { return localizeString("tip08");                     } }
      public static string tip09                      { get { return localizeString("tip09");                     } }
      public static string tip10                      { get { return localizeString("tip10");                     } }
      public static string tip11                      { get { return localizeString("tip11");                     } }
      public static string tip12                      { get { return localizeString("tip12");                     } }
      public static string tip13                      { get { return localizeString("tip13");                     } }
      public static string tip14                      { get { return localizeString("tip14");                     } }
      public static string tip15                      { get { return localizeString("tip15");                     } }
      public static string tip16                      { get { return localizeString("tip16");                     } }
      public static string tip17                      { get { return localizeString("tip17");                     } }
      public static string tip18                      { get { return localizeString("tip18");                     } }
      public static string tip19                      { get { return localizeString("tip19");                     } }
      public static string tip20                      { get { return localizeString("tip20");                     } }
      public static string pageCalibrationHead        { get { return localizeString("pageCalibrationHead");       } }
      public static string pageCalibrationHelp        { get { return localizeString("pageCalibrationHelp");       } }
      public static string pageCalibrationStart       { get { return localizeString("pageCalibrationStart");      } }
      public static string pageCalibrationOk          { get { return localizeString("pageCalibrationOk");         } }
      public static string pageCalibrationNotOk       { get { return localizeString("pageCalibrationNotOk");      } }
      public static string pageDisclaimerHead         { get { return localizeString("pageDisclaimerHead");        } }
      public static string pageDisclaimerText         { get { return localizeString("pageDisclaimerText");        } }
      public static string pageDisclaimerSwitch       { get { return localizeString("pageDisclaimerSwitch");      } }
      public static string pageHelpHead               { get { return localizeString("pageHelpHead");              } }
      public static string pageHelpAppStore           { get { return localizeString("pageHelpAppStore");          } }
      public static string pageHelpMode1              { get { return localizeString("pageHelpMode1");             } }
      public static string pageHelpMode2              { get { return localizeString("pageHelpMode2");             } }
      public static string pageHelpMode3              { get { return localizeString("pageHelpMode3");             } }
      public static string pageHelpPurchase           { get { return localizeString("pageHelpPurchase");          } }
      public static string pageHelpAbout              { get { return localizeString("pageHelpAbout");             } }
      public static string pageHelpContact            { get { return localizeString("pageHelpContact");           } }
      public static string pageHelpFollow             { get { return localizeString("pageHelpFollow");            } }
      public static string pageMainGpsOk              { get { return localizeString("pageMainGpsOk");             } }
      public static string pageMainGpsNotOk           { get { return localizeString("pageMainGpsNotOk");          } }
      public static string pageMainMountOk            { get { return localizeString("pageMainMountOk");           } }
      public static string pageMainMountNotOk         { get { return localizeString("pageMainMountNotOk");        } }
      public static string pageMainSpeedOk            { get { return localizeString("pageMainSpeedOk");           } }
      public static string pageMainSpeedNotOk         { get { return localizeString("pageMainSpeedNotOk");        } }
      public static string pageProfileHead            { get { return localizeString("pageProfileHead");           } }
      public static string pageProfileEditHead        { get { return localizeString("pageProfileEditHead");       } }
      public static string pageProfileEditCreate      { get { return localizeString("pageProfileEditCreate");     } }
      public static string pageProfileEditDefault     { get { return localizeString("pageProfileEditDefault");    } }
      public static string pageResultEmpty            { get { return localizeString("pageResultEmpty");           } }
      public static string pageResultDistance         { get { return localizeString("pageResultDistance");        } }
      public static string pageResultHeight           { get { return localizeString("pageResultHeight");          } }
      public static string pageResultInfoRun          { get { return localizeString("pageResultInfoRun");         } }
      public static string pageResultInfoDate         { get { return localizeString("pageResultInfoDate");        } }
      public static string pageResultInfoTime         { get { return localizeString("pageResultInfoTime");        } }
      public static string pageResultInfoVehicle      { get { return localizeString("pageResultInfoVehicle");     } }
      public static string pageResultShareTitle       { get { return localizeString("pageResultShareTitle");      } }
      public static string pageResultShareSubject     { get { return localizeString("pageResultShareSubject");    } }
      public static string pageResultSpeed            { get { return localizeString("pageResultSpeed");           } }
      public static string pageRunStatusResume        { get { return localizeString("pageRunStatusResume");       } }
      public static string pageRunTimeSplit           { get { return localizeString("pageRunTimeSplit");          } }
      public static string pageRunTimeTotal           { get { return localizeString("pageRunTimeTotal");          } }
      public static string pageConfigHead             { get { return localizeString("pageConfigHead");            } }
      public static string pageSetupCaption           { get { return localizeString("pageSetupCaption");          } }
      public static string pageSetupDefault           { get { return localizeString("pageSetupDefault");          } }
      public static string pageSetupDistance          { get { return localizeString("pageSetupDistance");         } }
      public static string pageSetupDistStart         { get { return localizeString("pageSetupDistStart");        } }
      public static string pageSetupDistEnd           { get { return localizeString("pageSetupDistEnd");          } }
      public static string pageSetupSpeed             { get { return localizeString("pageSetupSpeed");            } }
      public static string pageSetupSpeedStart        { get { return localizeString("pageSetupSpeedStart");       } }
      public static string pageSetupSpeedEnd          { get { return localizeString("pageSetupSpeedEnd");         } }
      public static string pageSetupZeroStart         { get { return localizeString("pageSetupZeroStart");        } }
      public static string pageSetupZeroEnd           { get { return localizeString("pageSetupZeroEnd");          } }
      public static string pageTextCalibRemindHead    { get { return localizeString("pageTextCalibRemindHead");   } }
      public static string pageTextCalibRemindText    { get { return localizeString("pageTextCalibRemindText");   } }
      public static string pageTextHelpHead           { get { return localizeString("pageTextHelpHead");          } }
      public static string pageTextHelpCalibration    { get { return localizeString("pageTextHelpCalibration");   } }
      public static string pageTextHelpProfile        { get { return localizeString("pageTextHelpProfile");       } }
      public static string pageTextHelpProfileEdit    { get { return localizeString("pageTextHelpProfileEdit");   } }
      public static string pageTextHelpResults        { get { return localizeString("pageTextHelpResults");       } }
      public static string pageTextHelpSetup          { get { return localizeString("pageTextHelpSetup");         } }
      public static string pageTextHelpConfig         { get { return localizeString("pageTextHelpConfig");        } }
      public static string pageTipOfDayHead           { get { return localizeString("pageTipOfDayHead");          } }
      public static string pageTipOfDaySwitch         { get { return localizeString("pageTipOfDaySwitch");        } }
      public static string runModeAcceleration        { get { return localizeString("runModeAcceleration");       } }
      public static string runModeBrake               { get { return localizeString("runModeBrake");              } }
      public static string runModeZeroToZero          { get { return localizeString("runModeZeroToZero");         } }
      public static string runModeWait                { get { return localizeString("runModeWait");               } }
      public static string runModeWaitGreater         { get { return localizeString("runModeWaitGreater");        } }
      public static string runModeLaunch              { get { return localizeString("runModeLaunch");             } }
      public static string runModeFinish              { get { return localizeString("runModeFinish");             } }
      public static string unitKph                    { get { return localizeString("unitKph");                   } }
      public static string unitMph                    { get { return localizeString("unitMph");                   } }
      public static string unitMeter                  { get { return localizeString("unitMeter");                 } }
      public static string unitFeet                   { get { return localizeString("unitFeet");                  } }
      public static string unitSecond                 { get { return localizeString("unitSecond");                } }
      public static string partYes                    { get { return localizeString("partYes");                   } }
      public static string partNo                     { get { return localizeString("partNo");                    } }
      public static string prepAt                     { get { return localizeString("prepAt");                    } }
      public static string quantityTime               { get { return localizeString("quantityTime");              } }
      public static string quantitySpeed              { get { return localizeString("quantitySpeed");             } }
      public static string quantityDistance           { get { return localizeString("quantityDistance");          } }
      public static string quantityHeight             { get { return localizeString("quantityHeight");            } }

      static readonly Dictionary<string, string> strings_en = new Dictionary<string, string>()
      {
         { "tip01",                            "For successful testing please don't forget to mount your cellphone in your car.\n\nVisit example.com for a list of recommend car mounts, giving you the best test results." },
         { "tip02",                            "Pushing the 'help' button on the main screen gives you all information about the currently active mode.\n\nLook out for the additional buttons with a question mark in the upper right corner of each screen." },
         { "tip03",                            "For successful testing first calibrate your car's specific launch detection.\n\nPush 'Create Profile' and then 'Calibration'.\n\nYour car must be in engine-idle. Your device must be mounted.\n\nPress 'Run' to start calibration." },
         { "tip04",                            "After choosing your test mode at the top of the main screen, all 3 conditions (GPS Ok, Mount Ok, Stand Still Ok) have to be 'green' to 'run' the test." },
         { "tip05",                            "After first start, before running your first test, push the 'Profile' button to create your own user profile." },
         { "tip06",                            "Please note:\n\nPrepare your app configuration and only interact with the app before you start driving.\n\nDont't look at your device's display or operate it during drive.\n\nRecent measurements will be signaled by a sound of this app.\n\nPlease check out your sound config for activated sounds." },
         { "tip07",                            "Pushing 'Results' tab at the top of the main screen gives you the results of your last test.\n\nThe color of the 'Results' button indicates the currend mode the displayed data based on." },
         { "tip08",                            "If you have any problems or questions using this app please contact us.\n\nSend your request to:\n\nExample@mail.com\n\nWe will contact you asap!" },
         { "tip09",                            "This 'Tip of the Day' can be deactivated by checking the checkbox at the bottom of this screen.\n\nGo to 'Configuration' page if you want to reactivate it." },
         { "tip10",                            "Before starting a test first have a look at the 'setup button' (burger menu icon).\n\nIt opens a specific menu for each mode, where you can define your own measurements by setting indivdual start marks and end marks." },
         { "tip11",                            "If you found some bugs, please let as know and send a mail to:\n\nExample@mail.com\n\nWe will do our best to resolve the issue." },
         { "tip12",                            "We recommend activated sounds for this app (by default sounds: on).\n\nDuring a test each finished measurement will be signaled by a sound.\n\nSo there is no need to look at the dispay during test drives." },
         { "tip13",                            "Results of your measurements are only shown if your car came to a full stop.\n\nPress the 'Results' button at the bottom of the screen after you finished a test run." },
         { "tip14",                            "Use the 'Setup Measurement' screen to set up your measurements for speed and distance.\n\nThe 'Results' screen will show measurements depending on the current setup." },
         { "tip15",                            "" },
         { "tip16",                            "" },
         { "tip17",                            "" },
         { "tip18",                            "" },
         { "tip19",                            "" },
         { "tip20",                            "" },
         { "pageCalibrationHead",              "Calibration" },
         { "pageCalibrationHelp",              "Mount the device to a smartphone car mount. Bring the vehicle to standstill, leave the engine running." },
         { "pageCalibrationStart",             "" }, // Press 'Run' to start calibration
         { "pageCalibrationOk",                "Calibration successful" },
         { "pageCalibrationNotOk",             "Calibration not successful" },
         { "pageDisclaimerHead",               "Disclaimer" },
         { "pageDisclaimerText",               "Always obey all traffic laws, rules and regulations.\n\nDo any setting changes or analyzing of measurement results only at a safe standstill of your vehicle.\n\nYou should exercise caution in using this App.\n\nAlways maintain full concentration on the road.\n\nWhile driving you must not look at the App display screen or use the App's buttons.\n\nLooking at small screens while driving is dangerous.\n\nLooking at the display screen and using the buttons while driving will distract your attention.\n\nUse this App responsibly." },
         { "pageDisclaimerSwitch",             "I agree" },
         { "pageHelpHead",                     "How to Use" },
         { "pageHelpAppStore",                 "Play Store won't open or load.\n\nWe’re sorry you’re having issues using the Google Play Store.\nDepending on your issue try below steps to fix your problem. Let's get started:\n\nDid you recently add a Google Account to the device?\nIf yes, your account should be ready soon. Sometimes a new account can take some time to work on the device. Wait 30 minutes and try to use the Play Store again. By then the account should be activated and ready to use.\n\nTry to do the following, check after each step if the Play Store is working:\n- Turn Airplane mode on and off.\n- Switch between Wi-Fi and mobile data.\n- Reset your Wi-Fi router (if you can).\n- Check if other apps that use the internet are working.\n- Check that your account is synced.\n\nGoogle Play Store not working? Here's what you can do:\n- Check your date and time settings. Google checks your Android smartphone's date and time for the Play Store. If the store does not find a time then it could cause some issues.\n- Check your disabled apps. Many apps need other apps in order to function properly. This is especially true when you're dealing with system apps such as the Google Play Store. If you recently disabled an app that could be your problem." },
         { "pageHelpMode1",                    "Car Performance gives you 3 different modes to test your car's performance.\n\nThis mode 'Acceleration' tests your acceleration from 0 Kph to 100 Kph by default.\n\nChoose your prefered Distance Unit (Meter/Feet), Speed Unit (Kph/Mph) and further settings at the 'Configuration' menue.\n\nOnce the 3 precondition icons get a green check mark your device is ready for testing and the 'Run' button appears at the bottom of this screen to start your test." },
         { "pageHelpMode2",                    "Car Performance gives you 3 different modes to test your car's performance.\n\nThis mode 'Brake' tests your brake performance from 100 Kph to 0 Kph by default.\n\nChoose your prefered Distance Unit (Meter/Feet), Speed Unit (Kph/Mph) and further settings at the 'Configuration' menue.\n\nOnce the 3 precondition icons get a green check mark your device is ready for testing and the 'Run' button appears at the bottom of this screen to start your test." },
         { "pageHelpMode3",                    "Car Performance gives you 3 different modes to test your car's performance.\n\nThis mode 'Zero to Zero' unifies the 2 other modes and tests the phase by 0 Kph upon 100 Kph to 0 Kph by default.\n\nChoose your prefered Distance Unit (Meter/Feet), Speed Unit (Kph/Mph) and further settings at the 'Configuration' menue.\n\nOnce the 3 precondition icons get a green check mark your device is ready for testing and the 'Run' button appears at the bottom of this screen to start your test." },
         { "pageHelpPurchase",                 "Purchased in-app products" },
         { "pageHelpAbout",                    "About this app" },
         { "pageHelpContact",                  "Contact" },
         { "pageHelpFollow",                   "Follow us on instagram.com/example" },
         { "pageMainGpsOk",                    "GPS Ok" },
         { "pageMainGpsNotOk",                 "GPS Not Ok" },
         { "pageMainMountOk",                  "Mount Ok" },
         { "pageMainMountNotOk",               "Mount Not Ok" },
         { "pageMainSpeedOk",                  "Standstill Ok" },
         { "pageMainSpeedNotOk",               "Standstill Not Ok" },
         { "pageProfileHead",                  "Select Profile" },
         { "pageProfileEditHead",              "Edit Profile" },
         { "pageProfileEditCreate",            "Create Profile" },
         { "pageProfileEditDefault",           "Example" },
         { "pageResultDistance",               "Distance" },
         { "pageResultHeight",                 "Height" },
         { "pageResultEmpty",                  "Results are empty" },
         { "pageResultInfoRun",                "Run Id" },
         { "pageResultInfoDate",               "Date" },
         { "pageResultInfoTime",               "Time" },
         { "pageResultInfoVehicle",            "Vehicle" },
         { "pageResultShareTitle",             "Car Performance - Share Result" },
         { "pageResultShareSubject",           "Car Performance - Run Number" },
         { "pageResultSpeed",                  "Speed" },
         { "pageRunStatusResume",              "Resume" },
         { "pageRunTimeSplit",                 "Time Split" },
         { "pageRunTimeTotal",                 "Time" },
         { "pageConfigHead",                   "Configuration" },
         { "pageSetupCaption",                 "Setup" },
         { "pageSetupDefault",                 "Default" },
         { "pageSetupDistance",                "Distance" },
         { "pageSetupDistStart",               "Distance Start" },
         { "pageSetupDistEnd",                 "Distance End" },
         { "pageSetupSpeed",                   "Speed" },
         { "pageSetupSpeedStart",              "Speed Start" },
         { "pageSetupSpeedEnd",                "Speed End" },
         { "pageSetupZeroStart",               "Speed Start" },
         { "pageSetupZeroEnd",                 "Target speed" },
         { "pageTextCalibRemindHead",          "Please Note" },
         { "pageTextCalibRemindText",          "For successful testing you should first create your car's profile to calibrate your car's specific launch detection.\n\nFirst of all use the profile button, create your profile (\"Create Profile\" button) and then check out 'Calibration' button to optimize your app to your car's engine-idle." },
         { "pageTextHelpCalibration",          "This 'Calibration' screen let you exactly calibrate this app to your car's specific engine-idle.\n\nThat is the requirement for best test results.\n\nFirst bring your car in engine idle state, savely mount your device and then begin calibrating by pressing the 'run' button.\n\nAfter a short time your calibrating is done and you are ready for your first testing." },
         { "pageTextHelpHead",                 "How to use" },
         { "pageTextHelpProfile",              "This 'Profile' screen contains the list of your vehicles.\n\nBelow the list are 2 buttons 'Create Profile' and 'Edit Profile'.\n\nTo edit a profile, select it form the list, then touch button 'Edit Profile'.\n\nEach profile has four attributes:\n-Name\n-Color\n-Calibration" },
         { "pageTextHelpProfileEdit",          "By pushing the 'Create Profile' button you can name your 'Vehicle' and pick out a specific profile 'Color'.\n\nBy pushing the 'Edit Profile' button you can modify this options of your active profile.\n\nFurthermore you can delete it by pressing the 'Delete Profile' button." },
         { "pageTextHelpResults",              "Once the test finished by pushing the 'Results' button you get to this 'Results' screen.\n\nHere are displayed all captured data of your latest test, categorized in the tabs 'Speed/Time', 'Speed/Distance' and 'Height'.\n\nPush one of it to get the relevant results for each category.\n\nShare your recent data by pressing the 'share' button at the upper mid area." },
         { "pageTextHelpSetup",                "The setup screen let you make specific settings for each mode you select.\n\nDefine your own measurements by setting indivdual start marks and end marks by pressing 'Set New Start' and 'Set New End' and choosing the desired values.\n\nAfter that press the 'Add' button and your new test definition appears at the list below.\n\nAt this list you can select a defined measurement and by pressing the 'Remove' button you can remove it from the list.\n\nTo set your setup screen to default press the red button." },
         { "pageTextHelpConfig",               "This 'Configuration' menue lets you adjust some parameters to your own requirements:\n\nLanguage: English/German\nDistance Unit: Meter/Feet\nSpeed Unit: Kph/Mph\nSpeedo filter: Medium/Strong/Off\nSounds: On/Off\nTip of the Day: On/Off" },
         { "pageTipOfDayHead",                 "Tip of the Day" },
         { "pageTipOfDaySwitch",               "Show tips at startup" },
         { "runModeAcceleration",              "Acceleration" },
         { "runModeBrake",                     "Brake" },
         { "runModeZeroToZero",                "Zero to Zero" },
         { "runModeWait",                      "Waiting for launch..." },
         { "runModeWaitGreater",               "Waiting for speed >" },
         { "runModeLaunch",                    "Launch detected" },
         { "runModeFinish",                    "Measure stop" },
         { "unitKph",                          "kph" },
         { "unitMph",                          "mph" },
         { "unitMeter",                        "m" },
         { "unitFeet",                         "ft" },
         { "unitSecond",                       "s" },
         { "partYes",                          "yes" },
         { "partNo",                           "no" },
         { "prepAt",                           "at" },
         { "quantityTime",                     "Time" },
         { "quantitySpeed",                    "Speed" },
         { "quantityDistance",                 "Distance" },
         { "quantityHeight",                   "Height" }
      };

      static readonly Dictionary<string, string> strings_de = new Dictionary<string, string>()
      {
         { "tip01",                            "Für beste Testergebnisse sollte dein Smartphone in deinem Auto sicher befestigt sein.\n\nAuf example.com findest du eine Übersicht von Halterungen, die nach unseren Erfahrungen optimale Messergebnisse liefern." },
         { "tip02",                            "Durch das Betätigen der 'Hilfe'-Buttons auf jedem Test-Screen erhältst du Informationen über die Handhabung des jeweiligen Test-Modus.\n\nDesweiteren findest du diverse 'Hilfe'-Buttons, um Infos über den derzeitig angezeigten Screen zu erhalten.\n\nSie befinden sich immer rechts oben." },
         { "tip03",                            "Für erfolgreiche Tests kalibriere zuallererst die App auf den Leerlauf deines Fahrzeugs.\n\nGehe über den Profil-Button mit dem Autosymbol auf die 'Profil Auswahl' und dann auf 'Kalibrierung'.\n\nDein Fahrzeug muss sich im Leerlauf befinden. Dein Smartphone muss in seiner Halterung befestigt sein.\n\nDrücke auf 'Start' um mit der Kalibrierung zu beginnen." },
         { "tip04",                            "Nachdem du über die Tabs am oberen Bildschirmrand im Haupt-Screen deinen Test-Modus ausgewählt hast, müssen alle 3 Symbole in der Mitte des Screens grün markiert erscheinen, damit die Messung starten kann.\n\nDrücke dann auf 'Starten', um mit dem Test zu beginnen." },
         { "tip05",                            "Lege dir zuerst dein eigenes Benutzer-Profil im Profil-Menü an, bevor du deine erste Messung startest.\n\nÜber den Profil-Button mit dem Autosymbol gelangst du dort hin." },
         { "tip06",                            "Bitte beachte:\n\nFühre jegliche Bedienung dieser App ausschließlich bei Fahrzeugstillstand aus und verzichte darauf, während der Fahrt auf den Bildschirm zu schauen oder dein Gerät zu bedienen.\n\nErfolgte Messungen solltest du dir während der Fahrt ausschließlich per Sound signalisieren lassen.\n\nAchte darauf, dass du 'Sounds' im 'Einstellungen'-Screen aktiviert hast." },
         { "tip07",                            "Durch Anwahl des 'Ergebnisse'-Tabs am oberen Bildrand im Haupt-Screen gelangst du zum 'Ergebnisse'-Screen.\n\nEr zeigt dir die Ergebnisse deiner letzten Messung.\n\nAn der Farbe des 'Ergebnisse'-Tabs erkennst du, auf welchen Test-Modus die Ergebnisse beruhen." },
         { "tip08",                            "Bei Problemen oder Fragen zu dieser App kontaktiere uns bitte umgehend.\n\nBitte sende dein Anliegen an:\n\nExample@mail.com\n\nWir bemühen uns um eine schnelle Bearbeitung!" },
         { "tip09",                            "Dieser 'Tipp des Tages' kann mithilfe der Checkbox am unteren Bildschirmrand deaktiviert werden.\n\nIm 'Einstellungen'-Menü kann er auch wieder reaktiviert werden." },
         { "tip10",                            "Bevor du mit dem ersten Test durchstartest, solltest du einen Blick auf den für jeden Messmodus spezifischen 'Messung einrichten'-Screen werfen.\n\nZu diesem gelangst du über den Button mit dem Burger-Symbol, rechts oben.\n\nHier kannst du eigene Messungen festlegen, mit eigenen, von dir selbst definierten, Start- und Endpunkten." },
         { "tip11",                            "Diese App befindet sich in stetiger Weiterentwicklung. Auftretende Fehler in der Funktionalität sind leider nicht auszuschließen.\n\nSollte dir dahingehend etwas auffallen, würden wir uns über dein Feedback sehr freuen.\n\nBitte sende es an:\n\nExample@mail.com" },
         { "tip12",                            "Wir empfehlen dir, die App-Soundausgabe aktiviert zu haben, da während eines Tests jede fertige Messung mit einem Sound signalisiert wird.\n\nSomit gibt es während einer Testfahrt keinen Grund, auf das Display zu schauen." },
         { "tip13",                            "Tests Ergebnisse werden nur angezeigt nachdem du dein Fahrzeug zum Stehen gebracht hast.\n\nDrücke hierfür den 'Ergebnisse'-Button unter dem Tacho." },
         { "tip14",                            "Mit dem 'Messung einrichten'-Screen kannst du Messungen für Geschwindigkeit sowie zurückgelegter Strecke angelegen.\n\nIn Abhängigkeit dieser Einstellungen zeigt der 'Ergebnisse'-Screen deine Testfahrten an." },
         { "tip15",                            "" },
         { "tip16",                            "" },
         { "tip17",                            "" },
         { "tip18",                            "" },
         { "tip19",                            "" },
         { "tip20",                            "" },
         { "pageCalibrationHead",              "Kalibrierung" },
         { "pageCalibrationHelp",              "Befestige das Handy in einer Geräte-Halterung. Bring das Fahrzeug zum Stillstand, der Motor läuft im Leerlauf." },
         { "pageCalibrationStart",             "" }, // Drücke 'Start' um die Kalibrierung zu starten
         { "pageCalibrationOk",                "Kalibrierung erfolgreich" },
         { "pageCalibrationNotOk",             "Kalibrierung nicht erfolgreich" },
         { "pageDisclaimerHead",               "Bitte beachten!" },
         { "pageDisclaimerText",               "Befolge immer alle Verkehrsregeln, Gebote und Vorschriften.\n\nBenutze diese Software nur bei sicherem Stillstand des Fahrzeugs.\n\nDu solltest Vorsicht bei der Verwendung dieser App ausüben.\n\nKonzentriere Dich immer voll auf die Straße.\n\nVerzichte darauf, während der Fahrt auf den App-Bildschirm zu schauen oder die App-Tastatur zu verwenden, da dies deine Aufmerksamkeit erheblich beeinträchigt und für dich und den Straßenverkehr eine Gefahr darstellt.\n\nVerwende diese App verantwortungsbewusst." },
         { "pageDisclaimerSwitch",             "Ich stimme zu" },
         { "pageHelpHead",                     "Anleitung" },
         { "pageHelpAppStore",                 "Play Store wird nicht geöffnet oder geladen.\n\nWir bedauern, dass du Probleme mit dem Google Play Store hast.\nVersuche unten stehende Schritte, um das Problem zu beheben. Los gehts:\n\nHast du vor kurzem ein Google-Konto für das Gerät hinzugefügt?\nDas Konto sollte bald fertig sein. Manchmal kann ein neues Konto einige Zeit dauern, bis es auf dem Gerät funktioniert. Warte 30 Minuten und versuche erneut den Play Store zu verwenden.\n\nVerusche folgende Schritte, schaue nach jedem Schritt, ob der Play Store funktioniert:\n- Schalte den Flugzeugmodus ein und aus.\n- Wähle zwischen WLAN und mobilen Daten.\n- Überprüfe, ob andere Apps, die das Internet nutzen, funktionieren.\n- Stelle sicher, dass dein Konto synchronisiert ist.\n\nGoogle Play Store funktioniert nicht? Das könnte dir helfen:\n- Überprüfe die Einstellungen für Datum und Uhrzeit. Google prüft das Datum und die Uhrzeit des Smartphones für den Play Store. Wenn der Play Store keine Zeit findet, kann er Probleme verursachen.\n- Überprüfe deine deaktivierten Apps. Viele Apps benötigen wiederum andere Apps, um ordnungsgemäß zu funktionieren. Dies gilt insbesondere dann, wenn sie mit Systemanwendungen wie dem Google Play Store umgehen. Wenn du vor kurzem eine App deaktiviert hast, könnte genau diese App dein Problem sein." },
         { "pageHelpMode1",                    "Car Performance bietet dir 3 unterschiedliche Test-Modi.\n\nDies ist der Modus 'Beschleunigung'.\n\nEr testet die Beschleunigungs-Phase von 0 km/h auf 100 km/h als Voreinstellung.\n\nUm diesen Modus nach deinen Anforderungen anzupassen, nutze das Einstellungs-Menü dieser App.\n\nSobald die 3 Icons in der Mitte mit einem grün hinterlegten Häkchen erscheinen, sind alle Voraussetzungen für eine Messung erfüllt.\n\nDer 'Start'-Button am unteren Bildschirmrand ist nun aktivierbar, um mit der Messung zu beginnen." },
         { "pageHelpMode2",                    "Car Performance bietet dir 3 unterschiedliche Test-Modi.\n\nDies ist der Modus 'Bremsen'.\n\nEr testet die Brems-Phase von 100 km/h auf 0 km/h als Voreinstellung.\n\nUm diesen Modus nach deinen Anforderungen anzupassen, nutze das Einstellungs-Menü dieser App.\n\nSobald die 3 Icons in der Mitte mit einem grün hinterlegten Häkchen erscheinen, sind alle Voraussetzungen für eine Messung erfüllt.\n\nDer 'Start'-Button am unteren Bildschirmrand ist nun aktivierbar, um mit der Messung zu beginnen." },
         { "pageHelpMode3",                    "Car Performance bietet dir 3 unterschiedliche Test-Modi.\n\nDies ist der Modus 'Von Null auf Null'.\n\nEr vereint die beiden anderen Modi dieser App in einen kompletten Test.\n\nHier wird der Bereich von 0 km/h auf 100 km/h und wieder auf 0 km/h ausgewertet.\n\nUm diesen Modus nach deinen Anforderungen anzupassen, nutze das Einstellungs-Menü dieser App.\n\nSobald die 3 Icons in der Mitte mit einem grün hinterlegten Häkchen erscheinen, sind alle Voraussetzungen für eine Messung erfüllt.\n\nDer 'Start'-Button am unteren Bildschirmrand ist nun aktivierbar, um mit der Messung zu beginnen." },
         { "pageHelpPurchase",                 "Gekaufte In-App Produkte" },
         { "pageHelpAbout",                    "Über diese App" },
         { "pageHelpContact",                  "Kontakt" },
         { "pageHelpFollow",                   "Folge uns auf instagram.com/example" },
         { "pageMainGpsOk",                    "GPS Ok" },
         { "pageMainGpsNotOk",                 "GPS Nicht Ok" },
         { "pageMainMountOk",                  "Halterung Ok" },
         { "pageMainMountNotOk",               "Halterung Nicht Ok" },
         { "pageMainSpeedOk",                  "Stillstand Ok" },
         { "pageMainSpeedNotOk",               "Stillstand Nicht Ok" },
         { "pageProfileHead",                  "Profil Auswahl" },
         { "pageProfileEditHead",              "Profil Editieren" },
         { "pageProfileEditCreate",            "Profil Erstellen" },
         { "pageProfileEditDefault",           "Beispiel" },
         { "pageResultDistance",               "Distanz" },
         { "pageResultEmpty",                  "Es liegen keine Ergebnisse vor" },
         { "pageResultHeight",                 "Höhe" },
         { "pageResultInfoRun",                "Lauf Nummer" },
         { "pageResultInfoDate",               "Datum" },
         { "pageResultInfoTime",               "Zeit" },
         { "pageResultInfoVehicle",            "Fahrzeug" },
         { "pageResultShareTitle",             "Car Performance - Ergebnis Teilen" },
         { "pageResultShareSubject",           "Car Performance - Lauf Nummer" },
         { "pageResultSpeed",                  "Geschwindigkeit" },
         { "pageRunStatusResume",              "Fahre fort" },
         { "pageRunTimeSplit",                 "Zwischenzeit" },
         { "pageRunTimeTotal",                 "Zeit" },
         { "pageConfigHead",                   "Einstellungen" },
         { "pageSetupCaption",                 "Setup" },
         { "pageSetupDefault",                 "Standard" },
         { "pageSetupDistance",                "Weg" },
         { "pageSetupDistStart",               "Weg Start" },
         { "pageSetupDistEnd",                 "Weg Ende" },
         { "pageSetupSpeed",                   "Geschwindigkeit" },
         { "pageSetupSpeedStart",              "Speed Start" },
         { "pageSetupSpeedEnd",                "Speed Ende" },
         { "pageSetupZeroStart",               "Speed Start" },
         { "pageSetupZeroEnd",                 "Ziel Speed" },
         { "pageTextCalibRemindHead",          "Bitte Beachten" },
         { "pageTextCalibRemindText",          "Für erfolgreiche Tests erstelle zunächst über den Profil-Button im Hauptbildschirm dein eigenes Fahrzeug-Profil ('Neues Profil').\n\nDort benutze bitte zu allererst die Schaltfläche 'Kalibrierung', um die App auf den Leerlauf deines Fahrzeuges optimal auszurichten." },
         { "pageTextHelpCalibration",          "In diesem 'Kalibrierung'-Bildschirm nimmst du eine Eichung der App auf den spezifischen Ruhezustand deines Testfahrzeuges vor.\n\nVersetze zunächst dein Fahrzeug in den Ruhezustand.\n\nBringe nun dein Smartphone sicher an deine Fahrzeughalterung an und starte als nächstes die Kalibrierung über den 'Starten'-Button.\n\nNach kurzer Zeit ist die Kalibrierung abgeschlossen und du kannst mit deinen ersten Tests beginnen." },
         { "pageTextHelpHead",                 "Anleitung" },
         { "pageTextHelpProfile",              "In diesem 'Profil'-Bildschirm siehst du eine Liste deiner Fahrzeuge.\n\nUnterhalb der Liste sind 2 Buttons 'Profil erstellen' und 'Profil editieren'.\n\nUm ein Fahrzeug zu ändern, wähle es an und drücke 'Profil editieren'.\n\nJedes Profil hat diese vier Attribute:\n-Name\n-Farbe\n-Kalibrierung" },
         { "pageTextHelpProfileEdit",          "Nach Anwahl des Buttons 'Neues Profil' kann dein 'Fahrzeug' benannt sowie ein spezifisches 'Symbol' für dein Profil vergeben werden.\n\nDarüberhinaus kannst du unter 'Profil editieren' diese Einstellungen deines gerade aktiven Profils ggf. verändern sowie dieses über 'Profil löschen' wieder entfernen." },
         { "pageTextHelpResults",              "Nach erfolgreichem Abschluss eines Testes gelangst du über den 'Ergebnisse'-Button zum 'Ergebnisse'-Screen.\n\nHier sind alle relevanten Daten deiner letzten Messung in den 3 Tabs 'Speed/Zeit', 'Speed/Distanz' und 'Höhe' kategorisiert und einsehbar.\n\nÜber den 'Teilen'-Button, rechts oben, kannst du die angezeigten Werte in sozialen Netzwerken veröffentlichen." },
         { "pageTextHelpSetup",                "Der Screen 'Konfiguration' gibt dir die Möglichkeit, für den von dir gerade gewählten Modus deine eigenen Messungen zu definieren und anzupassen.\n\nÜber die Buttons 'Startpunkt' und 'Endpunkt' kannst deine neue Messung festlegen mittels 'Hinzufügen' der darunter befindlichen Liste hinzufügen.\n\nDiese Liste zeigt dir alle Messungen, die in diesem Modus durchgeführt werden.\n\nDu kannst die einzelnen Messungen hier anwählen und sie bei Bedarf mittels 'Entfernen'-Button auch wieder von der Liste wegnehmen.\n\nDer rote Button 'Zückucksetzen' versetzt den gewählten Modus wieder zurück auf seine voreingestellte Messkonfiguration." },
         { "pageTextHelpConfig",               "In diesem 'Einstellungen'-Menü können einige Parameter an deine eigenen Testanforderungen angepasst werden:\n\nSprache: Englisch/Deutsch\nEinheit Strecke: Meter/Fuß\nEinheit Speed: km/h oder milen/h\nTacho Filter: Mittel/Stark/Aus\nSounds: An/Aus\nTipp des Tages: An/Aus" },
         { "pageTipOfDayHead",                 "Tipp des Tages" },
         { "pageTipOfDaySwitch",               "Zeige Tipps bei Start" },
         { "runModeAcceleration",              "Beschleunigung" },
         { "runModeBrake",                     "Bremsen" },
         { "runModeZeroToZero",                "Null auf Null" },
         { "runModeWait",                      "Warte auf Start..." },
         { "runModeWaitGreater",               "Warte auf Geschwindigkeit >" },
         { "runModeLaunch",                    "Start erkannt" },
         { "runModeFinish",                    "Messung Ende" },
         { "unitKph",                          "km/h" },
         { "unitMph",                          "mi/h" },
         { "unitMeter",                        "m" },
         { "unitFeet",                         "ft" },
         { "unitSecond",                       "s" },
         { "partYes",                          "ja" },
         { "partNo",                           "nein" },
         { "prepAt",                           "bei" },
         { "quantityTime",                     "Zeit" },
         { "quantitySpeed",                    "Geschwindigkeit" },
         { "quantityDistance",                 "Distanz" },
         { "quantityHeight",                   "Höhe" }
      };

      static string localizeString(string s)
      {
         if (isEnglish)
         {
            if (strings_en.ContainsKey(s))
            {
               return strings_en[s];
            }
         }

         if (strings_de.ContainsKey(s))
         {
            return strings_de[s];
         }

         return "Unknown";
      }
   }
}
