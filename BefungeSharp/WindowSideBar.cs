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
		/// Pages that rely on the interpreter mode
		/// </summary>
		private enum Page
		{
			//Base modes relating to a run speed or edit mode
			Edit_NormalMode,
			Edit_SelectionMode,
			Run_RealtimeMode,
			Run_StepMode,
			Extra_ASCII_Table,
			LoadedFingerprints
		}

		/// <summary>
		/// The data pool of all the pages
		/// </summary>
		private Dictionary<Page, string[]> _all_pages;

		/// <summary>
		/// A collection of pairs of Page Type and content that can be sorted through
		/// </summary>
        private List<Tuple<Page, string[]>> _current_pages;

        private string[] CurrentPage { get { return _current_pages[PageIndex].Item2; } }

		/// <summary>
		/// The current page we're on
		/// </summary>
		private int PageIndex { get; set; }

		/// <summary>
		/// The blank template page
		/// </summary>
		private readonly string[] TEMPLATE_PAGE;
		private readonly int PAGE_HEADER_START; //0
		private readonly int PAGE_HEADER_END; //2
		private readonly int PAGE_ROW; //3
		private readonly int PAGE_END;

		public WindowSideBar()
        {
            BAR_RIGHT = ConEx.ConEx_Draw.Dimensions.width - 1;
            BAR_BOTTOM = ConEx.ConEx_Draw.Dimensions.height - 1;

            _all_pages = new Dictionary<Page, string[]>();

            TEMPLATE_PAGE = new string[ConEx.ConEx_Draw.Dimensions.height];

            {
                //Create a 36 row long blank template which can have portions overridden
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
				
				PAGE_END = i;
                TEMPLATE_PAGE[i++] = "╚══════════════════════════════════╝";
                TEMPLATE_PAGE[i++] = "";
                TEMPLATE_PAGE[i++] = "◄◄ Home           N/C         End ►►";
            }

            //The normal edit mode
            {
                _all_pages.Add(Page.Edit_NormalMode, new string[TEMPLATE_PAGE.Length]);
                TEMPLATE_PAGE.CopyTo(_all_pages[Page.Edit_NormalMode], 0);

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
							}.CopyTo(_all_pages[Page.Edit_NormalMode], 0);
            }

            //Edit mode when you have started a selection
            {
                _all_pages.Add(Page.Edit_SelectionMode, new string[TEMPLATE_PAGE.Length]);
                TEMPLATE_PAGE.CopyTo(_all_pages[Page.Edit_SelectionMode], 0);
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
				}.CopyTo(_all_pages[Page.Edit_SelectionMode], 0);
            }

            //Regular runtime mode
            {
                _all_pages.Add(Page.Run_RealtimeMode, new string[TEMPLATE_PAGE.Length]);
                TEMPLATE_PAGE.CopyTo(_all_pages[Page.Run_RealtimeMode], 0);

                new string[] {
						TEMPLATE_PAGE[PAGE_HEADER_START],
						"║        Realtime Shortcuts        ║",
						TEMPLATE_PAGE[PAGE_HEADER_END],
						"║Select Speed - F1 - F6            ║",
						"║Back to Edit Mode - F12           ║"
					}.CopyTo(_all_pages[Page.Run_RealtimeMode], 0);
            }

			//Step runtime mode
            {
                _all_pages.Add(Page.Run_StepMode, new string[TEMPLATE_PAGE.Length]);
                TEMPLATE_PAGE.CopyTo(_all_pages[Page.Run_StepMode], 0);

                new string[] {
					    TEMPLATE_PAGE[PAGE_HEADER_START],
						"║            Step Shortcuts        ║",
						TEMPLATE_PAGE[PAGE_HEADER_END],
						"║Select Speed - F1 - F6            ║",
                        "║Back to Edit Mode - F12           ║",
                        TEMPLATE_PAGE[PAGE_ROW],
                        "║Next Tick - Right Arrow           ║"
				}.CopyTo(_all_pages[Page.Run_StepMode], 0);
            }

            //The ASCII Table
            {
                _all_pages.Add(Page.Extra_ASCII_Table, new string[TEMPLATE_PAGE.Length]);
                TEMPLATE_PAGE.CopyTo(_all_pages[Page.Extra_ASCII_Table], 0);
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
			    }.CopyTo(_all_pages[Page.Extra_ASCII_Table], 0);
            }

			//Loaded Fingerprints Table
			{
				_all_pages.Add(Page.LoadedFingerprints, new string[TEMPLATE_PAGE.Length]);
				TEMPLATE_PAGE.CopyTo(_all_pages[Page.LoadedFingerprints], 0);

				new string[] {
						TEMPLATE_PAGE[PAGE_HEADER_START],
						"║        Loaded Fingerprints       ║",
						TEMPLATE_PAGE[PAGE_HEADER_END],
						"║                                  ║",
						"║                                  ║"
					}.CopyTo(_all_pages[Page.LoadedFingerprints], 0);
			}
            _current_pages = new List<Tuple<Page, string[]>>();
        }

		/// <summary>
		/// Draws the User interface of whatever mode the board is in
		/// </summary>
		/// <param name="mode">The mode of the board</param>
		public void Draw(BoardMode mode)
		{
            //Update the page index bar
			CurrentPage[TEMPLATE_PAGE.Length-1] = String.Format("◄◄ Home           {0}/{1}         End ►►", PageIndex + 1, _current_pages.Count());

			//Insert every string of the page into the draw buffer
			for (int i = 0; i < CurrentPage.Count(); i++)
			{
				ConEx.ConEx_Draw.InsertString(CurrentPage[i], BAR_TOP + i, BAR_LEFT, false);
			}

			if (CurrentPage == _all_pages[Page.Extra_ASCII_Table])
			{
				ColorizeASCII_Table();
			}
		}

		/// <summary>
		/// Colorize the instructions of the ASCII table to match
		/// </summary>
		private void ColorizeASCII_Table()
		{
            string[] ASCII_Table = _all_pages[Page.Extra_ASCII_Table];
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

		public string[] RefreshLoadedFingerprintsPage()
		{
			string[] loaded_fingerprints_page = new string[TEMPLATE_PAGE.Length];
			_all_pages[Page.LoadedFingerprints].CopyTo(loaded_fingerprints_page, 0);

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

				loaded_fingerprints_page[PAGE_HEADER_END + 1 + i] = fingerprint_members;
			}

			return loaded_fingerprints_page;
		}

		public void Update(BoardMode mode, IEnumerable<ConsoleKeyInfo> keysHit)
		{
			#region --HandleInput-------------
			for (int i = 0; i < keysHit.Count(); i++)
			{
				//--Debugging key presses
				System.ConsoleKey k = keysHit.ElementAt(i).Key;
				var m = keysHit.ElementAt(i).Modifiers;
				//------------------------

                switch (keysHit.ElementAt(i).Key)
                {
                    //Cycle through the pages
                    case ConsoleKey.Home:
                        PageIndex = PageIndex > 0 ? PageIndex -= 1 : PageIndex = _current_pages.Count - 1;
                        break;
                    case ConsoleKey.End:
                        PageIndex = PageIndex < _current_pages.Count - 1 ? PageIndex += 1 : PageIndex = 0;
                        break;
                }
			}
			#endregion

            _current_pages.Clear();
			if (mode == BoardMode.Edit)
			{

			}
			else
			{
				if (mode == BoardMode.Run_STEP)
				{
					_current_pages.Add(new Tuple<Page, string[]>(Page.Run_StepMode, _all_pages[Page.Run_StepMode]));
				}
				else
				{
					_current_pages.Add(new Tuple<Page, string[]>(Page.Run_RealtimeMode, _all_pages[Page.Run_RealtimeMode]));
				}

				_all_pages[Page.LoadedFingerprints] = RefreshLoadedFingerprintsPage();
				_current_pages.Add(new Tuple<Page, string[]>(Page.LoadedFingerprints, _all_pages[Page.LoadedFingerprints]));
			}
			_current_pages.Add(new Tuple<Page, string[]>(Page.Extra_ASCII_Table, _all_pages[Page.Extra_ASCII_Table]));

			switch (mode)
			{
				case BoardMode.Run_MAX:
				case BoardMode.Run_FAST:
				case BoardMode.Run_MEDIUM:
				case BoardMode.Run_SLOW:

					break;
				case BoardMode.Run_STEP:

					break;
				case BoardMode.Edit:
					if (Program.WindowUI.SelectionActive == true)
					{
						_current_pages.Add(new Tuple<Page, string[]>(Page.Edit_SelectionMode, _all_pages[Page.Edit_SelectionMode]));
					}
					else
					{
						_current_pages.Add(new Tuple<Page, string[]>(Page.Edit_NormalMode, _all_pages[Page.Edit_NormalMode]));
					}
					break;
			}
		}
	}
}
