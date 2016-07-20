using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConEx;

namespace BefungeSharp
{
	public class WindowSideBar
	{
		/* The side bar extends from the right side of the board to a location W
		 * | [0,81]                 [0,C_Width]
		 * |
		 * |
		 * |
		 * |
		 * | [C_Height,81]                 [C_Height,C_Width]
		 * */
		private const int BAR_TOP = 0;
		private const int BAR_LEFT = 81;
		private int BAR_RIGHT;
		private int BAR_BOTTOM;

		/// <summary>
		/// Pages of sidebar
		/// </summary>
		private enum Page : int
		{
			PageEnumStart, //DO NOT USE
			//Base modes relating to a run speed or edit mode
			Edit_NormalMode,
			Edit_SelectionMode,
			Run_RealtimeMode,
			Run_StepMode,
			Extra_ASCII_Table,
			Loaded_Fingerprints,
			PageEnumEnd //DO NOT USE
		}

		/// <summary>
		/// All known pages, where the key is the page enum and the value is the page content.
		/// Some pages have static content (like the ASCII table), some have dynamic content
		/// </summary>
		private Dictionary<Page,string[]> AllPages { get; set; }
		
		/// <summary>
		/// A list of all enabled pages that can be viewed
		/// </summary>
		private List<Page> EnabledPages { get; set; }

		/// <summary>
		/// The 0 based index for which enabled page we are on
		/// </summary>
		private int PageIndex { get; set; }

		/// <summary>
		/// The current page's content
		/// </summary>
		private string[] CurrentPage {
										get
										{
											//If we have switched modes from one with many pages to one
											//with less, change the PageIndex to the last one
											if (PageIndex > EnabledPages.Count() - 1)
											{
												PageIndex = EnabledPages.Count() - 1;
											}
											return AllPages[EnabledPages[PageIndex]];
										}
									 }

		/// <summary>
		/// The blank template page
		/// </summary>
		private readonly string[] TEMPLATE_PAGE;

		/// <summary>
		/// Where the top of the page's frame's header starts
		/// </summary>
		private readonly int PAGE_HEADER_START; //0
		
		/// <summary>
		/// Where the top of the page's frame's header ends
		/// </summary>
		private readonly int PAGE_HEADER_END; //2
		
		/// <summary>
		/// The location of the first blank row in the page
		/// </summary>
		private readonly int PAGE_ROW; //3

		/// <summary>
		/// Where the page's frame ends
		/// </summary>
		private readonly int PAGE_END; //30

		/// <summary>
		/// Adds a page to the sidebar's list of known pages
		/// </summary>
		/// <param name="page">The page enum it represents</param>
		/// <param name="content">The content of said page</param>
		private void AddPage(Page page, string[] content)
		{
			AllPages.Add(page, new string[TEMPLATE_PAGE.Length]);
			TEMPLATE_PAGE.CopyTo(AllPages[page], 0);
			content.CopyTo(AllPages[page], 0);
		}

		/// <summary>
		/// Cycles to the next enabled page
		/// </summary>
		/// <returns>The new current page</returns>
		private Page NextPage()
		{
			//PageIndex >= catches if the previous mode we were in had more pages than the current one
			if (PageIndex + 1 >= EnabledPages.Count())
			{
				PageIndex = 0;
			}
			else
			{
				++PageIndex;
			}
		
			return EnabledPages[PageIndex];
		}

		/// <summary>
		/// Cycles to the previous enabled page
		/// </summary>
		/// <returns>The new current page</returns>
		private Page PreviousPage()
		{
			//PageIndex >= catches if the previous mode we were in had more pages than the current one
			if (PageIndex - 1 < 0 || PageIndex - 1 >= EnabledPages.Count())
			{
				PageIndex = EnabledPages.Count() - 1;
			}
			else
			{
				--PageIndex;
			}

			return EnabledPages[PageIndex];
		}

		public WindowSideBar()
        {
            BAR_RIGHT = ConEx.ConEx_Draw.Dimensions.width - 1;
            BAR_BOTTOM = ConEx.ConEx_Draw.Dimensions.height - 1;

			AllPages = new Dictionary<Page, string[]>();
			EnabledPages = new List<Page>();
			PageIndex = 0;

            TEMPLATE_PAGE = new string[ConEx.ConEx_Draw.Dimensions.height];
            {
                //Create a 36 row long blank template which can have portions overridden
				//Remember the difference between ++i and i++

                int i = 0;
				PAGE_HEADER_START = i;
                TEMPLATE_PAGE[i++] = "╔══════════════════════════════════╗";
                TEMPLATE_PAGE[i++] = "║                                  ║";
                TEMPLATE_PAGE[i++] = "╠══════════════════════════════════╣";
				PAGE_HEADER_END = i-1;
				PAGE_ROW = i;
                for (i = 3; i < TEMPLATE_PAGE.Length - 3; i++)
                {
                    TEMPLATE_PAGE[i] = "║                                  ║";
                }
				
                TEMPLATE_PAGE[i++] = "╚══════════════════════════════════╝";
				PAGE_END = i;
				TEMPLATE_PAGE[i++] = "                                    ";
                TEMPLATE_PAGE[i++] = "◄◄ Home           N/C         End ►►";
            }

            //The normal edit mode
            {
                AddPage(Page.Edit_NormalMode,
					//The regular keyboard shortcuts
					new string[]{
								TEMPLATE_PAGE[PAGE_HEADER_START],
								"║            Edit Shortcuts        ║",
								TEMPLATE_PAGE[PAGE_HEADER_END],
								"║New File - Ctrl + N               ║",
								"║Save - Alt + S                    ║",
								"║Reload Source - Alt + R           ║",
								TEMPLATE_PAGE[PAGE_ROW],
								"║Run (Step) - F1                   ║",
								"║Run (Real Time) - F2 - F5         ║",
								"║Run (Terminal Mode) - F6          ║",
								TEMPLATE_PAGE[PAGE_ROW],
								"║Main Menu - Esc                   ║",
								TEMPLATE_PAGE[PAGE_ROW],
								"║Insert Snippet - Insert           ║",
								"║Set IP Delta - Ctrl + Arrow Key   ║",
								"║Select - Shift (hold) + Arrow Key ║",
								"║Move Viewport - Tab + Arrow Key   ║"
								//"║XSelect All - Ctrl + A           ║",
								});
            }

            //Edit mode when you have started a selection
            {
				AddPage(Page.Edit_SelectionMode,
						new string[] {
										TEMPLATE_PAGE[PAGE_HEADER_START],
										"║       Selection Shortcuts        ║",
										TEMPLATE_PAGE[PAGE_HEADER_END],
										"║Adjust Selection Box - Arrow Keys ║",
										"║Copy Section - Ctrl + C           ║",
										"║Cut Section - Ctrl + X            ║",
										"║Paste Section - Ctrl + V          ║",
										"║Clear area - Delete               ║",
										//"║XReverse line - Alt + R          ║",
										"║Cancel Selection - Any Other Key  ║"
						});
            }

            //Regular runtime mode
            {
				AddPage(Page.Run_RealtimeMode,
						new string[] {
								TEMPLATE_PAGE[PAGE_HEADER_START],
								"║        Realtime Shortcuts        ║",
								TEMPLATE_PAGE[PAGE_HEADER_END],
								"║Select Speed - F1 - F6            ║",
								"║Back to Edit Mode - F12           ║"
						});
            }

			//Step runtime mode
            {
				AddPage(Page.Run_StepMode,
				new string[] {
					    TEMPLATE_PAGE[PAGE_HEADER_START],
						"║            Step Shortcuts        ║",
						TEMPLATE_PAGE[PAGE_HEADER_END],
						"║Select Speed - F1 - F6            ║",
                        "║Back to Edit Mode - F12           ║",
                        TEMPLATE_PAGE[PAGE_ROW],
                        "║Next Tick - Right Arrow           ║"
				});
            }

            //The ASCII Table
            {
				AddPage(Page.Extra_ASCII_Table,
						new string[] {
							"╔══════════════════════════════════╗",
							"║           ASCII Table            ║",
							"╠═══════╦═══════╦════════╦═════════╣",
							"║ 32    ║ 58  : ║ 82   T ║ 111  o  ║",
							"║ 33  ! ║ 59  ; ║ 83   U ║ 112  p  ║",
							"║ 34  \" ║ 60  < ║ 84   V ║ 113  q  ║",
							"║ 35  # ║ 61  = ║ 88   W ║ 114  r  ║",
							"║ 36  $ ║ 62  > ║ 89   X ║ 115  s  ║",
							"║ 37  % ║ 63  ? ║ 90   Y ║ 116  t  ║",
							"║ 38  & ║ 64  @ ║ 91   Z ║ 117  u  ║",
							"║ 39  ' ║ 65  A ║ 92   [ ║ 118  v  ║",
							"║ 40  ( ║ 66  B ║ 93   \\ ║ 119  w  ║",
							"║ 41  ) ║ 67  C ║ 94   ] ║ 120  x  ║",
							"║ 42  * ║ 68  D ║ 95   _ ║ 121  y  ║",
							"║ 43  + ║ 69  E ║ 96   ` ║ 122  z  ║",
							"║ 44  , ║ 70  F ║ 97   a ║ 123  {  ║",
							"║ 45  - ║ 71  G ║ 98   b ║ 124  |  ║",
							"║ 46  . ║ 72  H ║ 99   c ║ 125  }  ║",
							"║ 47  / ║ 73  I ║ 100  d ║ 126  ~  ║",
							"║ 48  0 ║ 74  J ║ 101  e ║         ║",
							"║ 49  1 ║ 75  K ║ 102  f ║         ║",
							"║ 50  2 ║ 76  L ║ 103  g ║         ║",
							"║ 51  3 ║ 77  M ║ 104  h ║         ║",
							"║ 52  4 ║ 78  N ║ 105  i ║         ║",
							"║ 53  5 ║ 79  O ║ 106  j ║         ║",
							"║ 54  6 ║ 80  P ║ 107  k ║         ║",
							"║ 55  7 ║ 81  Q ║ 108  l ║         ║",
							"║ 56  8 ║ 82  R ║ 109  m ║         ║",
							"║ 57  9 ║ 83  S ║ 110  n ║         ║",
							"╚═══════╩═══════╩════════╩═════════╝"
						});
            }

			//Loaded Fingerprints table, dynamically changed during runtime
			{
				AddPage(Page.Loaded_Fingerprints,
				new string[] {
						TEMPLATE_PAGE[PAGE_HEADER_START],
						"║        Loaded Fingerprints       ║",
						TEMPLATE_PAGE[PAGE_HEADER_END],
						"║                                  ║",
						"║                                  ║"
					});
			}
        }

		/// <summary>
		/// Draws the User interface of whatever mode the board is in
		/// </summary>
		/// <param name="mode">The mode of the board</param>
		public void Draw(BoardMode mode)
		{
            //Update the page index bar
			CurrentPage[TEMPLATE_PAGE.Length-1] = String.Format("◄◄ Home           {0}/{1}         End ►►", PageIndex + 1, EnabledPages.Count());

			//Insert every string of the page into the draw buffer
			for (int i = 0; i < CurrentPage.Count(); i++)
			{
				ConEx.ConEx_Draw.InsertString(CurrentPage[i], BAR_TOP + i, BAR_LEFT, false);
			}

			if (EnabledPages[PageIndex] == Page.Extra_ASCII_Table)
			{
				ColorizeASCII_Table();
			}
		}

		private void ApplyTemplateMask(string[] page_content)
		{
			for (int row = 0; row <= PAGE_END; ++row)
			{
				char[] page_row = page_content[row].ToCharArray();
				for (int col = 0; col < TEMPLATE_PAGE.Count() - 1; ++col)
				{
					if (TEMPLATE_PAGE[row][col] != ' ')
					{
						page_row[col] = TEMPLATE_PAGE[row][col];
					}
				}

				page_content[row] = new String(page_row);
			}
		}
		
		/// <summary>
		/// Colorize the instructions of the ASCII table to match
		/// </summary>
		private void ColorizeASCII_Table()
		{
            string[] ASCII_Table = AllPages[Page.Extra_ASCII_Table];
			//For the whole ASCII table diagram (skipping the first 3 lines)
			//and excluding the last 2
			for(int row = 3; row < ASCII_Table.Length - 2; row++)
			{
				for(int column = 0; column < ASCII_Table[row].Length; column++)
				{
					char c = ASCII_Table[row][column];

					//Only colorize the right ASCII characters (excluding space)
					if (c > ' ' && c <= '~')
					{
						//Ignore all numbers except if the row and column are right
						//6 comes from the hardcoded ASCII art table
						if ((c < '0' || c > '9') || (column == 6))
						{
							ConEx_Draw.InsertCharacter(c,
													   row,
													   column + BAR_LEFT,
													   Instructions.InstructionManager.InstructionSet[c].Color);
						}
					}
				}
			}
		}

        /// <summary>
        /// Clears the whole side bar area
        /// </summary>
        /// <param name="mode">Unused</param>
		public void ClearArea(BoardMode mode)
		{
			ConEx.ConEx_Draw.FillArea(' ', BAR_TOP, BAR_LEFT, ConEx.ConEx_Draw.Dimensions.width, ConEx.ConEx_Draw.Dimensions.height);
		}

		/// <summary>
		/// Refreshes the loaded fingerprints page with any new information
		/// </summary>
		public void RefreshLoadedFingerprintsPage()
		{
			string[] loaded_fingerprints_page = new string[TEMPLATE_PAGE.Length];
			IP ip =  Program.Interpreter.IPs.First();

			for (int i = ip.LoadedFingerprints.Count() - 1; i >= 0 && i < PAGE_END; --i)
			{
				string short_name = ip.LoadedFingerprints.ElementAt(i).ShortName;
				string members = new string(ip.LoadedFingerprints.ElementAt(i)
											.Members
											.Select(s => s.Value == null ? ' ' : s.Key)
											.ToArray<char>());

				string fingerprint_members = (TEMPLATE_PAGE[PAGE_ROW][0] + " " + short_name + ":" + members)
											 .PadRight(TEMPLATE_PAGE[0].Count() - 1, ' ')
											 + TEMPLATE_PAGE[PAGE_ROW][TEMPLATE_PAGE[PAGE_ROW].Count()-1];

				AllPages[Page.Loaded_Fingerprints][PAGE_HEADER_END + 1 + i] = fingerprint_members;
			}
			
			ApplyTemplateMask(AllPages[Page.Loaded_Fingerprints]);
		}

		public void Update(BoardMode mode, IEnumerable<ConsoleKeyInfo> keysHit)
		{
			//Based on the mode choose which pages are enabled
			//- Edit Mode
			//  1. Keyboard or Selection shortcuts
			//  2. ASCII page
			//- Run Mode
			//  1. Step mode or run mode keyboard shortcuts
			//  2. ASCII page
			//  3. Current IP's loaded fingerprints

			EnabledPages.Clear();
			
			switch (mode)
			{
				case BoardMode.Run_TERMINAL:
					break;//No sidebar shown
				case BoardMode.Run_MAX:
				case BoardMode.Run_FAST:
				case BoardMode.Run_MEDIUM:
				case BoardMode.Run_SLOW:
					EnabledPages.Add(Page.Run_RealtimeMode);
					EnabledPages.Add(Page.Extra_ASCII_Table);

					RefreshLoadedFingerprintsPage();
					EnabledPages.Add(Page.Loaded_Fingerprints);
					break;
				case BoardMode.Run_STEP:
					EnabledPages.Add(Page.Run_StepMode);
					EnabledPages.Add(Page.Extra_ASCII_Table);
					
					RefreshLoadedFingerprintsPage();
					EnabledPages.Add(Page.Loaded_Fingerprints);
					break;
				case BoardMode.Edit:
					if (Program.WindowUI.SelectionActive == true)
					{
						EnabledPages.Add(Page.Edit_SelectionMode);
					}
					else
					{
						EnabledPages.Add(Page.Edit_NormalMode);
					}

					EnabledPages.Add(Page.Extra_ASCII_Table);
					break;
			}

			#region --HandleInput-------------
			for (int i = 0; i < keysHit.Count(); i++)
			{
				//--Debugging key presses
				//System.ConsoleKey k = keysHit.ElementAt(i).Key;
				//var m = keysHit.ElementAt(i).Modifiers;
				//------------------------

				switch (keysHit.ElementAt(i).Key)
				{
					//Cycle through the pages
					case ConsoleKey.Home:
						PreviousPage();
						break;
					case ConsoleKey.End:
						NextPage();
						break;
				}
			}
			#endregion
		}
	}
}
