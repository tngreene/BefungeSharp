using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BefungeSharp.Menus
{
    public class OptionsMenu : Menu
    {
        public override void OnOpening()
        {
            Menu.DecorateWindowTop("Options Menu");
            base.OnOpening();
        }

        public override void RunLoop()
        {
            do
            {
                this.OnOpening();
                Console.WriteLine("Choose an option to cycle");
                
                Console.WriteLine("1.) Current Language Version: "
                                               + OptionsManager.Get<int>("I", "LF_SPEC_VERSION"));

                Console.WriteLine("2.) Number of Dimensions: "
                                               + OptionsManager.Get<int>("I","LF_DIMENSIONS"));
                
                //Build up the sandbox mode to display
                Console.Write("3.) Sandbox Mode:");
                                
                
                Console.WriteLine(BuildSandboxDisplayString());
                
                Console.WriteLine("4.) Syntax Highlighting: "
                    + (OptionsManager.Get<bool>("V", "COLOR_SYNTAX_HIGHLIGHTING") ? "yes" : "no"));
                Console.WriteLine("5.) Reset to defaults");
                Console.WriteLine("6.) Back");

                Console.WriteLine("\r\nEnter a number between 1 - 6");
                string menu_input = ConEx.ConEx_Input.WaitForIntInRange(1,6,true).ToString();

                string pattern = "";
                string requirement_message = "";
                
                switch (menu_input)
                {
                    case "1":
                        pattern = "^(93|98)$";
                        requirement_message = "Must be 93 or 98";
                        break;
                    case "2":
                      //  pattern = "^[123]$";
                        requirement_message = "Must be 1, 2, or 3";
                        break;
                    case "3":
                        pattern = "[io= ]*";
                        requirement_message = "Choose any/all of i, or o, =. Press enter to revert to none";
                        break;
                    case "4":
                      //  pattern = "^(yes|no)$";
                        requirement_message = "Must be a yes or no";
                        break;
                    case "5":
                      //  pattern = "^(yes|no)$";
                        requirement_message = "Must be a yes or no";
                        break;
                    case "6":
                        this.OnClosing();
                        return;
                    default:
                        throw new Exception(menu_input + " is not a valid number!");
                }

                string value_input ="";
                
                Console.WriteLine(requirement_message);
                Console.WriteLine("Enter a value");
                    
                switch (menu_input)
                {
                    case "1":
                        value_input = ConEx.ConEx_Input.WaitForRegExMatch(pattern, requirement_message);
                        OptionsManager.Set<int>("I", "LF_SPEC_VERSION", Convert.ToInt32(value_input));
                        Console.WriteLine("Spec version is now: {0}", value_input);
                        break;
                    case "2":
                        int dimensions = ConEx.ConEx_Input.WaitForIntInRange(1, 3, true, requirement_message);
                        OptionsManager.Set<int>("I", "LF_DIMENSIONS", dimensions);
                        Console.WriteLine("Dimensions is now: {0}", dimensions);
                        break;
                    case "3":
                        value_input = ConEx.ConEx_Input.WaitForRegExMatch(pattern, requirement_message);
                        if (value_input.Contains("i"))
                        {
                            OptionsManager.Set<bool>("I", "LF_FILE_INPUT", false);
                        }
                        else
                        {
                            OptionsManager.Set<bool>("I","LF_FILE_INPUT", OptionsManager.DefaultOptions["Interpreter"]["LF_FILE_INPUT"].BoolValue);
                        }
                        if (value_input.Contains("o"))
                        {
                            OptionsManager.Set<bool>("I", "LF_FILE_OUTPUT", false);
                        }
                        else
                        {
                            OptionsManager.Set<bool>("I","LF_FILE_OUTPUT", OptionsManager.DefaultOptions["Interpreter"]["LF_FILE_OUTPUT"].BoolValue);
                        }
                        if (value_input.Contains("="))
                        {
                            OptionsManager.Set<int>("I", "LF_EXECUTE_STYLE", 0);
                        }
                        else
                        {
                            OptionsManager.Set<int>("I", "LF_EXECUTE_STYLE", OptionsManager.DefaultOptions["Interpreter"]["LF_EXECUTE_STYLE"].IntValue);
                        }
                        Console.WriteLine("The following items are sandboxed: {0}", BuildSandboxDisplayString());
                        break;
                    case "4":
                        if (ConEx.ConEx_Input.WaitForBooleanChoice() == true)
                        {
                            OptionsManager.Set<bool>("V", "COLOR_SYNTAX_HIGHLIGHTING", true);
                        }
                        else
                        {
                            OptionsManager.Set<bool>("V", "COLOR_SYNTAX_HIGHLIGHTING", false);
                        }
                        Console.WriteLine("Syntax Highlighting is {0}", OptionsManager.Get<bool>("V", "COLOR_SYNTAX_HIGHLIGHTING") ? "on" : "off" );
                        break;
                    case "5":
                        if (ConEx.ConEx_Input.WaitForBooleanChoice() == true)
                        {
                            Console.WriteLine("Are you sure you want to reset all options to their default values?");
                            
                            bool is_reseting = ConEx.ConEx_Input.WaitForBooleanChoice();
                            if (is_reseting == true)
                            {
                                OptionsManager.ResetSessionOptions();
                            }
                        }
                        break;
                }

                Console.WriteLine("Press any key to continue");
                Console.ReadKey(true);
            }
            while (true);

            this.OnClosing();
        }

        private string BuildSandboxDisplayString()
        {
            string sandbox_mode = "";
            sandbox_mode += OptionsManager.Get<bool>("I", "LF_FILE_INPUT") == false ? "i" : "";
            sandbox_mode += OptionsManager.Get<bool>("I", "LF_FILE_OUTPUT") == false ? "o" : "";
            sandbox_mode += OptionsManager.Get<int>("I", "LF_EXECUTE_STYLE") == 0 ? "=" : "";

            //If the sandbox mode hasn't been assaigned anything print "NONE"
            sandbox_mode = sandbox_mode == "" ? "none" : sandbox_mode;
            return sandbox_mode;
        }
    }
}
