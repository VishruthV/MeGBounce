using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeGBounce
{
    class ContractsManager
    {
        internal static List<MeGSymbol> GetContractsFromSymbols()
        {
            List<string> symbols = ReadSymbolsFile();
            List<MeGSymbol> ret = new List<MeGSymbol>();

            foreach (string sym in symbols)
            {
                string[] split = sym.Split(',');

                Krs.Ats.IBNet.Contract c = new Krs.Ats.IBNet.Contract();
                c.Symbol = split[0];
                c.Exchange = Parameters.Exchange;
                c.SecurityType = Parameters.SecurityType;
                c.Currency = Parameters.Currency;
                //c.Expiry = Parameters.ExpiryDate;

                int lotSize = int.Parse(split[1]);


                ret.Add(new MeGSymbol { Contract=c,LotSize = lotSize});
            }

            return ret;
        }

        private static List<string> ReadSymbolsFile()
        {
            string filename = Parameters.SymbolUniverseFile;
            //Symbols are assumed to be newline seperated.
            string[] sym = File.ReadAllLines(filename);
            List<string> ret = sym.ToList();
            ret.RemoveAt(0);
            return ret; //Remove 0th item (first line), as it is the header line
        }
    }
}
