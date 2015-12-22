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

                Console.WriteLine("2.) Number of Dimensions: ");
                
                //Build up the sandbox mode to display
                Console.Write("3.) Sandbox Mode:");
                                
                string sandbox_mode = "";
                sandbox_mode += OptionsManager.Get<bool>("I", "LF_FILE_INPUT")  == false ? "i" : "";
                sandbox_mode += OptionsManager.Get<bool>("I", "LF_FILE_OUTPUT") == false ? "o" : "";
                sandbox_mode += OptionsManager.Get<int> ("I", "LF_EXECUTE_STYLE") == 0   ? "=" : "";
                
                //If the sandbox mode hasn't been assaigned anything print "NONE"
                sandbox_mode = sandbox_mode == "" ? "NONE" : "";
                Console.WriteLine(sandbox_mode);
                
                Console.WriteLine("4.) Syntax Highlighting: "
                    + (OptionsManager.Get<bool>("V", "COLOR_SYNTAX_HIGHLIGHTING") ? "ON" : "OFF"));
                Console.WriteLine("5.) Reset to defaults");
                Console.WriteLine("6.) Back");

                Console.WriteLine("\r\nEnter a number between 1 - 6");
                string menu_input = Menu.WaitForInput('1','6').ToString();
                
                string pattern;
                string requirement_message;
                
                switch (menu_input)
                {
                    case "1":
                        pattern = "(93|98)";
                        requirement_message = "Must be 93 or 98";
                        break;
                    case "2":
                        pattern = "[123]";
                        requirement_message = "Must be 1,2, or 3";
                        break;
                    case "3":
                        pattern = "[io= ]*";
                        requirement_message = "Must be a combination of i,o,= or a space";
                        break;
                    case "4":
                        pattern = "(on|off)";
                        requirement_message = "Must be a on or off";
                        break;
                    case "5":
                        pattern = "y";//Look for yes, anything else is a no
                        requirement_message = "Must be yes or no";
                        break;
                    case "6":
                        this.OnClosing();
                        return;
                    default:
                        throw new Exception(menu_input + " is not a valid number!");
                }

                string value_input ="";
                while (true)
                {
                    Console.WriteLine(requirement_message);
                    Console.WriteLine("Enter a value");
                    value_input = Console.ReadLine();
                    if (Regex.IsMatch(value_input, pattern) == true || value_input.ToLower() != "back")
                    {
                        switch (menu_input)
                        {
                            case "1":
                                OptionsManager.Set<int>("I", "LF_SPEC_VERSION", Convert.ToInt32(value_input));
                                break;
                            case "2":
                                OptionsManager.Set<int>("I", "LF_DIMENSIONS", Convert.ToInt32(value_input));
                                break;
                            case "3":
                                if (value_input.Contains("i"))
                                {
                                    OptionsManager.Set<bool>("I", "LF_FILE_INPUT", false);
                                }
                                else
                                {
                                    OptionsManager.DefaultOptions["Interpreter"]["LF_FILE_INPUT"] = OptionsManager.DefaultOptions["Interpreter"]["LF_FILE_INPUT"];
                                }
                                if (value_input.Contains("o"))
                                {
                                    OptionsManager.Set<bool>("I", "LF_FILE_OUTPUT", false);
                                }
                                else
                                {
                                    OptionsManager.SessionOptions["Interpreter"]["LF_FILE_OUTPUT"] = OptionsManager.DefaultOptions["Interpreter"]["LF_FILE_OUTPUT"];
                                }
                                if (value_input.Contains("="))
                                {
                                    OptionsManager.Set<int>("I", "LF_EXECUTE_STYLE", 0);
                                }
                                else
                                {
                                    OptionsManager.Set<int>("I", "LF_EXECUTE_STYLE", OptionsManager.DefaultOptions["Interpreter"]["LF_EXECUTE_STYLE"].IntValue);
                                }
                                break;
                            case "4":
                                OptionsManager.Set<bool>("V", "COLOR_SYNTAX_HIGHLIGHTING", !OptionsManager.Get<bool>("V", "COLOR_SYNTAX_HIGHLIGHTING"));
                                break;
                            case "5":
                                if (value_input.ToLower() == "y")
                                {
                                    Console.WriteLine("Are you sure you want to reset all options to their default values?");
                                    bool is_reseting = Menu.WaitForBooleanChoice();
                                    if (is_reseting == true)
                                    {
                                        OptionsManager.ResetSessionOptions();
                                    }
                                }
                                break;
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please try again");
                    }
                }
            }
            while (true);

            this.OnClosing();
        }        
    }
}
