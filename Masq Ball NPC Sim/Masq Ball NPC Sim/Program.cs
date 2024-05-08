using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Masq_Ball_NPC_Sim
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string funcSelect;
            int[] triggerMap = new int[96];

            List<NPCData> npcList = new List<NPCData>();
            npcList = DataLoadIn(npcList);

            while (true)
            {
                Console.WriteLine("Available functions: cell lookup, save, exit, cell change, member lookup, member list, room lookup, next period, load backup, private trigger, public trigger, display trigger.");
                Console.Write("Select an action to perform: ");
                funcSelect = Console.ReadLine();
                switch (funcSelect)
                {
                    case "cell lookup":
                        CellLookup(npcList);
                        break;
                    case "save":
                        CSVSave(npcList);
                        break;
                    case "exit":
                        CSVSave(npcList);
                        goto EndOfMain;
                    case "cell change":
                        npcList = CellChange(npcList);
                        break;
                    case "member lookup":
                        MemberLookup(npcList);
                        break;
                    case "member list":
                        FullMemberList(npcList);
                        break;
                    case "room lookup":
                        RoomLookup(npcList);
                        break;
                    case "next period":
                        npcList = GoToNextPeriod(npcList, triggerMap);
                        triggerMap = TriggerDecay(triggerMap);
                        break;
                    case "load backup":
                        npcList = LoadBackup(npcList);
                        break;
                    case "private trigger":
                        PrivateTrigger();
                        break;
                    case "public trigger":
                        triggerMap = PublicTrigger(triggerMap);
                        break;
                    case "display trigger":
                        DisplayTrigger(triggerMap);
                        break;
                }
            }
        EndOfMain:;
        }
        private static List<NPCData> DataLoadIn(List<NPCData> npcList)
        {
            Console.WriteLine("Loading NPC Data...");
            using (var reader = new StreamReader("D:\\D&D Stuff\\NPC Sim\\NPC_doc_start.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new NPCData
                    {
                        Mask = csv.GetField("Mask"),
                        Name = csv.GetField("Name"),
                        Alignment = csv.GetField("Alignment"),
                        Affiliation1 = csv.GetField<int>("Affiliation1"),
                        Affiliation2 = csv.GetField<int>("Affiliation2"),
                        Affiliation3 = csv.GetField<int>("Affiliation3"),
                        Affiliation4 = csv.GetField<int>("Affiliation4"),
                        Personality = csv.GetField("Personality"),
                        Motive = csv.GetField("Motive"),
                        AntiMotive = csv.GetField("AntiMotive"),
                        Skill1 = csv.GetField("Skill1"),
                        Skill2 = csv.GetField("Skill2"),
                        Species = csv.GetField("Species"),
                        Location = csv.GetField<int>("Location"),
                        Cooperation = csv.GetField<double>("Cooperation"),
                        MapKnowledge = csv.GetField<int>("MapKnowledge"),
                        Secrecy = csv.GetField<int>("Secrecy"),
                        Intelligence1 = csv.GetField<int>("Intelligence1"),
                        Intelligence2 = csv.GetField<int>("Intelligence2"),
                        Intelligence3 = csv.GetField<int>("Intelligence3"),
                        Intelligence4 = csv.GetField<int>("Intelligence4"),
                        Intelligence5 = csv.GetField<int>("Intelligence5"),
                        Intelligence6 = csv.GetField<int>("Intelligence6"),
                        FavorOwed = csv.GetField<int>("FavorOwed"),
                        Sociability = csv.GetField<int>("Sociability"),
                        Curiosity = csv.GetField<int>("Curiosity"),
                        Paranoia = csv.GetField<int>("Paranoia"),
                        Mobility = csv.GetField<int>("Mobility"),
                        SpecialBehavior = csv.GetField<int>("SpecialBehavior")
                    };
                    npcList.Add(record);
                }
            }
            CSVSave(npcList);
            return npcList;
        }
        private static List<NPCData> LoadBackup(List<NPCData> npcList)
        {
            Console.Write("Are you sure?: ");
            if (Console.ReadLine() != "yes") { return npcList; }
            Console.WriteLine("Loading NPC Data Backup...");
            npcList.Clear();
            using (var reader = new StreamReader("D:\\D&D Stuff\\NPC Sim\\NPC_doc_start_backup.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new NPCData
                    {
                        Mask = csv.GetField("Mask"),
                        Name = csv.GetField("Name"),
                        Alignment = csv.GetField("Alignment"),
                        Affiliation1 = csv.GetField<int>("Affiliation1"),
                        Affiliation2 = csv.GetField<int>("Affiliation2"),
                        Affiliation3 = csv.GetField<int>("Affiliation3"),
                        Affiliation4 = csv.GetField<int>("Affiliation4"),
                        Personality = csv.GetField("Personality"),
                        Motive = csv.GetField("Motive"),
                        AntiMotive = csv.GetField("AntiMotive"),
                        Skill1 = csv.GetField("Skill1"),
                        Skill2 = csv.GetField("Skill2"),
                        Species = csv.GetField("Species"),
                        Location = csv.GetField<int>("Location"),
                        Cooperation = csv.GetField<double>("Cooperation"),
                        MapKnowledge = csv.GetField<int>("MapKnowledge"),
                        Secrecy = csv.GetField<int>("Secrecy"),
                        Intelligence1 = csv.GetField<int>("Intelligence1"),
                        Intelligence2 = csv.GetField<int>("Intelligence2"),
                        Intelligence3 = csv.GetField<int>("Intelligence3"),
                        Intelligence4 = csv.GetField<int>("Intelligence4"),
                        Intelligence5 = csv.GetField<int>("Intelligence5"),
                        Intelligence6 = csv.GetField<int>("Intelligence6"),
                        FavorOwed = csv.GetField<int>("FavorOwed"),
                        Sociability = csv.GetField<int>("Sociability"),
                        Curiosity = csv.GetField<int>("Curiosity"),
                        Paranoia = csv.GetField<int>("Paranoia"),
                        Mobility = csv.GetField<int>("Mobility"),
                        SpecialBehavior = csv.GetField<int>("SpecialBehavior")
                    };
                    npcList.Add(record);
                }
            }
            return npcList;
        }
        private static void CellLookup(List<NPCData> npcList)
        {
            string column;
            string indexInput;
            int index;
            string stringHolder;

            Console.WriteLine("Entering Cell Lookup...");
            Console.WriteLine("List of properties: mask, name, alignment, affiliation, personality, motive, skills, species, location, cooperation,");
            Console.WriteLine("map knowledge, secrecy, intelligence, favor, sociability, curiosity, paranoia, mobility, special behavior.");
            Console.WriteLine("Enter 'exit' in any field to exit.");

            LoopStart:
            while (true)
            {
                Console.Write("Requested property: ");
                column = Console.ReadLine();
                if (column == "exit")
                    return;
                Console.Write("Requested member: ");
                indexInput = Console.ReadLine();
                if (indexInput == "exit")
                    return;
                index = -1;
                if (true != (Int32.TryParse(indexInput, out index)))
                {
                    Console.WriteLine("The {0} of member {1} could not be found. Request member by number.", column, indexInput);
                    goto LoopStart;
                }
                if (index > npcList.Count)
                {
                    Console.WriteLine("The {0} of member {1} could not be found. There are only {2} members.", column, indexInput, npcList.Count);
                    goto LoopStart;
                }
                index--;
                stringHolder = "";

                switch (column)
                {
                    case "mask":
                        Console.WriteLine("The mask of member {0} is {1}.", indexInput, npcList[index].Mask);
                        break;
                    case "name":
                        Console.WriteLine("The name of member {0} is {1}.", indexInput, npcList[index].Name);
                        break;
                    case "alignment":
                        Console.WriteLine("The alignment of member {0} is {1}.", indexInput, npcList[index].Alignment);
                        break;
                    case "affiliation":
                        if (npcList[index].Affiliation3 != 15) { stringHolder = String.Format("{0}, ", npcList[index].Affiliation3); }
                        if (npcList[index].Affiliation4 != 15) { stringHolder += String.Format("{0}, ", npcList[index].Affiliation4); }
                        stringHolder += String.Format("{0} and {1}", npcList[index].Affiliation1, npcList[index].Affiliation2);
                        Console.WriteLine("The affiliations of member {0} are {1}.", indexInput, stringHolder);
                        break;
                    case "personality":
                        Console.WriteLine("The personality of member {0} is {1}.", indexInput, npcList[index].Personality);
                        break;
                    case "motive":
                        Console.WriteLine("The motive of member {0} is {1}.\nThe anti-motive is {2}.", indexInput, npcList[index].Motive, npcList[index].AntiMotive);
                        break;
                    case "skills":
                        Console.WriteLine("The skills of member {0} are {1} and {2}.", indexInput, npcList[index].Skill1, npcList[index].Skill2);
                        break;
                    case "species":
                        Console.WriteLine("The species of member {0} is {1}.", indexInput, npcList[index].Species);
                        break;
                    case "location":
                        Console.WriteLine("The location of member {0} is {1} ({2}).", indexInput, npcList[index].Location, GetRoomNameList()[npcList[index].Location]);
                        break;
                    case "cooperation":
                        Console.WriteLine("The cooperation level of member {0} is {1}.", indexInput, npcList[index].Cooperation);
                        break;
                    case "map knowledge":
                        Console.WriteLine("The map knowledge of member {0} is {1}.", indexInput, npcList[index].MapKnowledge);
                        break;
                    case "secrecy":
                        Console.WriteLine("The secrecy level of member {0} is {1}.", indexInput, npcList[index].Secrecy);
                        break;
                    case "intelligence":
                        Console.WriteLine("The intelligence of player 1 on member {0} is {1}.", indexInput, npcList[index].Intelligence1);
                        Console.WriteLine("The intelligence of player 2 on member {0} is {1}.", indexInput, npcList[index].Intelligence2);
                        Console.WriteLine("The intelligence of player 3 on member {0} is {1}.", indexInput, npcList[index].Intelligence3);
                        Console.WriteLine("The intelligence of player 4 on member {0} is {1}.", indexInput, npcList[index].Intelligence4);
                        Console.WriteLine("The intelligence of player 5 on member {0} is {1}.", indexInput, npcList[index].Intelligence5);
                        Console.WriteLine("The intelligence of player 6 on member {0} is {1}.", indexInput, npcList[index].Intelligence6);
                        break;
                    case "favor":
                        if (npcList[index].FavorOwed == 1) { stringHolder = "owes"; }
                        else { stringHolder = "doesn't owe"; }
                        Console.WriteLine("Member {0} {1} player 1 a favor.", indexInput, stringHolder);
                        break;
                    case "sociability":
                        Console.WriteLine("The sociability level of member {0} is {1}.", indexInput, npcList[index].Sociability);
                        break;
                    case "curiosity":
                        Console.WriteLine("The curiosity level of member {0} is {1}.", indexInput, npcList[index].Curiosity);
                        break;
                    case "paranoia":
                        Console.WriteLine("The paranoia level of member {0} is {1}.", indexInput, npcList[index].Paranoia);
                        break;
                    case "mobility":
                        Console.WriteLine("The mobility level of member {0} is {1}.", indexInput, npcList[index].Mobility);
                        break;
                    case "special behavior":
                        if (npcList[index].SpecialBehavior == 1) { stringHolder = "has"; }
                        else { stringHolder = "doesn't have"; }
                        Console.WriteLine("Member {0} {1} special behavior.", indexInput, stringHolder);
                        break;
                    default:
                        Console.WriteLine("The {0} of member {1} could not be found. Invalid property.", column, indexInput);
                        break;
                }
            }
        }
        private static List<NPCData> CellChange(List<NPCData> npcList)
        {
            string column;
            string indexInput;
            int index;
            string stringHolder;
            int intHolder;
            double doubleHolder;

            Console.WriteLine("Entering Cell Change...");
            Console.WriteLine("List of properties: mask, name, alignment, affiliation1, affiliation2, affiliation3, affiliation4, personality,");
            Console.WriteLine("motive, anti motive, skill1, skill2, species, location, cooperation, map knowledge, secrecy, intelligence1, ");
            Console.WriteLine("intelligence2, intelligence3, intelligence4, intelligence5, intelligence6, favor, sociability, curiosity,");
            Console.WriteLine("paranoia, mobility, special behavior.");
            Console.WriteLine("Enter 'exit' in any field to exit.");

        LoopStart:
            while (true)
            {
                Console.Write("Requested property: ");
                column = Console.ReadLine();
                if (column == "exit")
                    goto Exit;
                Console.Write("Requested member: ");
                indexInput = Console.ReadLine();
                if (indexInput == "exit")
                    goto Exit;
                index = -1;
                if (true != (Int32.TryParse(indexInput, out index)))
                {
                    Console.WriteLine("The {0} of member {1} could not be found. Request member by number.", column, indexInput);
                    goto LoopStart;
                }
                if (index > npcList.Count)
                {
                    Console.WriteLine("The {0} of member {1} could not be found. There are only {2} members.", column, indexInput, npcList.Count);
                    goto LoopStart;
                }
                index--;
                stringHolder = "";
                intHolder = -1;
                doubleHolder = -1;

                switch (column)
                {
                    case "mask":
                        Console.WriteLine("The mask of member {0} is {1}.", indexInput, npcList[index].Mask);
                        Console.Write("Enter new value for member {0}'s mask: ", indexInput);
                        npcList[index].Mask = Console.ReadLine();
                        Console.WriteLine("The mask of member {0} is now {1}.", indexInput, npcList[index].Mask);
                        break;
                    case "name":
                        Console.WriteLine("The name of member {0} is {1}.", indexInput, npcList[index].Name);
                        Console.Write("Enter new value for member {0}'s name: ", indexInput);
                        npcList[index].Name = Console.ReadLine();
                        Console.WriteLine("The name of member {0} is now {1}.", indexInput, npcList[index].Name);
                        break;
                    case "alignment":
                        Console.WriteLine("The alignment of member {0} is {1}.", indexInput, npcList[index].Alignment);
                        Console.Write("Enter new value for member {0}'s alignment: ", indexInput);
                        npcList[index].Alignment = Console.ReadLine();
                        Console.WriteLine("The alignment of member {0} is now {1}.", indexInput, npcList[index].Alignment);
                        break;
                    case "affiliation1":
                        Console.WriteLine("Affiliation 1 of member {0} is {1}.", indexInput, npcList[index].Affiliation1);
                        Console.Write("Enter new value for member {0}'s affiliation 1: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Affiliation1 = intHolder;
                        Console.WriteLine("Affiliation 1 of member {0} is now {1}.", indexInput, npcList[index].Affiliation1);
                        break;
                    case "affiliation2":
                        Console.WriteLine("Affiliation 2 of member {0} is {1}.", indexInput, npcList[index].Affiliation2);
                        Console.Write("Enter new value for member {0}'s affiliation 2: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Affiliation2 = intHolder;
                        Console.WriteLine("Affiliation 2 of member {0} is now {1}.", indexInput, npcList[index].Affiliation2);
                        break;
                    case "affiliation3":
                        Console.WriteLine("Affiliation 3 of member {0} is {1}.", indexInput, npcList[index].Affiliation3);
                        Console.Write("Enter new value for member {0}'s affiliation 3: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Affiliation3 = intHolder;
                        Console.WriteLine("Affiliation 1 of member {0} is now {1}.", indexInput, npcList[index].Affiliation3);
                        break;
                    case "affiliation4":
                        Console.WriteLine("Affiliation 4 of member {0} is {1}.", indexInput, npcList[index].Affiliation4);
                        Console.Write("Enter new value for member {0}'s affiliation 4: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Affiliation4 = intHolder;
                        Console.WriteLine("Affiliation 4 of member {0} is now {1}.", indexInput, npcList[index].Affiliation4);
                        break;
                    case "personality":
                        Console.WriteLine("The personality of member {0} is {1}.", indexInput, npcList[index].Personality);
                        Console.Write("Enter new value for member {0}'s personality: ", indexInput);
                        npcList[index].Personality = Console.ReadLine();
                        Console.WriteLine("The personality of member {0} is now {1}.", indexInput, npcList[index].Personality);
                        break;
                    case "motive":
                        Console.WriteLine("The motive of member {0} is {1}.", indexInput, npcList[index].Motive);
                        Console.Write("Enter new value for member {0}'s motive: ", indexInput);
                        npcList[index].Motive = Console.ReadLine();
                        Console.WriteLine("The motive of member {0} is now {1}.", indexInput, npcList[index].Motive);
                        break;
                    case "anti motive":
                        Console.WriteLine("The anti motive of member {0} is {1}.", indexInput, npcList[index].AntiMotive);
                        Console.Write("Enter new value for member {0}'s anti motive: ", indexInput);
                        npcList[index].AntiMotive = Console.ReadLine();
                        Console.WriteLine("The anti motive of member {0} is now {1}.", indexInput, npcList[index].AntiMotive);
                        break;
                    case "skill1":
                        Console.WriteLine("Skill 1 of member {0} is {1}.", indexInput, npcList[index].Skill1);
                        Console.Write("Enter new value for member {0}'s skill 1: ", indexInput);
                        npcList[index].Skill1 = Console.ReadLine();
                        Console.WriteLine("Skill 1 of member {0} is now {1}.", indexInput, npcList[index].Skill1);
                        break;
                    case "skill2":
                        Console.WriteLine("Skill 2 of member {0} is {1}.", indexInput, npcList[index].Skill2);
                        Console.Write("Enter new value for member {0}'s skill 2: ", indexInput);
                        npcList[index].Skill2 = Console.ReadLine();
                        Console.WriteLine("Skill 2 of member {0} is now {1}.", indexInput, npcList[index].Skill2);
                        break;
                    case "species":
                        Console.WriteLine("The species of member {0} is {1}.", indexInput, npcList[index].Species);
                        Console.Write("Enter new value for member {0}'s species: ", indexInput);
                        npcList[index].Species = Console.ReadLine();
                        Console.WriteLine("The species of member {0} is now {1}.", indexInput, npcList[index].Species);
                        break;
                    case "location":
                        Console.WriteLine("The location of member {0} is {1} ({2}).", indexInput, npcList[index].Location, GetRoomNameList()[npcList[index].Location]);
                        Console.Write("Enter new value for member {0}'s location: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Location = intHolder;
                        Console.WriteLine("The location of member {0} is now {1} ({2}).", indexInput, npcList[index].Location, GetRoomNameList()[npcList[index].Location]);
                        break;
                    case "cooperation":
                        Console.WriteLine("The cooperation level of member {0} is {1}.", indexInput, npcList[index].Cooperation);
                        Console.Write("Enter new value for member {0}'s cooperation level: ", indexInput);
                        while (true != Double.TryParse(Console.ReadLine(), out doubleHolder))
                        {
                            Console.Write("Invalid input. Enter a number value: ");
                        }
                        npcList[index].Cooperation = doubleHolder;
                        Console.WriteLine("The cooperation level of member {0} is now {1}.", indexInput, npcList[index].Cooperation);
                        break;
                    case "map knowledge":
                        Console.WriteLine("The map knowledge of member {0} is {1}.", indexInput, npcList[index].MapKnowledge);
                        Console.Write("Enter new value for member {0}'s map knowledge: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].MapKnowledge = intHolder;
                        Console.WriteLine("the map knowledge of member {0} is now {1}.", indexInput, npcList[index].MapKnowledge);
                        break;
                    case "secrecy":
                        Console.WriteLine("The secrecy level of member {0} is {1}.", indexInput, npcList[index].Secrecy);
                        Console.Write("Enter new value for member {0}'s secrecy level: ", indexInput);
                        while (true != Double.TryParse(Console.ReadLine(), out doubleHolder))
                        {
                            Console.Write("Invalid input. Enter a number value: ");
                        }
                        npcList[index].Secrecy = doubleHolder;
                        Console.WriteLine("The secrecy level of member {0} is now {1}.", indexInput, npcList[index].Secrecy);
                        break;
                    case "intelligence1":
                        Console.WriteLine("The intelligence of player 1 on member {0} is {1}.", indexInput, npcList[index].Intelligence1);
                        Console.Write("Enter new value for the intelligence of player 1 on member {0}: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Intelligence1 = intHolder;
                        Console.WriteLine("The intelligence of player 1 on member {0} is now {1}.", indexInput, npcList[index].Intelligence1);
                        break;
                    case "intelligence2":
                        Console.WriteLine("The intelligence of player 2 on member {0} is {1}.", indexInput, npcList[index].Intelligence2);
                        Console.Write("Enter new value for the intelligence of player 2 on member {0}: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Intelligence2 = intHolder;
                        Console.WriteLine("The intelligence of player 2 on member {0} is now {1}.", indexInput, npcList[index].Intelligence2);
                        break;
                    case "intelligence3":
                        Console.WriteLine("The intelligence of player 3 on member {0} is {1}.", indexInput, npcList[index].Intelligence3);
                        Console.Write("Enter new value for the intelligence of player 3 on member {0}: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Intelligence3 = intHolder;
                        Console.WriteLine("The intelligence of player 3 on member {0} is now {1}.", indexInput, npcList[index].Intelligence3);
                        break;
                    case "intelligence4":
                        Console.WriteLine("The intelligence of player 4 on member {0} is {1}.", indexInput, npcList[index].Intelligence4);
                        Console.Write("Enter new value for the intelligence of player 4 on member {0}: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Intelligence4 = intHolder;
                        Console.WriteLine("The intelligence of player 4 on member {0} is now {1}.", indexInput, npcList[index].Intelligence4);
                        break;
                    case "intelligence5":
                        Console.WriteLine("The intelligence of player 5 on member {0} is {1}.", indexInput, npcList[index].Intelligence5);
                        Console.Write("Enter new value for the intelligence of player 5 on member {0}: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Intelligence5 = intHolder;
                        Console.WriteLine("The intelligence of player 5 on member {0} is now {1}.", indexInput, npcList[index].Intelligence5);
                        break;
                    case "intelligence6":
                        Console.WriteLine("The intelligence of player 6 on member {0} is {1}.", indexInput, npcList[index].Intelligence6);
                        Console.Write("Enter new value for the intelligence of player 6 on member {0}: ", indexInput);
                        while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
                        {
                            Console.Write("Invalid input. Enter an integer value: ");
                        }
                        npcList[index].Intelligence6 = intHolder;
                        Console.WriteLine("The intelligence of player 6 on member {0} is now {1}.", indexInput, npcList[index].Intelligence6);
                        break;
                    case "favor":
                        if (npcList[index].FavorOwed == 1) { stringHolder = "owes"; }
                        else { stringHolder = "doesn't owe"; }
                        Console.WriteLine("Member {0} {1} player 1 a favor.", indexInput, stringHolder);
                        Console.Write("Does member {0} owe player 1 a favor? Yes or no: ", indexInput);
                        stringHolder = Console.ReadLine();
                        while ((stringHolder != "yes")||(stringHolder != "no"))
                        {
                            Console.Write("Invalid input. Enter yes or no: ");
                        }
                        if (stringHolder == "yes") { npcList[index].FavorOwed = 1; }
                        else { npcList[index].FavorOwed = 0; }
                        if (npcList[index].FavorOwed == 1) { stringHolder = "owes"; }
                        else { stringHolder = "doesn't owe"; }
                        Console.WriteLine("Member {0} now {1} player 1 a favor.", indexInput, npcList[index].FavorOwed);
                        break;
                    case "sociability":
                        Console.WriteLine("The sociability level of member {0} is {1}.", indexInput, npcList[index].Sociability);
                        Console.Write("Enter new value for member {0}'s sociability level: ", indexInput);
                        while (true != Double.TryParse(Console.ReadLine(), out doubleHolder))
                        {
                            Console.Write("Invalid input. Enter a number value: ");
                        }
                        npcList[index].Sociability = doubleHolder;
                        Console.WriteLine("The sociability level of member {0} is now {1}.", indexInput, npcList[index].Sociability);
                        break;
                    case "curiosity":
                        Console.WriteLine("The curiosity level of member {0} is {1}.", indexInput, npcList[index].Curiosity);
                        Console.Write("Enter new value for member {0}'s curiosity level: ", indexInput);
                        while (true != Double.TryParse(Console.ReadLine(), out doubleHolder))
                        {
                            Console.Write("Invalid input. Enter a number value: ");
                        }
                        npcList[index].Curiosity = doubleHolder;
                        Console.WriteLine("The curiosity level of member {0} is now {1}.", indexInput, npcList[index].Curiosity);
                        break;
                    case "paranoia":
                        Console.WriteLine("The paranoia level of member {0} is {1}.", indexInput, npcList[index].Paranoia);
                        Console.Write("Enter new value for member {0}'s paranoia level: ", indexInput);
                        while (true != Double.TryParse(Console.ReadLine(), out doubleHolder))
                        {
                            Console.Write("Invalid input. Enter a number value: ");
                        }
                        npcList[index].Paranoia = doubleHolder;
                        Console.WriteLine("The paranoia level of member {0} is now {1}.", indexInput, npcList[index].Paranoia);
                        break;
                    case "mobility":
                        Console.WriteLine("The mobility level of member {0} is {1}.", indexInput, npcList[index].Mobility);
                        Console.Write("Enter new value for member {0}'s mobility level: ", indexInput);
                        while (true != Double.TryParse(Console.ReadLine(), out doubleHolder))
                        {
                            Console.Write("Invalid input. Enter a number value: ");
                        }
                        npcList[index].Mobility = doubleHolder;
                        Console.WriteLine("The mobility level of member {0} is now {1}.", indexInput, npcList[index].Mobility);
                        break;
                    case "special behavior":
                        if (npcList[index].SpecialBehavior == 1) { stringHolder = "has"; }
                        else { stringHolder = "doesn't have"; }
                        Console.WriteLine("Member {0} {1} special behavior.", indexInput, stringHolder); 
                        Console.Write("Does member {0} have special behavior? Yes or no: ", indexInput);
                        stringHolder = Console.ReadLine();
                        while ((stringHolder != "yes") && (stringHolder != "no"))
                        {
                            Console.Write("Invalid input. Enter yes or no: ");
                            Console.ReadLine();
                        }
                        if (stringHolder == "yes") { npcList[index].SpecialBehavior = 1; }
                        else { npcList[index].SpecialBehavior = 0; }
                        if (npcList[index].SpecialBehavior == 1) { stringHolder = "has"; }
                        else { stringHolder = "doesn't have"; }
                        Console.WriteLine("Member {0} now {1} special behavior.", indexInput, stringHolder);
                        break;
                    default:
                        Console.WriteLine("The {0} of member {1} could not be found. Invalid property.", column, indexInput);
                        break;
                }
            }
        Exit:
            CSVSave(npcList);
            return npcList;
        }
        private static void CSVSave(List<NPCData> npcList)
        {
            using (var writer = new StreamWriter("D:\\D&D Stuff\\NPC Sim\\NPC_doc_start.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<NPCData>();
                csv.NextRecord();
                foreach (var record in npcList)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }
            Console.WriteLine("Save completed!");
        }
        private static void ReadoutHeader()
        {
            Console.WriteLine("Member  Mask             Name        Alignment       A1  A2  A3  A4  Personality              Motive         Anti-Motive    Skill 1         Skill 2         Species       Location  Cooperation  Map  Secrecy  I1  I2  I3  I4  I5  I6  Favor  Soc  Cur  Par  Mob  SB");
        }
        private static void MemberReadout(List<NPCData> npcList, int index)
        {
            string valueString;
            int lengthTarget;

            lengthTarget = 8;
            valueString = String.Format("{0}", index + 1);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 17;
            valueString += String.Format("{0}", npcList[index].Mask);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 12;
            valueString += String.Format("{0}", npcList[index].Name);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 16;
            valueString += String.Format("{0}", npcList[index].Alignment);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Affiliation1);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Affiliation2);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Affiliation3);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Affiliation4);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 25;
            valueString += String.Format("{0}", npcList[index].Personality);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 15;
            valueString += String.Format("{0}", npcList[index].Motive);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 15;
            valueString += String.Format("{0}", npcList[index].AntiMotive);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 16;
            valueString += String.Format("{0}", npcList[index].Skill1);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 16;
            valueString += String.Format("{0}", npcList[index].Skill2);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 14;
            valueString += String.Format("{0}", npcList[index].Species);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 10;
            valueString += String.Format("{0}", npcList[index].Location);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 13;
            valueString += String.Format("{0}", npcList[index].Cooperation);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 5;
            valueString += String.Format("{0}", npcList[index].MapKnowledge);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 9;
            valueString += String.Format("{0}", npcList[index].Secrecy);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Intelligence1);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Intelligence2);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Intelligence3);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Intelligence4);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Intelligence5);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 4;
            valueString += String.Format("{0}", npcList[index].Intelligence6);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 7;
            valueString += String.Format("{0}", npcList[index].FavorOwed);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 5;
            valueString += String.Format("{0}", npcList[index].Sociability);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 5;
            valueString += String.Format("{0}", npcList[index].Curiosity);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 5;
            valueString += String.Format("{0}", npcList[index].Paranoia);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 5;
            valueString += String.Format("{0}", npcList[index].Mobility);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            lengthTarget += 0;
            valueString += String.Format("{0}", npcList[index].SpecialBehavior);
            while (valueString.Length < lengthTarget) { valueString += " "; }
            Console.WriteLine(valueString);
        }
        private static void MemberLookup(List<NPCData> npcList)
        {
            int index;
            
            Console.WriteLine("Entering Member Lookup...");
            Console.Write("Requested member: ");
            while (true != Int32.TryParse(Console.ReadLine(), out index)) 
            {
                Console.Write("Invalid input. Enter an integer between 1 and {0}: ", npcList.Count);
            }
            index--;
            ReadoutHeader();
            MemberReadout(npcList, index);
        }
        private static void FullMemberList(List<NPCData> npclist)
        {
            Console.WriteLine("Entering Member List...");

            ReadoutHeader();
            for (int index = 0; index < npclist.Count; index++)
            {
                MemberReadout(npclist, index);
            }
        }
        private static void RoomLookup(List<NPCData> npcList)
        {
            int intHolder;
            string[] roomNameList = GetRoomNameList();
            
            Console.WriteLine("Entering Room Lookup...");
            Console.Write("Input room number: ");
            while (true != Int32.TryParse(Console.ReadLine(), out intHolder))
            {
                Console.Write("Invalid input. Enter an integer between 1 and {0}", roomNameList.Length);
            }
            Console.WriteLine("Showing all occupants of room {0} ({1})", intHolder, roomNameList[intHolder]);

            ReadoutHeader();
            for (int index = 0; index < npcList.Count; index++)
            {
                if (npcList[index].Location == intHolder)
                {
                    MemberReadout(npcList, index);
                }
            }
        }
        private static string[] GetRoomNameList()
        {
            string[] roomNames = { "Outside", "Entrance Hall", "2F Lobby", "2F Staircase Hall", "Private Room 1", "Private Room 2", "Private Room 3", "Private Room 4",
                                 "2F NW Hall", "2F SW Hall", "2F NE Hall", "2F SE Hall", "3F Lobby", "3F NW Hall", "3F SW Hall", "3F NE Hall", "3F SE Hall",
                                 "4F Lobby", "SE Garden", "SW Garden", "NW Garden", "King's Lobby", "King's Quarters", "N Storage Room", "S Storage Room",
                                 "1F Servant Passage", "N Kitchen", "S Kitchen", "Servant's Quarters", "Room 1", "Room 2", "Room 3", "Room 4", "Room 5",
                                 "Room 6", "Room 7", "Room 8", "Room 9", "Room 10", "Room 11", "Room 12", "1F Elevator", "2F Elevator", "2F Servant Passage",
                                 "2F Servant Back Passage", "3F Elevator", "3F Servant Passage", "BF Elevator", "BF Teleporter Room", "W Collapsed Passage",
                                 "E Collapsed Passage", "BF Wizard's Tower", "1F Secret Passage", "1F Wizard's Tower", "2F Staircase Alcove", "2F S Alcove",
                                 "2F Secret Passage", "2F Wizard's Tower", "3F Secret Passage", "3F Wizard's Tower", "4F Wizard's Tower", "5F Wizard's Tower",
                                 "BF Pipe", "0.5F Elevator", "0.5F N Passage", "0.5F Central Passage", "0.5F S Passage", "0.5F Pipe", "0.5F Wizard's Tower",
                                 "1F Pipe", "1.5F Elevator", "1.5F W Passage", "1.5F S Passage", "1.5F N Passage", "1.5F Wizard's Tower", "1.5F Pipe",
                                 "2F Pipe", "2.5F Elevator", "2.5F N Passage", "2.5F Central Passage", "2.5F S Passage", "2.5F Wizard's Tower", "2.5F Pipe",
                                 "3F Pipe", "3.5F Elevator", "3.5F W Passage", "3.5F Central Passage", "3.5F Wizard's Tower", "3.5F Pipe", "4F Pipe", "4.5F Secret Room",
                                 "4.5F Teleporter Room", "4.5F Wizard's Tower", "4.5F Pipe", "5F Pipe", "The Backrooms"};
            return roomNames;
        }
        private static int[] GetRoomSecurity()
        {
            int[] roomSecurity = {0,0,0,0,0,0,0,0,0,0,0,
                                    0,0,0,0,0,0,0,0,0,0,
                                    1,1,2,2,2,2,2,2,2,2,
                                    2,2,2,2,2,2,2,2,2,2,
                                    2,2,2,2,2,2,2,2,2,3,
                                    3,3,3,3,3,3,3,3,3,3,
                                    3,4,4,4,4,4,4,4,4,4,
                                    4,4,4,4,4,4,4,4,4,4,
                                    4,4,4,4,4,4,4,4,4,4,
                                    4,4,4,4,5 };
            return roomSecurity;
        }
        private static int[,] GetRoomBorders()
        {
            int[,] roomBorders = { { 1, 26, 48, 91, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 0,  2, 66, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 1,  3,  8, 10, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 2,  4,  5,  6,  7, 43, 44, 54, 95, 95, 95, 95, 95, 95 },
                                   { 3, 42, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 3, 42, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 3, 44, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 3, 44, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 2,  9, 43, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 8, 43, 55, 56, 72, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 2, 11, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {10, 55, 56, 72, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 3, 13, 15, 58, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {12, 14, 15, 46, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {13, 16, 46, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {12, 13, 16, 58, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {14, 15, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {12, 18, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {17, 19, 21, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {18, 20, 85, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {19, 89, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {18, 22, 89, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {21, 61, 94, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {24, 25, 28, 41, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {23, 66, 72, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {23, 26, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 0, 25, 27, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {26, 66, 72, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {23, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 52 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 69, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {23, 42, 45, 47, 63, 70, 77, 84, 95, 95, 95, 95, 95, 95 },
                                   { 4,  5, 41, 43, 45, 47, 63, 70, 77, 84, 95, 95, 95, 95 },
                                   { 3,  8,  9, 42, 56, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 3,  6,  7, 57, 76, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {41, 42, 46, 47, 63, 70, 77, 84, 95, 95, 95, 95, 95, 95 },
                                   {13, 14, 45, 80, 85, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {41, 42, 45, 48, 63, 70, 77, 84, 95, 95, 95, 95, 95, 95 },
                                   { 0, 47, 49, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {48, 50, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {49, 51, 62, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {50, 62, 67, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {28, 53, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {52, 57, 59, 60, 61, 67, 69, 74, 81, 87, 92, 95, 95, 95 },
                                   { 3, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 9, 11, 56, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 9, 11, 43, 55, 80, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {44, 53, 59, 60, 61, 67, 74, 76, 81, 87, 92, 95, 95, 95 },
                                   {12, 15, 59, 83, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {53, 57, 58, 60, 61, 67, 74, 81, 83, 87, 92, 95, 95, 95 },
                                   {63, 57, 59, 61, 67, 74, 81, 87, 89, 92, 95, 95, 95, 95 },
                                   {22, 53, 57, 59, 60, 67, 74, 81, 87, 92, 94, 95, 95, 95 },
                                   {50, 51, 68, 69, 75, 76, 82, 83, 88, 89, 93, 94, 95, 95 },
                                   {41, 42, 45, 47, 64, 70, 77, 84, 95, 95, 95, 95, 95, 95 },
                                   {63, 65, 68, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {64, 66, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 1, 24, 27, 65, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {51, 53, 57, 59, 68, 74, 81, 87, 95, 95, 95, 95, 95, 95 },
                                   {62, 64, 67, 69, 75, 76, 82, 83, 88, 89, 93, 94, 95, 95 },
                                   {34, 53, 62, 68, 75, 76, 82, 83, 88, 89, 93, 94, 95, 95 },
                                   {41, 42, 45, 47, 63, 71, 73, 77, 84, 95, 95, 95, 95, 95 },
                                   {70, 72, 73, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   { 9, 11, 24, 27, 71, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {70, 71, 75, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {51, 53, 57, 59, 60, 61, 67, 75, 81, 87, 92, 95, 95, 95 },
                                   {62, 68, 69, 73, 74, 76, 82, 83, 88, 89, 93, 94, 95, 95 },
                                   {44, 57, 62, 68, 69, 75, 76, 82, 83, 88, 89, 93, 94, 95 },
                                   {41, 42, 45, 47, 63, 70, 78, 84, 95, 95, 95, 95, 95, 95 },
                                   {77, 79, 82, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {78, 80, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {46, 56, 79, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {51, 53, 57, 59, 60, 61, 67, 74, 87, 92, 95, 95, 95, 95 },
                                   {62, 68, 69, 75, 76, 78, 81, 83, 88, 89, 93, 94, 95, 95 },
                                   {58, 59, 62, 68, 69, 75, 76, 82, 88, 89, 93, 94, 95, 95 },
                                   {41, 42, 45, 47, 63, 70, 77, 85, 86, 95, 95, 95, 95, 95 },
                                   {19, 46, 84, 86, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {84, 85, 88, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {51, 53, 57, 59, 60, 61, 67, 74, 81, 92, 95, 95, 95, 95 },
                                   {62, 68, 69, 75, 76, 82, 83, 89, 93, 94, 95, 95, 95, 95 },
                                   {20, 21, 60, 62, 68, 69, 75, 76, 82, 83, 88, 93, 94, 95 },
                                   {91, 92, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {90, 93, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 },
                                   {51, 53, 57, 59, 60, 61, 67, 74, 81, 87, 90, 93, 95, 95 },
                                   {62, 68, 69, 75, 76, 82, 83, 88, 89, 91, 92, 94, 95, 95 },
                                   {22, 61, 62, 68, 69, 75, 76, 82, 83, 88, 89, 93, 95, 95 },
                                   {95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95, 95 } };
            return roomBorders;
        }
        private static double[] GetRoomMovements()
        {
            double[] roomMovements = {0,3,5,5,3,1,1,1,9,7,9,
                                        7,5,9,7,9,7,6,5,5,5,
                                        1,1,3,3,3,3,3,2,1,1,
                                        1,1,1,1,1,1,1,1,1,1,
                                        3,3,2,2,3,2,3,3,1,1,
                                        1,2,2,2,2,2,2,2,1,1,
                                        1,0,0,0,0,0,0,0,0,0,
                                        0,0,0,0,0,0,0,0,0,0,
                                        0,0,0,0,0,0,0,0,0,0,
                                        0,0,0,0,0 };
            return roomMovements;
        }
        private static List<NPCData> GoToNextPeriod(List<NPCData> npcList, int[] triggerMap)
        {
            int currentRoom;
            int currentNextRoom;
            int occupantCounter;
            int curiosityCheck1 = 0;
            int curiosityCheck2 = 0;
            double memberSum;
            double rngValue;
            Random rng = new Random();
            double[] currentMove = new double[15];
            
            List<NPCData> npcListOld = npcList;
            int[] roomSecurity = GetRoomSecurity();
            double[] roomMovements = GetRoomMovements();
            int[,] roomBorders = GetRoomBorders();
            int[] roomOccupancy = new int[roomSecurity.Length];
            occupantCounter = 0;

            Console.WriteLine("Entering Next Period...");

            for (int i = 0; i < roomSecurity.Length; i++)
            {
                for (int j = 0; j < npcList.Count; j++)
                {
                    if (npcList[j].Location == i) { occupantCounter++; } 
                }
                roomOccupancy[i] = occupantCounter;
            }
            
            for (int index = 0; index < npcList.Count; index++)
            {
                memberSum = 0;
                currentRoom = npcList[index].Location;
                for (int room = 0; room < 15; room++)
                {
                    if (room == 0) { currentNextRoom = currentRoom; }
                    else { currentNextRoom = roomBorders[currentRoom, room - 1]; }
                    currentMove[room] = roomMovements[currentNextRoom]; 
                    if (npcList[index].MapKnowledge == 0) { currentMove[room] = 4; }
                    //factoring map knowledge
                    if (roomSecurity[currentNextRoom] > npcList[index].MapKnowledge) { currentMove[room] = 0; }
                    //factoring sociability
                    currentMove[room] *= Math.Pow(1.02, npcList[index].Sociability * roomOccupancy[currentNextRoom]);
                    //factoring curiosity
                    //it's scuffed but I'm 99% sure it works
                    for (int k = 0; k < npcList.Count; k++)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            for (int m = 0; m < 4; m++)
                            {
                                if (npcList[currentNextRoom].Location != npcList[k].Location) { goto CuriosityCheckNextMember; }
                                switch(l)
                                {
                                    case 0:
                                        curiosityCheck1 = npcList[index].Affiliation1;
                                        break;
                                    case 1:
                                        curiosityCheck1 = npcList[index].Affiliation2;
                                        break;
                                    case 2:
                                        if (npcList[index].Affiliation3 != 15) { curiosityCheck1 = npcList[index].Affiliation3; }
                                        else { goto CuriosityCheckNextCheck; }
                                        break;
                                    case 3:
                                        if (npcList[index].Affiliation3 != 15) { curiosityCheck1 = npcList[index].Affiliation4; }
                                        else { goto CuriosityCheckNextCheck; }
                                        break;

                                }
                                switch (l)
                                {
                                    case 0:
                                        curiosityCheck2 = npcList[k].Affiliation1;
                                        break;
                                    case 1:
                                        curiosityCheck2 = npcList[k].Affiliation2;
                                        break;
                                    case 2:
                                        if (npcList[index].Affiliation3 != 15) { curiosityCheck2 = npcList[k].Affiliation3; }
                                        else { goto CuriosityCheckNextCheck; }
                                        break;
                                    case 3:
                                        if (npcList[index].Affiliation3 != 15) { curiosityCheck2 = npcList[k].Affiliation4; }
                                        else { goto CuriosityCheckNextCheck; }
                                        break;
                                }
                                if (curiosityCheck1 == curiosityCheck2)
                                {
                                    currentMove[room] *= Math.Pow(1.05, npcList[index].Curiosity);
                                    goto CuriosityCheckNextMember;
                                }
                            CuriosityCheckNextCheck:;
                            }
                        }
                    CuriosityCheckNextMember:;
                    }
                    //factoring paranoia
                    currentMove[room] *= Math.Pow(.95, npcList[index].Paranoia * triggerMap[currentNextRoom]);
                    //factoring mobility
                    currentMove[room] *= Math.Pow(.95, npcList[index].Mobility);
                    //special behavior
                    if (npcList[index].SpecialBehavior == 1)
                    {
                        if (index < 4)
                        {
                            if (currentNextRoom == npcList[99].Location) { currentMove[room] *= 3; }
                        }
                        else if (index == 4)
                        {
                            if (currentNextRoom == npcList[101].Location) { currentMove[room] *= .1; }
                        }
                        else if (index == 5)
                        {
                            switch (currentNextRoom)
                            {
                                case 51:
                                    currentMove[room] *= 3;
                                    break;
                                case 53:
                                    currentMove[room] *= 3;
                                    break;
                                case 57:
                                    currentMove[room] *= 3;
                                    break;
                                case 59:
                                    currentMove[room] *= 3;
                                    break;
                                case 60:
                                    currentMove[room] *= 3;
                                    break;
                                case 61:
                                    currentMove[room] *= 3;
                                    break;
                            }
                        }
                        else if (index == 6)
                        {
                            if (currentNextRoom == npcList[101].Location) { currentMove[room] *= 3; }
                            if (currentNextRoom == npcList[100].Location) { currentMove[room] *= 3; }
                        }
                        else
                        {
                            currentMove[room] = 0;
                        }
                    }
                    if (currentNextRoom == 95) { currentMove[room] = 0; }
                    memberSum += currentMove[room];
                }
                rngValue = rng.NextDouble();
                for (int room = 0; room < 15; room++)
                {
                    if (room == 0) { currentNextRoom = currentRoom; }
                    else { currentNextRoom = roomBorders[currentRoom, room - 1]; }
                    currentMove[room] = currentMove[room] / memberSum;
                    if (currentMove[room] > rngValue)
                    {
                        npcList[index].Location = currentNextRoom;
                        goto NextMember;
                    }
                    else
                    {
                        rngValue = rngValue - currentMove[room];
                    }
                }
            NextMember:;
            }
            CSVSave(npcList);
            return npcList;
        }
        private static void PrivateTrigger()
        {
            Stack<int> currentStack = new Stack<int>();
            Stack<int> nextStack = new Stack<int>();
            Stack<int> checkedRooms = new Stack<int>();
            int sourceRoom;
            int targetRoom;
            int triggerStrength;
            int[,] roomBorders = GetRoomBorders();
            string[] roomNames = GetRoomNameList();

            Console.WriteLine("Entering Private Trigger...");

            Console.Write("Enter trigger source: ");
            while (true != Int32.TryParse(Console.ReadLine(), out sourceRoom))
            {
                Console.Write("Invalid input. Enter an integer between 1 and {0}", roomNames.Length);
            }
            Console.Write("Enter trigger strength: ");
            while (true != Int32.TryParse(Console.ReadLine(), out triggerStrength))
            {
                Console.Write("Invalid input. Enter an integer between 1 and {0}", roomNames.Length);
            }
            Console.Write("Enter target room: ");
            while (true != Int32.TryParse(Console.ReadLine(), out targetRoom))
            {
                Console.Write("Invalid input. Enter an integer between 1 and {0}", roomNames.Length);
            }
            currentStack.Push(sourceRoom);
            triggerStrength--;
            while (triggerStrength > 0)
            {
                while (currentStack.Count > 0)
                {
                    for (int i = 0; i < 14; i++)
                    {
                        if (roomBorders[currentStack.Peek(), i] == targetRoom) { goto LoopEnd; }
                        if (true != checkedRooms.Contains(roomBorders[currentStack.Peek(), i])) { nextStack.Push(roomBorders[currentStack.Peek(), i]); }
                    }
                    currentStack.Pop();
                }
                triggerStrength--;
                while (nextStack.Count > 0)
                {
                    currentStack.Push(nextStack.Pop());
                }
            }
            Console.WriteLine("There is no trigger in room {0} ({1}).", targetRoom, roomNames[targetRoom]);
            return;
        LoopEnd:;
            Console.WriteLine("There is a trigger of strength {0} in room {1} ({2}).", triggerStrength, targetRoom, roomNames[targetRoom]);
            return;
        }
        private static int[] PublicTrigger(int[] triggerMap)
        {
            Stack<int> currentStack = new Stack<int>();
            Stack<int> nextStack = new Stack<int>();
            int sourceRoom;
            int triggerStrength;
            int currentRoom;
            int[,] roomBorders = GetRoomBorders();
            string[] roomNames = GetRoomNameList();

            Console.WriteLine("Entering Public Trigger...");

            Console.Write("Enter trigger source: ");
            while (true != Int32.TryParse(Console.ReadLine(), out sourceRoom))
            {
                Console.Write("Invalid input. Enter an integer between 1 and {0}", roomNames.Length);
            }
            Console.Write("Enter trigger strength: ");
            while (true != Int32.TryParse(Console.ReadLine(), out triggerStrength))
            {
                Console.Write("Invalid input. Enter an integer between 1 and {0}", roomNames.Length);
            }
            currentStack.Push(sourceRoom);
            triggerMap[sourceRoom] = triggerStrength;
            triggerStrength--;
            while (triggerStrength > 0)
            {
                while (currentStack.Count > 0)
                {
                    for (int i = 0; i < 14; i++)
                    {
                        currentRoom = roomBorders[currentStack.Peek(), i];
                        if (triggerMap[currentRoom] < triggerStrength) 
                        { 
                            nextStack.Push(currentRoom);
                            triggerMap[currentRoom] = triggerStrength;
                        }
                    }
                    currentStack.Pop();
                }
                triggerStrength--;
                while (nextStack.Count > 0)
                {
                    currentStack.Push(nextStack.Pop());
                }
            }
            return triggerMap;
        }
        private static int[] TriggerDecay(int[] triggerMap)
        {
            for (int i = 0; i < triggerMap.Length; i++)
            {
                triggerMap[i]--;
                if (triggerMap[i] < 0) { triggerMap[i] = 0; }
            }
            return triggerMap;
        }
        private static void DisplayTrigger(int[] triggerMap)
        {
            Console.WriteLine("Entering Trigger Display...");
            
            string str = "";
            string[] roomNames = GetRoomNameList();
            for (int i = 0; i < triggerMap.Length; i++)
            {
                str = String.Format("{0}\t{1}", i, roomNames[i]);
                while (str.Length <= 30) { str += " "; }
                if (i > 9) { str += " "; }
                str += String.Format("{0}", triggerMap[i]);
                Console.WriteLine(str);
            }
        }
    }
}