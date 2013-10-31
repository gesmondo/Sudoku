using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ges_Sudoku
{

    public class Program
    {
        
        // init sub-squares
        private static int[] Square1 = { 0, 1, 2, 9, 10, 11, 18, 19, 20 };
        private static int[] Square2 = { 3, 4, 5, 12, 13, 14, 21, 22, 23 };
        private static int[] Square3 = { 6, 7, 8, 15, 16, 17, 24, 25, 26 };
        private static int[] Square4 = { 27, 28, 29, 36, 37, 38, 45, 46, 47 };
        private static int[] Square5 = { 30, 31, 32, 39, 40, 41, 48, 49, 50 };
        private static int[] Square6 = { 33, 34, 35, 42, 43, 44, 51, 52, 53 };
        private static int[] Square7 = { 54, 55, 56, 63, 64, 65, 72, 73, 74 };
        private static int[] Square8 = { 57, 58, 59, 66, 67, 68, 75, 76, 77 };
        private static int[] Square9 = { 60, 61, 62, 69, 70, 71, 78, 79, 80 };
        
        
        

        public static void Main()
        {

            //File with sudoku
            string FileContent = strReadFile("c:\\data\\code\\s1.txt");
            Debug.WriteLine("Open and read file");


            int solvedSudoku = 0;
            int forCount = 0;

            int iReturn = 0;
            //Create dictionary object that will hold sudoku
            Dictionary<string, string> openWith =
            new Dictionary<string, string>();
            // Create dictionary object that will hold zero list
            //Dictionary<int, int> dZList = new Dictionary <int, int>();
            var dZList = new List<int>();
            //int[] dZlist;
            
            //Create Tuple object that will hold information about previous moves
            Dictionary<int, int> dOldValue = new Dictionary<int, int>();

            Debug.WriteLine("Calling Module GetSuDoku");
            Dictionary<string, string> dSudoku = GetSudoku(openWith, FileContent);

            //Print Sudoku to solve in readable manner
            strWriteFile(dSudoku, "c:\\data\\code\\quiz.txt");
            
            //A value that will count the numbers of recursive calls
            int RecursiveCount = 0;
            
            //Begin with fetching information about which square to begin with
            Debug.WriteLine("Calling Module BuildZeroList");
            iReturn = BuildZeroList(ref dZList, dSudoku);
            forCount = dZList.Count();
            //Check if Sudpku has any empty spots
            if (forCount != 0)
            {            
                for (int i = 0; i <= forCount; i++)
                {
                    //ref Dictionary<int, int> dOldValue
                    Debug.WriteLine("Calling Module RecursiveSetSudokuValue with Zero element: " + dZList[i]);
                    solvedSudoku = RecursiveSetSudokuValue(ref dOldValue, ref dSudoku, dZList[i], ref RecursiveCount, 1);
                    //If a solution is found then break
                    if (solvedSudoku == 1)
                    {
                        break;
                    }

                    //Console.ReadLine();
                }
            }

            Console.WriteLine("Solution found!");
            Console.ReadLine();
            strWriteFile(dSudoku, "c:\\data\\code\\solution.txt");
        }


        public static Dictionary<string, string> GetSudoku(Dictionary<string,string> dictionary, string FileCont)
        {
            // init the in-loop counter
            int j = 0;
            // Add some elements to the dictionary. There are no  
            // duplicate keys, but some of the values are duplicates.
            for (int i = 0; i < FileCont.Length; i++)
            {
                
                if (char.IsDigit(FileCont[i]))
                {
                    dictionary.Add(j.ToString(), FileCont[i].ToString());
                    j++;
                }
            } 
            
            return dictionary;
         }


        public static string strReadFile(string path)
        {
            StreamReader myReader = new StreamReader(path);
            string line = "";
            string addLine = "";

            while (line != null)
            {
                line = myReader.ReadLine();
                if (line != null)
                {
                    addLine += line;
                }
            }

            myReader.Close();
            return addLine;
        }

        //Write Sudoku to file
        private static void strWriteFile(Dictionary <string, string> sudoku, string filename)
        {
            StreamWriter myWrite = new StreamWriter(filename);
            int newLine = 0;
            string strSoduku = "";

            foreach (KeyValuePair <string, string> value in sudoku)
            {
                newLine++;
                strSoduku += value.Value + " ";
                if (newLine % 9 == 0)
                {
                    myWrite.WriteLine(strSoduku);
                    strSoduku = "";
                }
            }
            
            myWrite.Close();
        }

        private static int RecursiveSetSudokuValue(ref Dictionary<int, int> dOldValue, 
            ref Dictionary<string, string> dict, int pos, ref int RecCheck, int Value)
        {
            bool CheckCol = true;
            bool CheckRow = true;
            bool CheckSq = true;
            bool ValueAdded = false;
            int OldKey;
            int OldValue;
            int iRes = 0;

            // Create List object that will hold zero list
            List<int> dZList = new List<int>();

            RecCheck ++;
            Debug.WriteLine("Inside RecursiveSetSudokuValue for the: " + RecCheck + " time");
            
            for (int i = Value; i <= 9; i++)
            {
                //Check unique value for Column
                CheckCol = GetColumn(dict, pos, i.ToString());
                //Check unique value for Row               
                CheckRow = GetRow(dict, pos, i.ToString());
                //Check unique value for Square
                CheckSq = GetSquare(dict, pos, i.ToString());
                
                    if ((CheckCol && CheckRow && CheckSq) == true)
                    {
                        Debug.WriteLine("Found a value to set; position: " + pos + " Value: " + i.ToString());
                        //Put values in the dictionary object that holds old values
                        dOldValue.Add(pos, i);
                        //Add new value to the given position on the Sudoku                         
                        dict [pos.ToString()]= i.ToString();
                        ValueAdded = true;
                        //Take the next square from the zero array only if not backtracked
                        iRes = BuildZeroList(ref dZList, dict);
                        //Search for end condition
                        if (dZList.Count != 0)
                        {
                            Debug.WriteLine("Still there exists zeros: " + dZList.Count);
                            Debug.WriteLine(" ");
                            //call recursive function, set value to 1 since it's time to start from the beginning on the new position
                            return RecursiveSetSudokuValue(ref dOldValue, ref dict, dZList.First(), ref RecCheck, 1);
                        }
                        else
                        {
                            Debug.WriteLine("Found a solution!");
                            return 1;
                        }
                    }
                }
            
            // In this case we have a problem; we couldn't find a number to add
            //So we have to backtrack using our list of previous added numbers
            while (true)
            {
                
                if (ValueAdded == false)
                {

                    //OldKey = dOldValue.ElementAt(0).Key;
                    OldKey = dOldValue.Last().Key;
                    //Set back to zero
                    OldValue = dOldValue.Last().Value;
                    //Check that oldvalue is not greater than nine
                    if ((OldValue + 1) != 10)
                    {
                        //Add a zero to the old key in the Sudoku
                        dict[OldKey.ToString()] = 0.ToString();
                        //Remove the key, value numbers from the list of previous added numbers
                        dOldValue.Remove(dOldValue.Last().Key);
                        //Check if there still exists old values to backtrack
                        if (dOldValue.Count != 0)
                        {
                            //Recursive call to SetSudokuValue
                            Debug.WriteLine("Didn't find any values to set in position " + pos);
                            Debug.WriteLine("Calling RecursiveSetSudokuValue with position: " + OldKey + " and value: " + OldValue);
                            Debug.WriteLine(" ");
                            return RecursiveSetSudokuValue(ref dOldValue, ref dict, OldKey, ref RecCheck, OldValue + 1);
                        }
                        return 0;
                    }
                    //We have reach a nine, we need to backtrack once more
                    else
                    {
                        //Add a zero to the Zudoku
                        dict[OldKey.ToString()] = 0.ToString();
                        //Remove the key, value numbers from the list of previous added numbers
                        dOldValue.Remove(dOldValue.Last().Key);
                    }
                }
               
            }
        }

        
        // Create a list that holds information abouts which squares that holds most "information" a.k.a. fewest zeros.
        private static int BuildZeroList(ref List<int> lista,  Dictionary<string, string> dict)
        {


            Dictionary<int, int> zeroDict = new Dictionary<int, int>();
            int j = 0;

            for (int i = 0; i < 81; i++)
            {
                // Only fetch positions that have 0:s
                if (dict.ElementAt(i).Value == "0")
                {
                    int zeroCount = 0;
                    zeroCount = GetZeroRow(dict, i);
                    zeroCount += GetZeroColumn(dict, i);
                    zeroCount += GetZeroSquare(dict, i);
                    zeroDict.Add(i, zeroCount);
                }
              
            }
            
            //sorting function
            //sort the position that have the least zeros
            zeroDict = zeroDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (KeyValuePair<int, int> count in zeroDict)
            {
                j++;
                lista.Add(count.Key);
            }

            return 1;
        }

        private static int GetZeroColumn(Dictionary<string, string> dict, int pos)
        {

            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>();

            int iCount = 0;
            int tenth = pos % 9;
            //j will hold the index for the column 
            int j = 0;

            for (int i = 0; i < 9; i++)
            {
                kvp = dict.ElementAt(tenth + j);
                printKeyPair(kvp);
                if (kvp.Value == "0")
                    iCount ++;
                //iterate to the next index in the column
                j = j + 9;
            }
            return iCount;
        }    
        
        private static bool GetColumn (Dictionary<string,string> dict, int pos, string value)
        {
            
            KeyValuePair<string, string> kvp = new KeyValuePair<string,string>();

            int tenth = pos % 9;
            //j will hold the index for the column 
            int j = 0;

            for (int i = 0; i < 9; i++)
            {
                kvp = dict.ElementAt(tenth + j);
                printKeyPair(kvp);
                if (kvp.Value == value)
                    return false;
                //iterate to the next index in the column
                j = j + 9;
            }
            return true;
        }
        
        private static bool GetRow(Dictionary<string, string> dict, int pos, string value)
        {

            int i = 0;
            int tenth = pos / 9;

            foreach (KeyValuePair<string, string> kvpNew in dict)
            {
                if (i / 9 == tenth)
                {
                    printKeyPair(kvpNew);
                    if (kvpNew.Value == value)
                        return false;
                }
                i++;
            }
            return true;
        }

        private static int GetZeroRow(Dictionary<string, string> dict, int pos)
        {

            int i = 0;
            int tenth = pos / 9;
            int iCount = 0;

            foreach (KeyValuePair<string, string> kvpNew in dict)
            {
                if (i / 9 == tenth)
                {
                    printKeyPair(kvpNew);
                    if (kvpNew.Value == "0")
                        iCount++;
                }
                i++;
            }
            return iCount;
        }

        private static bool GetSquare(Dictionary<string, string> dict, int pos, string value)
        {

            
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(); 
            
            if (Square1.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square1[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;
                    
                }

                return true;
            }

            if (Square2.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square2[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;

                }

                return true;
            }

            if (Square3.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square3[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;

                }

                return true;
            }
            
            if (Square4.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square4[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;

                }

                return true;
            }

            if (Square5.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square5[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;

                }

                return true;
            }

            if (Square6.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square6[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;

                }

                return true;
            }

            if (Square7.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square7[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;

                }

                return true;
            }

            if (Square8.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square8[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;

                }

                return true;
            }

            if (Square9.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square9[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == value)
                        return false;

                }

                return true;
            }
            return true;
        }


        //Check how many zeros that exists in a speciffic square
        private static int GetZeroSquare(Dictionary<string, string> dict, int pos)
        {

            int iCount = 0;
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>();

            if (Square1.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square1[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }

            else if (Square2.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square2[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }

            else if (Square3.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square3[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }

            else if (Square4.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square4[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }

            else if (Square5.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square5[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }

            else if (Square6.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square6[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }

            else if (Square7.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square7[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }

            else if (Square8.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square8[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }

            else if (Square9.Contains(pos))
            {
                for (int i = 0; i < 9; i++)
                {
                    kvp = dict.ElementAt(Square9[i]);
                    printKeyPair(kvp);
                    if (kvp.Value == "0")
                        iCount++;
                }
            }
            return iCount;
        }


        private static void printKeyPair (KeyValuePair <string, string> pair)
        {
            Console.WriteLine("Key = {0}, Value = {1}", pair.Key, pair.Value);
        }
    }
}
