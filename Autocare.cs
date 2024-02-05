using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Globalization;
using System.Security;
using Microsoft.Win32;
using System.Drawing;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace ACESinspector
{
    public class Asset
    {
        public int id;
        public string action;
        public int basevehicleid;
        public string assetName;
        public List<VCdbAttribute> VCdbAttributes;
        public List<QdbQualifier> QdbQualifiers;
        public List<String> notes;

        public Asset()
        {

            VCdbAttributes = new List<VCdbAttribute>();
            VCdbAttributes.Clear();
            QdbQualifiers = new List<QdbQualifier>();
            QdbQualifiers.Clear();
            notes = new List<string>();
            notes.Clear();
        }

        public string niceFullFitmentString(VCdb vcdb, Qdb qdb)
        {
            List<string> stringList = new List<string>();
            if (VCdbAttributes.Count() > 0) { stringList.Add(this.niceAttributesString(vcdb, false)); }
            if (QdbQualifiers.Count() > 0) { stringList.Add(this.niceQdbQualifierString(qdb)); }
            if (notes.Count() > 0) { stringList.AddRange(notes); }
            return String.Join(";", stringList.ToArray());
        }

        public string niceAttributesString(VCdb vcdb, bool includeNotes)
        {// returns human-readable (Limited; V6 2.3L;) rendition of VCdb-coded attributes (and optionally the notes) from this app 
            List<string> stringList = new List<string>();
            foreach (VCdbAttribute myAttribute in VCdbAttributes) { stringList.Add(vcdb.niceAttribute(myAttribute)); }
            if (includeNotes) { stringList.AddRange(notes); }
            return string.Join(";", stringList);
        }

        public string niceQdbQualifierString(Qdb qdb)
        {
            string returnString = "";
            foreach (QdbQualifier myQdbQualifier in QdbQualifiers)
            {
                returnString += qdb.niceQdbQualifier(myQdbQualifier.qualifierId, myQdbQualifier.qualifierParameters);

            }
            return returnString;
        }

    }


    /// <summary>
    ///  Each instance of App contains data derived from one App node in the ACES xml file
    /// </summary>
    public class App : IComparable<App>
    {

        /*
          As of ACES 4.0, there are 4 ways an app can communicate a "vehicle":
          	- Base Vehicle ID
			- Make / Year-Range 
			- Base Equipment ID
			- Mfr / Equipment Model / Vehicle Type
         
          during import, apps of the second type (make/year-range) will be dynamically converted to basevehicle-type apps. So for the purposes of ACESinspector, there are three types of
          apps:
            1 - Base Vehicle ID
			2 - Base Equipment ID
			3 - Mfr / Equipment Model / Vehicle Type




         */

        public int id;
        public int type; //1=basevehicle, 2=equipmentbase, 3=Mfr / Equipment Model / Vehicle Type
        public string reference;
        public string action;
        public bool validate;
        public int basevehicleid;
        public int parttypeid;
        public int positionid;
        public int quantity;
        public string part;
        public string mfrlabel;
        public string asset;
        public int assetitemorder;
        public string assetitemref;
        public List<VCdbAttribute> VCdbAttributes;
        public List<QdbQualifier> QdbQualifiers;
        public List<String> notes;
        public bool containsVCdbVolation;
        public bool hasBeenValidated;
        public List<validationProblem> problemsFound;
        public string hash;
        public string brand;
        public string subbrand;

        public App()
        {
            VCdbAttributes = new List<VCdbAttribute>();
            VCdbAttributes.Clear();
            QdbQualifiers = new List<QdbQualifier>();
            QdbQualifiers.Clear();
            notes = new List<string>();
            notes.Clear();
            problemsFound = new List<validationProblem>();
        }

        public void Clear()
        {
            id = 0;
            action = "";
            basevehicleid = 0;
            parttypeid = 0;
            positionid = 0;
            quantity = 0;
            part = "";
            notes.Clear();
            mfrlabel = "";
            asset = "";
            assetitemorder = 0;
            assetitemref = "";
            containsVCdbVolation = false;
            VCdbAttributes.Clear();
            QdbQualifiers.Clear();
            //            problemsFound.Clear();
            //hasBeenValidated = false;
        }

        public string niceAttributesString(VCdb vcdb, bool includeNotes)
        {// returns human-readable (Limited; V6 2.3L;) rendition of VCdb-coded attributes (and optionally the notes) from this app 
            List<string> stringList = new List<string>();
            foreach (VCdbAttribute myAttribute in VCdbAttributes) { stringList.Add(vcdb.niceAttribute(myAttribute)); }
            if (includeNotes) { stringList.AddRange(notes); }
            return string.Join(";", stringList);
        }

        public string namevalpairString(bool includeNotes)
        {// returns CSS-style (Submodel:13;EnvineBase:332;) rendition of VCdb-coded attributes in this app
            List<string> stringList = new List<string>();
            foreach (VCdbAttribute myAttribute in VCdbAttributes) { stringList.Add(myAttribute.name + ":" + myAttribute.value.ToString()); }
            if (includeNotes) { stringList.AddRange(notes); }
            return string.Join(";", stringList);
        }

        public string rawQdbDataString()
        {// return a tab & semicolon delimited string of all Qdb qualifiers in this app like: 
            //  the two QdbQualifiers: "1 Piece Driveshaft" and "with Aluminum Driveshaft"   would be "240:1::;12993:::;
            string returnString = "";

            foreach (QdbQualifier myQdbQualifier in QdbQualifiers)
            {
                returnString += myQdbQualifier.qualifierId.ToString();
                foreach (String parameter in myQdbQualifier.qualifierParameters)
                {
                    returnString += ":" + parameter;
                }
                returnString += ";";
            }
            return returnString;
        }

        public string niceQdbQualifierString(Qdb qdb)
        {
            List<string> stringList = new List<string>();
            
            foreach (QdbQualifier myQdbQualifier in QdbQualifiers)
            {
                //returnString += qdb.niceQdbQualifier(myQdbQualifier.qualifierId, myQdbQualifier.qualifierParameters);
                stringList.Add(qdb.niceQdbQualifier(myQdbQualifier.qualifierId, myQdbQualifier.qualifierParameters));

            }
            //return returnString;
            return String.Join(";", stringList.ToArray());
        }

        public string niceFullFitmentString(VCdb vcdb, Qdb qdb)
        {
            List<string> stringList = new List<string>();
            if (VCdbAttributes.Count() > 0) { stringList.Add(this.niceAttributesString(vcdb, false)); }
            if (QdbQualifiers.Count() > 0) { stringList.Add(this.niceQdbQualifierString(qdb)); }
            if (notes.Count() > 0) { stringList.AddRange(notes); }
            return String.Join(";", stringList.ToArray());
        }

        public string niceMMYstring(VCdb vcdb)
        {
            return vcdb.niceMakeOfBasevid(this.basevehicleid) + ", " + vcdb.niceModelOfBasevid(this.basevehicleid) + ", " + vcdb.niceYearOfBasevid(this.basevehicleid);
        }


        public string appHash()
        {
            string result = "";
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(basevehicleid.ToString() + parttypeid.ToString() + positionid.ToString() + quantity.ToString() + namevalpairString(true) + rawQdbDataString() + mfrlabel + part + asset + assetitemorder.ToString() + brand + subbrand));
                for (int i = 0; i < data.Length; i++){result+=data[i].ToString("x2");}
            }
            return result;
        }


        /*        public string MMYstringOfdeletedBasevid(VCdb vcdb)
        {




        }
        */


        public int CompareTo(App other)
        {// used by Array.Sort() after all apps have been imported from the XML into memory
            int strCmpResults = 0;

            if (this.basevehicleid > other.basevehicleid)
            {//A basevehicleid > B basevehicleid
                return (+1);
            }
            else
            {//A basevehicleid <= B basevehicleid
                if (this.basevehicleid == other.basevehicleid)
                {//A basevehicleid = B basevehicleid -  now compare secondary stuff
                    if (this.parttypeid > other.parttypeid)
                    {// A parttypeid > B parttypeid
                        return (+1);
                    }
                    else
                    {// A parttypeid <= B parttypeid
                        if (this.parttypeid == other.parttypeid)
                        {// basebids are equal, parttypeids are equal

                            if (this.positionid > other.positionid)
                            {//A pos > B pos
                                return (+1);
                            }
                            else
                            {//A pos <= B pos
                                if (this.positionid == other.positionid)
                                {// basebids are equal, parttypeids are equal, positionids are equal
                                    if (string.Compare(this.part, other.part) > 0)
                                    {// a.part > b.part
                                        return (+1);
                                    }
                                    else
                                    {//a.part <= b.part
                                        if (this.part == other.part)
                                        {/// basebids are equal, parttypeids are equal, positionids are equal, parts are equal
                                            if (string.Compare(this.mfrlabel, other.mfrlabel) > 0)
                                            {// mfrlabels A > B
                                                return (+1);
                                            }
                                            else
                                            {// mfrlabels A <= B
                                                if (this.mfrlabel == other.mfrlabel)
                                                {
                                                    strCmpResults = string.Compare(this.namevalpairString(true), other.namevalpairString(true));
                                                    if (strCmpResults > 0)
                                                    {// qualifiers A > qualifiers B
                                                        return (+1);
                                                    }
                                                    else
                                                    {//qualifiers A <= qualifiers B
                                                        if (strCmpResults == 0)
                                                        {// basebids are equal, parttypeids are equal, positionids are equal, parts are equal, mfrlabels are equal, qualifiers are equal,
                                                            if (string.Compare(this.asset, other.asset) > 0)
                                                            {//assetid A > assetid B
                                                                return (+1);
                                                            }
                                                            else
                                                            {
                                                                if (this.asset == other.asset)
                                                                {// basebids are equal, parttypeids are equal, positionids are equal, parts are equal, mfrlabels are equal, qualifiers are equal, assetid's are equal
                                                                    if (this.assetitemorder > other.assetitemorder)
                                                                    {//assetorder A > assetorder B
                                                                        return (+1);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.assetitemorder == other.assetitemorder)
                                                                        {// basebids are equal, parttypeids are equal, positionids are equal, parts are equal, mfrlabels are equal, qualifiers are equal, assetid's are equal, asset orders are equal
                                                                            return (0);
                                                                        }
                                                                        else
                                                                        {//assetorder A < assetorder B
                                                                            return (-1);
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {//assetid A < assetid B
                                                                    return (-1);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {//qualifiers A < qualifiers B
                                                            return (-1);
                                                        }
                                                    }
                                                }
                                                else
                                                {// mfrlabel A < mfrlabel B
                                                    return (-1);
                                                }
                                            }
                                        }
                                        else
                                        {// A part < B part
                                            return (-1);
                                        }
                                    }
                                }
                                else
                                {//A pos < B pos
                                    return (-1);
                                }
                            }
                        }
                        else
                        {//  A parttypeid < B parttypeid
                            return (-1);
                        }
                    }
                }
                else
                {//A basevehicleid < B basevehicleid
                    return (-1);
                }
            }
        }
    }

    // for conveying a group of apps to thread divorced from the main UI thread
    // used with "individual app errors" analysis function
    public class analysisChunk
    {
        public int id;
        public string appgroupHashKey;
        public List<App> appsList = new List<App>();
        public List<App> problemAppsList = new List<App>(); // only used in context of macroanalysis
        public string problemsDescription = "";
        public string cachefile;
        public int parttypePositionErrorsCount = 0;
        public int qdbErrorsCount = 0;
        public int questionableNotesCount = 0;
        public int basevehicleidsErrorsCount = 0;
        public int vcdbCodesErrorsCount = 0;
        public int vcdbConfigurationsErrorsCount = 0;
        public int parttypeDisagreementErrorsCount = 0;
        public int assetProblemsCount = 0;
        public int qtyOutlierCount = 0;
        public List<string> problems = new List<string>(); // only used in context of macroanalysis
        public List<string> lowestBadnessPermutation = new List<string>(); // only used in context of macroanalysis
        public bool complete = false;
    }

    // for conveying a collection of chunks to the fitmentLogicProblems analyzer function
    public class analysisChunkGroup
    {
        public int id;
        public List<analysisChunk> chunkList = new List<analysisChunk>();
        public string cachefile;
        public int warningsCount;
        public int errorsCount;
        public bool complete = false;
    }


    public class fitmentNode
    {
        public int nodeId;
        public int parentNode;
        public string fitmentElementType;   // "vcdb" or "note" or "qdb"
        public string fitmentElementData;   // "EngineBase:220" or "10,000lb GVW and up" or "with Distributor in Rear"
        public string fitmentElementString; // this is the human-readable rendered version of the fitment element (L6 2.4L instead of Enginebase:x)
        public List<int> childNodeIds;  // this is in lieu of and external index. it is the list of immediate child node id's
        public List<int> pathFromRoot;  // this is in lieu of and external index. it is the sequential list ancestor nodes that trace a path back to root
        public bool touched; // for various crawl algorythms to mark where it's been
        public int clickType;
        public bool deleted;
        public bool filler; // indicates that this node is a placeholder used in render process.
        public bool markedAsCosmetic; // indicates that the user has deemed this node as cosmetic (not needed). 
        public int graphicalXpos; // for rendering in a graphical chart
        public int graphicalYpos;
        public int graphicalWidth;
        public int graphicalHeight;
        public string logicProblemGroup; // relative group number displayed in the fitment logic problems datagrid. it is stored here for connecting a flagged-by-the-user node back to a cpecific group for reporting
        public App app;

        public bool isComplementaryTo(fitmentNode otherNode)
        {
            if (nodeId == otherNode.nodeId) { return false; }// don't return a match for a node seeing itself in the list
            if (fitmentElementType == otherNode.fitmentElementType) { return true; }
            return false;
        }

        public bool isEqualTo(fitmentNode otherNode)
        {
            if (nodeId == otherNode.nodeId) { return false; }  // don't return a match for a node seeing itself in the list
            if (fitmentElementType == otherNode.fitmentElementType && fitmentElementData == otherNode.fitmentElementData) { return true; }
            return false;
        }

        public string nodeHash()
        {
            //xxx
            string result = "";
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(fitmentElementType+fitmentElementData));
                for (int i = 0; i < data.Length; i++) { result += data[i].ToString("x2"); }
            }
            return result;
        }
    }




    public class productAttribute : IComparable<productAttribute>
    {
        public string PADBid;
        public string PADBYN;
        public string UOM;
        public int recordNumber;
        public string value;

        public int CompareTo(productAttribute other) { return this.recordNumber - other.recordNumber; }
    }


    public class digitalAsset
    {
        public string assetID;
        public string fileName;
        public string assetType;
        public string fileType;
        public string representation;
        public int resolutionDPI;
        public string colorMode;
        public string background;
        public string orientationView;
        public string sssetDimensionsUOM;
        public string sssetDimensionsHeight;
        public string sssetDimensionsWidth;
        public string uri;
    }


    public class packageWeight
    {
        public string UOM;
        public float weight;
    }

    public class package
    {
        public string packageUOM;
        public int quantityofEaches;
        public List<packageWeight> weights;

        public package()
        {
            packageUOM = "";
            quantityofEaches = 1;
            weights = new List<packageWeight>();
            weights.Clear();
        }
    }




    public class PIESitem : IComparable<PIESitem>
    {
        public string action;
        public string partNumber;
        public string itemLevelGTIN;
        public string itemLevelGTINqualifier;
        public string brandID;
        public float minimumOrderQuantity;
        public string minimumOrderQuantityUOM;
        public int PartTerminologyID;
        public List<productAttribute> productAttributes;
        public List<digitalAsset> digitalAssets;
        public List<package> packages;

        public int CompareTo(PIESitem other) { return (string.Compare(this.partNumber, other.partNumber)); }

        public PIESitem()
        {
            productAttributes = new List<productAttribute>();
            productAttributes.Clear();
            digitalAssets = new List<digitalAsset>();
            digitalAssets.Clear();
            packages = new List<package>();
            packages.Clear();
        }


    }




    public class buyersguideApplication : IComparable<buyersguideApplication>
    {
        public string part;
        public string MakeName;
        public string ModelName;
        public int startYear;
        public int endYear;

        public int CompareTo(buyersguideApplication other)
        {// this function is use by Array.Sort() after all apps have been imported from the XML into memory
            int strCmpResults = 0;

            strCmpResults = string.Compare(this.part, other.part);
            if (strCmpResults > 0)
            {//partA > partB
                return (+1);
            }
            else
            {// partA <= partB
                if (strCmpResults == 0)
                {//partA == partB

                    strCmpResults = string.Compare(this.MakeName, other.MakeName);
                    if (strCmpResults > 0)
                    {//makeA > makeB
                        return (+1);
                    }
                    else
                    {//makeA <= makeB
                        if (strCmpResults == 0)
                        {//makeA == makeB
                            strCmpResults = string.Compare(this.ModelName, other.ModelName);
                            if (strCmpResults > 0)
                            {//modelA > modelB
                                return (+1);
                            }
                            else
                            {//modelA <= modelB
                                if (strCmpResults == 0)
                                {
                                    if (this.startYear > other.startYear)
                                    {
                                        return (+1);
                                    }
                                    else
                                    {// startyearA <= startyearB
                                        if (this.startYear == other.startYear)
                                        {
                                            return 0;
                                        }
                                        else
                                        {// startyearA < startyearB
                                            return (-1);
                                        }
                                    }
                                }
                                else
                                {//modelA < modelB
                                    return (-1);
                                }
                            }
                        }
                        else
                        {//makeA < makeB
                            return (-1);
                        }
                    }
                }
                else
                {// partA < partB
                    return (-1);
                }
            }
        }



    }



    public class validationProblem
    {
        public string problemType;
        public string problemData;
    }

    public class BaseVehicle
    {
        public bool valid;
        public string MakeName;
        public int MakeId;
        public string ModelName;
        public int ModelId;
        public string YearName;
        public int Year;
        public string VehicleTypeName;
        public int VehicleTypeId;
        public Dictionary<int, vcdbVehilce> vcdbVehicleDict = new Dictionary<int, vcdbVehilce>();
    }

    public class vcdbVehilce
    {
        public int SubmodelID;
        public int RegionID;
        public int PublicationStageID;
        public List<int> DriveTypeIDlist = new List<int>();
        public List<int> MfrBodyCodeIDlist = new List<int>();
        public List<int> WheelBaseIDlist = new List<int>();
        public List<vcdbBedConfig> BedConfigList = new List<vcdbBedConfig>();
        public List<vcdbBodyStyleConfig> BodyStyleConfigList = new List<vcdbBodyStyleConfig>();
        public List<vcdbBrakeConfig> BrakeConfigList = new List<vcdbBrakeConfig>();
        public List<vcdbEngineConfig> EngineConfigList = new List<vcdbEngineConfig>();
        public List<vcdbSpringTypeConfig> SpringTypeConfigList = new List<vcdbSpringTypeConfig>();
        public List<vcdbSteeringConfig> SteeringConfigList = new List<vcdbSteeringConfig>();
        public List<vcdbTransmission> TransmissionList = new List<vcdbTransmission>();
    }
  
    public class vcdbBedConfig
    {
        public int BedLengthID;
        public int BedTypeID;
    }

    public class vcdbBodyStyleConfig
    {
        public int BodyNumDoorsID;
        public int BodyTypeID;
    }

    public class vcdbBrakeConfig
    {
        public int FrontBrakeTypeID;
        public int RearBrakeTypeID;
        public int BrakeSystemID;
        public int BrakeABSID;
    }

    public class vcdbEngineConfig
    {
        public int EngineBaseID;
        public int EngineDesignationID;
        public int EngineVINID;
        public int ValvesID;
        public int FuelDeliveryTypeID;
        public int FuelDeliverySubTypeID;
        public int FuelSystemControlTypeID;
        public int FuelSystemDesignID;
        public int AspirationID;
        public int CylinderHeadTypeID;
        public int FuelTypeID;
        public int IgnitionSystemTypeID;
        public int EngineMfrID;
        public int EngineVersionID;
        public int PowerOutputID;
        public int EngineBlockID;
        public int EngineBoreStrokeID;
    }



    public class vcdbEngineConfig2
    {
        public int EngineDesignationID;
        public int EngineVINID;
        public int ValvesID;
        public int EngineBaseID;

        public int EngineBlockID;
        public int EngineBoreStrokeID;


        public int FuelDeliveryTypeID;
        public int FuelDeliverySubTypeID;
        public int FuelSystemControlTypeID;
        public int FuelSystemDesignID;
        public int AspirationID;
        public int CylinderHeadTypeID;
        public int FuelTypeID;
        public int IgnitionSystemTypeID;
        public int EngineMfrID;
        public int EngineVersionID;
        public int PowerOutputID;
    }


    public class vcdbSpringTypeConfig
    {
        public int FrontSpringTypeID;
        public int RearSpringTypeID;
    }

    public class vcdbSteeringConfig
    {
        public int SteeringTypeID;
        public int SteeringSystemID;
    }

    public class vcdbTransmission
    {
        public int TransmissionBaseID;
        public int TransmissionTypeID;
        public int TransmissionNumSpeedsID;
        public int TransmissionControlTypeID;
        public int TransmissionMfrCodeID;
        public int TransmissionElecControlledID;
        public int TransmissionMfrID;
    }


    public class VCdbAttribute:IComparable<VCdbAttribute>
    {// each one of these is a name/value pair that lives in an App. ex name="Submidel", value=13. A List of these (plus Notes, plus Qdb qualifiers) makes up the "fitment" for an App
        public string name;
        public int value;

        // comparison method for getting a List<VCdbAttribute> into a sequence that is allowed by the XSD. IMHO the fact that the XSD specifies sequence for vcdb-coded attributes is bullshit. just sayin.
        public int CompareTo(VCdbAttribute other)
        {
            int thisSortScore = 0; int otherSortScore = 0;
            Dictionary<string, int> d = new Dictionary<string, int>()
            {
                {"MfrBodyCode",1},{"BodyNumDoors",2},{"BodyType",3},{"DriveType",4},{"EngineBase",5},{"EngineDesignation",6},
                {"EngineVIN",7},{"EngineVersion",8},{"EngineMfr",9},{"PowerOutput",10},{"ValvesPerEngine",11},{"FuelDeliveryType",12},
                {"FuelDeliverySubType",13},{"FuelSystemControlType",14},{"FuelSystemDesign",15},{"Aspiration",16},{"CylinderHeadType",17},
                {"FuelType",18},{"IgnitionSystemType",19},{"TransmissionMfrCode",20},{"TransmissionBase",21},{"TransmissionType",22},
                {"TransmissionControlType",23},{"TransmissionNumSpeeds",24},{"TransElecControlled",25},{"TransmissionMfr",26},
                {"BedLength",27},{"BedType",28},{"WheelBase",29},{"BrakeSystem",30},{"FrontBrakeType",31},{"RearBrakeType",32},
                {"BrakeABS",33},{"FrontSpringType",34},{"RearSpringType",35},{"SteeringSystem",36},{"SteeringType",37},{"Region",38}}
            ;
            if(d.ContainsKey(this.name)){ thisSortScore = d[this.name];}
            if(d.ContainsKey(other.name)){ otherSortScore = d[other.name];}
            return (thisSortScore-otherSortScore);
        }
    }



    public class QdbQualifier
    {
        public int qualifierId;
        public int qualifierTypeId;
        public string qualifierText;
        public List<string> qualifierParameters;// = new List<string>();
        public QdbQualifier()
        {
            qualifierParameters = new List<string>();
        }

    }




    public class ACES
    {// one of these holds the entire contents of the imported ACES file - along with the results of analysis. The methods for analysis are also here. the rationale for containerizing the data is for future 
        // development that might want to import/analyze/merge/split ACES datasets from/to seperate objects

        public bool successfulImport = false;
        public bool analysisRunning = false;
        public int discardedDeletsOnImport = 0;
        public int analysisTime;
        public string filePath;
        public string fileMD5hash;
        public string version;
        public string Company;
        public string SenderName;
        public string SenderPhone;
        public string TransferDate;
        public string BrandAAIAID;
        public string DocumentTitle;
        public string EffectiveDate;
        public string SubmissionType;
        public string VcdbVersionDate;
        public string QdbVersionDate;
        public string PcdbVersionDate;
        public string differentialsSummary;
        public int FooterRecordCount;
        public bool addedToFitmentPermutationMineingCache = false;
        public Decimal qtyOutlierThreshold;
        public Decimal qtyOutlierSampleSize;
        public bool allowGraceForWildcardConfigs;
        public bool ignoreNAitems;
        public int xmlAppNodeCount;
        public int xmlAssetNodeCount;
        public decimal QdbUtilizationScore;

        public int qtyOutlierCount;
        public int assetProblemsCount;
        public int parttypePositionErrorsCount;
        public int qdbErrorsCount;
        public int questionableNotesCount;
        public int vcdbConfigurationsErrorsCount;
        public int fitmentLogicProblemsCount;
        public int parttypeDisagreementCount;
        public int vcdbCodesErrorsCount;
        public int basevehicleidsErrorsCount;


        public List<string> qtyOutlierCachefilesList = new List<string>();
        public List<string> parttypePositionErrorsCachefilesList = new List<string>();
        public List<string> qdbErrorsCachefilesList = new List<string>();
        public List<string> vcdbConfigurationsErrorsCachefilesList = new List<string>();
        public List<string> fitmentLogicProblemsCachefilesList = new List<string>();
        public List<string> parttypeDisagreementCachefilesList = new List<string>();
        public List<string> vcdbCodesErrorsCachefilesList = new List<string>();
        public List<string> basevehicleidsErrorsCachefilesList = new List<string>();
        

        public List<App> apps = new List<App>();
        public List<Asset> assets = new List<Asset>();

        public Dictionary<string, int> partsAppCounts = new Dictionary<string, int>();
        public Dictionary<string, string> interchange = new Dictionary<string, string>();
        public Dictionary<string, string> assetNameInterchange = new Dictionary<string, string>();
        public Dictionary<string, List<int>> partsPartTypes = new Dictionary<string, List<int>>();
        public Dictionary<string, List<int>> partsPositions = new Dictionary<string, List<int>>();
        public Dictionary<string, int> noteCounts = new Dictionary<string, int>();
        public Dictionary<int, int> basevidOccurrences = new Dictionary<int, int>();
        public Dictionary<int, int> qdbidOccurrences = new Dictionary<int, int>();
        public List<string> distinctAssets = new List<string>();
        public List<string> distinctMfrLabels = new List<string>();
        public List<int> distinctPartTypes = new List<int>();
        public List<String> distinctAssetNames = new List<String>();


        public List<string> differentialParts = new List<string>();
        public List<string> differentialVehicles = new List<string>();
        public List<String> xmlValidationErrors = new List<string>();
        public Dictionary<String, String> ACESschemas = new Dictionary<string, string>();   // define as static so all instances of class can use same dictionary of XSD schema data
        public List<fitmentNode> fitmentNodeList = new List<fitmentNode>();
        public Dictionary<string, List<App>> fitmentProblemGroupsAppLists = new Dictionary<string, List<App>>(); // string key is the MMY/parttype/position/Mfrlabel/Asset/assetitemre/assetitemorder that defines "root" for every fitment tree
        public Dictionary<string, List<String>> fitmentProblemGroupsBestPermutations = new Dictionary<string, List<String>>(); // for keeping track of the least-bad fitment element permutation for each overlap group
        public Dictionary<string, string> appHashesFlaggedAsCosmetic = new Dictionary<string, string>();
        public Dictionary<string, List<string>> fitmentNodesFlaggedAsCosmetic = new Dictionary<string, List<string>>();

        public Dictionary<string, string> fitmentPermutationMiningCache = new Dictionary<string, string>();
        public List<analysisChunk> individualAnanlysisChunksList = new List<analysisChunk>();
        public List<analysisChunk> outlierAnanlysisChunksList = new List<analysisChunk>();
        public List<analysisChunk> fitmentAnalysisChunksList = new List<analysisChunk>(); 
        public List<analysisChunkGroup> fitmentAnalysisChunksGroups = new List<analysisChunkGroup>();
        public Dictionary<string, int> vcdbUsageStatsDict = new Dictionary<string, int>();
        public int vcdbUsageStatsTotalApps=0;
        public List<string> vcdbUsageStatsFileList = new List<string>();
        public Dictionary<String, bool> noteBlacklist = new Dictionary<string, bool>(); // list of dirty words to look for in free-form notes. The bool field indicates "exact match". False in this field causes the search to look for given key anywhere within target. True indicates entire string must match axactly
        public List<string> analysisHistory = new List<string>();
        public int logLevel;
        public bool logToFile;
        
        

        public ACES()
        {// populate XSD's as strings in a string/string dictionary.

            ACESschemas.Add("1.08", "<?xml version =\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\"><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" maxOccurs=\"unbounded\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"xs:string\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"App\"><xs:complexType><xs:sequence><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence></xs:choice><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"TransferCaseBase\" minOccurs=\"0\"/><xs:element ref=\"TransferCase\" minOccurs=\"0\"/><xs:element ref=\"TransferCaseMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"RestraintType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/></xs:sequence><xs:attribute name=\"action\" use=\"required\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"D\"/></xs:restriction></xs:simpleType></xs:attribute><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/></xs:complexType></xs:element><xs:element name=\"ApprovedFor\" type=\"xs:string\"/><xs:element name=\"Aspiration\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BaseVehicle\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BedLength\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BedType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BodyNumDoors\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BodyType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BrakeABS\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BrakeSystem\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"CylinderHeadType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"DisplayOrder\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DriveType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EffectiveDate\" type=\"xs:string\"/><xs:element name=\"EngineBase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EngineDesignation\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EngineMfr\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EngineVIN\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EngineVersion\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element ref=\"RecordCount\"/></xs:sequence></xs:complexType></xs:element><xs:element name=\"FrontBrakeType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FrontSpringType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelDeliverySubType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelDeliveryType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelSystemControlType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelSystemDesign\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element ref=\"Company\"/><xs:element ref=\"SenderName\"/><xs:element ref=\"SenderPhone\"/><xs:element ref=\"SenderPhoneExt\" minOccurs=\"0\"/><xs:element ref=\"TransferDate\"/><xs:element ref=\"MfrCode\" minOccurs=\"0\"/><xs:element ref=\"DocumentTitle\"/><xs:element ref=\"DocFormNumber\" minOccurs=\"0\"/><xs:element ref=\"EffectiveDate\"/><xs:element ref=\"ApprovedFor\" minOccurs=\"0\"/><xs:element ref=\"SubmissionType\"/><xs:element ref=\"MapperCompany\" minOccurs=\"0\"/><xs:element ref=\"MapperContact\" minOccurs=\"0\"/><xs:element ref=\"MapperPhone\" minOccurs=\"0\"/><xs:element ref=\"MapperPhoneExt\" minOccurs=\"0\"/><xs:element ref=\"MapperEmail\" minOccurs=\"0\"/><xs:element ref=\"VcdbVersionDate\"/></xs:sequence></xs:complexType></xs:element><xs:element name=\"IgnitionSystemType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Make\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"MapperCompany\" type=\"xs:string\"/><xs:element name=\"MapperContact\" type=\"xs:string\"/><xs:element name=\"MapperEmail\" type=\"xs:string\"/><xs:element name=\"MapperPhone\" type=\"xs:string\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\"/><xs:element name=\"MfrBodyCode\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"MfrCode\" type=\"xs:string\"/><xs:element name=\"MfrLabel\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Model\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Note\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/><xs:attribute name=\"lang\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"en\"/><xs:enumeration value=\"fr\"/><xs:enumeration value=\"sp\"/></xs:restriction></xs:simpleType></xs:attribute></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Part\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"PartType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Position\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Qty\" type=\"xs:string\"/><xs:element name=\"RearBrakeType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"RearSpringType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"RecordCount\" type=\"xs:string\"/><xs:element name=\"Region\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"RestraintType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\"/><xs:element name=\"SteeringSystem\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"SteeringType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"SubModel\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"SubmissionType\" type=\"xs:string\"/><xs:element name=\"TransferCase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransferCaseBase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransferCaseMfr\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransferDate\" type=\"xs:string\"/><xs:element name=\"TransmissionBase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionControlType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionMfr\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionMfrCode\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionNumSpeeds\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"VcdbVersionDate\" type=\"xs:string\"/><xs:element name=\"VehicleType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"WheelBase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Years\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"to\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element></xs:schema>");
            ACESschemas.Add("2.0", " <?xml version =\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\"><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" maxOccurs=\"unbounded\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"xs:string\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"App\"><xs:complexType><xs:sequence><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence></xs:choice><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice><xs:element ref=\"TransElecContolled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"TransferCaseBase\" minOccurs=\"0\"/><xs:element ref=\"TransferCase\" minOccurs=\"0\"/><xs:element ref=\"TransferCaseMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"RestraintType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/></xs:sequence><xs:attribute name=\"action\" use=\"required\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"D\"/></xs:restriction></xs:simpleType></xs:attribute><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/><xs:attribute name=\"validate\" default=\"yes\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"yes\"/><xs:enumeration value=\"no\"/></xs:restriction></xs:simpleType></xs:attribute></xs:complexType></xs:element><xs:element name=\"ApprovedFor\" type=\"xs:string\"/><xs:element name=\"Aspiration\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BaseVehicle\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BedLength\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BedType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BodyNumDoors\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BodyType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BrakeABS\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"BrakeSystem\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"CylinderHeadType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"DisplayOrder\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DriveType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EffectiveDate\" type=\"xs:string\"/><xs:element name=\"EngineBase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EngineDesignation\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EngineMfr\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EngineVIN\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"EngineVersion\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element ref=\"RecordCount\"/></xs:sequence></xs:complexType></xs:element><xs:element name=\"FrontBrakeType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FrontSpringType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelDeliverySubType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelDeliveryType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelSystemControlType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelSystemDesign\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"FuelType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element ref=\"Company\"/><xs:element ref=\"SenderName\"/><xs:element ref=\"SenderPhone\"/><xs:element ref=\"SenderPhoneExt\" minOccurs=\"0\"/><xs:element ref=\"TransferDate\"/><xs:element ref=\"MfrCode\" minOccurs=\"0\"/><xs:element ref=\"DocumentTitle\"/><xs:element ref=\"DocFormNumber\" minOccurs=\"0\"/><xs:element ref=\"EffectiveDate\"/><xs:element ref=\"ApprovedFor\" minOccurs=\"0\"/><xs:element ref=\"SubmissionType\"/><xs:element ref=\"MapperCompany\" minOccurs=\"0\"/><xs:element ref=\"MapperContact\" minOccurs=\"0\"/><xs:element ref=\"MapperPhone\" minOccurs=\"0\"/><xs:element ref=\"MapperPhoneExt\" minOccurs=\"0\"/><xs:element ref=\"MapperEmail\" minOccurs=\"0\"/><xs:element ref=\"VcdbVersionDate\"/><xs:element ref=\"QdbVersionDate\"/><xs:element ref=\"PcdbVersionDate\"/></xs:sequence></xs:complexType></xs:element><xs:element name=\"IgnitionSystemType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Make\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"MapperCompany\" type=\"xs:string\"/><xs:element name=\"MapperContact\" type=\"xs:string\"/><xs:element name=\"MapperEmail\" type=\"xs:string\"/><xs:element name=\"MapperPhone\" type=\"xs:string\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\"/><xs:element name=\"MfrBodyCode\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"MfrCode\" type=\"xs:string\"/><xs:element name=\"MfrLabel\" type=\"xs:string\"/><xs:element name=\"Model\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Note\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\"/><xs:attribute name=\"lang\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"en\"/><xs:enumeration value=\"fr\"/><xs:enumeration value=\"sp\"/></xs:restriction></xs:simpleType></xs:attribute></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Part\" type=\"xs:string\"/><xs:element name=\"PartType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"PcdbVersionDate\"><xs:complexType/></xs:element><xs:element name=\"Position\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"QdbVersionDate\"><xs:complexType/></xs:element><xs:element name=\"Qty\" type=\"xs:string\"/><xs:element name=\"Qual\"><xs:complexType><xs:sequence><xs:element ref=\"param\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"text\"/></xs:sequence><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"RearBrakeType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"RearSpringType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"RecordCount\" type=\"xs:string\"/><xs:element name=\"Region\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"RestraintType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\"/><xs:element name=\"SteeringSystem\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"SteeringType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"SubModel\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"SubmissionType\" type=\"xs:string\"/><xs:element name=\"TransElecContolled\"><xs:complexType/></xs:element><xs:element name=\"TransferCase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransferCaseBase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransferCaseMfr\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransferDate\" type=\"xs:string\"/><xs:element name=\"TransmissionBase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionControlType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionMfr\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionMfrCode\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionNumSpeeds\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"TransmissionType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"ValvesPerEngine\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"VcdbVersionDate\" type=\"xs:string\"/><xs:element name=\"VehicleType\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"WheelBase\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"Years\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"to\" type=\"xs:string\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"param\"><xs:complexType><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\" type=\"xs:string\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\" type=\"xs:string\"/></xs:extension></xs:simpleContent></xs:complexType></xs:element><xs:element name=\"text\" type=\"xs:string\"/></xs:schema>");
            ACESschemas.Add("3.0", "<?xml version =\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" xml:lang=\"en\" version=\"3.0\"><xs:annotation><xs:documentation>AAIA ACES xml schema version 3.0 for exchanging Automotive Aftermarket catalog application data.(c)2003-2009 AAIA All rights reserved. We do not enforce a default namespace or \"targetNamespace\" with this release to minimize the changes			required to existing instance documents and procedures.		</xs:documentation></xs:annotation><xs:simpleType name=\"acesVersionType\"><xs:annotation><xs:documentation source=\"http://www.xfront.com/Versioning.pdf\">Ties the instance document to a schema version.</xs:documentation></xs:annotation>   <xs:restriction base=\"xs:decimal\"><xs:enumeration value=\"1.0\"/><xs:enumeration value=\"2.0\"/><xs:enumeration value=\"3.0\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"actionType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"D\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetNameType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:minLength value=\"1\"/><xs:maxLength value=\"45\"/>			</xs:restriction></xs:simpleType><xs:simpleType name=\"brandType\"><xs:annotation><xs:documentation source=\"http://www.regular-expressions.info/xmlcharclass.html\">Ideally four uppercase chars without vowels but legacy included some vowels so we exclude just the ones necessary for each character position.</xs:documentation></xs:annotation>		<xs:restriction base=\"xs:string\"><xs:pattern value=\"[B-Z-[EIOU]][B-Z-[EIO]][B-Z-[OU]][A-Z]\"/></xs:restriction></xs:simpleType>	<xs:simpleType name=\"idType\"><xs:restriction base=\"xs:positiveInteger\"/></xs:simpleType><xs:simpleType name=\"partNumberBaseType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"0\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"uomType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"mm\"/><xs:enumeration value=\"cm\"/><xs:enumeration value=\"in\"/><xs:enumeration value=\"ft\"/>			<xs:enumeration value=\"mg\"/><xs:enumeration value=\"g\"/><xs:enumeration value=\"kg\"/><xs:enumeration value=\"oz\"/><xs:enumeration value=\"lb\"/><xs:enumeration value=\"ton\"/></xs:restriction></xs:simpleType> <xs:simpleType name=\"yearType\"><xs:restriction base=\"xs:positiveInteger\"><xs:minInclusive value=\"1896\"/><xs:maxInclusive value=\"2015\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yesnoType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"yes\"/><xs:enumeration value=\"no\"/></xs:restriction></xs:simpleType><xs:complexType name=\"appItemsBaseType\" abstract=\"true\"><xs:sequence><xs:group   ref=\"vehicleIdentGroup\"/><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"PowerOutput\" minOccurs=\"0\"/>			<xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:group   ref=\"transGroup\" minOccurs=\"0\"/><xs:element ref=\"TransElecControlled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/></xs:sequence><xs:attribute name=\"action\"   type=\"actionType\" use=\"required\"/><xs:attribute name=\"id\"       type=\"idType\" use=\"required\"/><xs:attribute name=\"ref\"      type=\"xs:string\"/><xs:attribute name=\"validate\" type=\"yesnoType\" default=\"yes\"/></xs:complexType>	<xs:complexType name=\"appType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"AssetName\"/><xs:element ref=\"AssetItemOrder\" minOccurs=\"0\"/><xs:element ref=\"AssetItemRef\" minOccurs=\"0\"/></xs:sequence>					</xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"assetType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"AssetName\" minOccurs=\"1\"/></xs:sequence></xs:extension></xs:complexContent></xs:complexType>	<xs:complexType name=\"noteType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\"/><xs:attribute name=\"lang\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"en\"/><xs:enumeration value=\"fr\"/><xs:enumeration value=\"sp\"/></xs:restriction></xs:simpleType></xs:attribute></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partNumberType\"><xs:simpleContent><xs:extension base=\"partNumberBaseType\"><xs:attribute name=\"BrandAAIAID\" type=\"brandType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partTypeType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.2\">A Part Type references the primary key in the Parts PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"positionType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.14\">A Position references the primary key in the Position PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"qualType\"><xs:sequence><xs:element name=\"param\" type=\"paramType\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element name=\"text\"  type=\"xs:string\"/></xs:sequence><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:complexType><xs:complexType name=\"paramType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\"    type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\"      type=\"uomType\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\"   type=\"uomType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"vehAttrType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.5\">Vehicle Attributes reference the primary key in the associated VCdb table.</xs:documentation></xs:annotation>	<xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"yearRangeType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" type=\"yearType\" use=\"required\"/><xs:attribute name=\"to\"   type=\"yearType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Asset\" minOccurs=\"0\" maxOccurs=\"unbounded\"/>				<xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"acesVersionType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element name=\"Company\"         type=\"xs:string\"/><xs:element name=\"SenderName\"      type=\"xs:string\"/><xs:element name=\"SenderPhone\"     type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\"  type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"TransferDate\"    type=\"xs:date\"/><xs:element name=\"MfrCode\"         type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"BrandAAIAID\"     type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"DocumentTitle\"   type=\"xs:string\"/><xs:element name=\"DocFormNumber\"   type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\"   type=\"xs:date\"/><xs:element name=\"ApprovedFor\"     type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"SubmissionType\"  type=\"xs:string\"/><xs:element name=\"MapperCompany\"   type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperContact\"   type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhone\"     type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhoneExt\"  type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperEmail\"     type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"VcdbVersionDate\" type=\"xs:date\"/><xs:element name=\"QdbVersionDate\"  type=\"xs:date\"/><xs:element name=\"PcdbVersionDate\" type=\"xs:date\"/></xs:sequence></xs:complexType></xs:element>	<xs:group name=\"vehicleIdentGroup\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.1\">Either a Base Vehicle (which includes a year) or a Make / Year-Range combination must be included with each application.</xs:documentation></xs:annotation>	<xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence></xs:choice>	</xs:group><xs:group name=\"transGroup\"><xs:choice><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:group><xs:element name=\"App\"                     type=\"appType\"/><xs:element name=\"Aspiration\"              type=\"vehAttrType\"/><xs:element name=\"Asset\"                   type=\"assetType\"/><xs:element name=\"AssetItemOrder\"          type=\"xs:positiveInteger\"/><xs:element name=\"AssetItemRef\"            type=\"xs:string\"/>	<xs:element name=\"AssetName\"               type=\"assetNameType\"/><xs:element name=\"BaseVehicle\"             type=\"vehAttrType\"/><xs:element name=\"BedLength\"               type=\"vehAttrType\"/><xs:element name=\"BedType\"                 type=\"vehAttrType\"/><xs:element name=\"BodyNumDoors\"            type=\"vehAttrType\"/><xs:element name=\"BodyType\"                type=\"vehAttrType\"/><xs:element name=\"BrakeABS\"                type=\"vehAttrType\"/><xs:element name=\"BrakeSystem\"             type=\"vehAttrType\"/><xs:element name=\"CylinderHeadType\"        type=\"vehAttrType\"/><xs:element name=\"DisplayOrder\"            type=\"xs:positiveInteger\"/><xs:element name=\"DriveType\"               type=\"vehAttrType\"/><xs:element name=\"EngineBase\"              type=\"vehAttrType\"/><xs:element name=\"EngineDesignation\"       type=\"vehAttrType\"/><xs:element name=\"EngineMfr\"               type=\"vehAttrType\"/><xs:element name=\"EngineVIN\"               type=\"vehAttrType\"/><xs:element name=\"EngineVersion\"           type=\"vehAttrType\"/><xs:element name=\"FrontBrakeType\"          type=\"vehAttrType\"/><xs:element name=\"FrontSpringType\"         type=\"vehAttrType\"/><xs:element name=\"FuelDeliverySubType\"     type=\"vehAttrType\"/><xs:element name=\"FuelDeliveryType\"        type=\"vehAttrType\"/><xs:element name=\"FuelSystemControlType\"   type=\"vehAttrType\"/><xs:element name=\"FuelSystemDesign\"        type=\"vehAttrType\"/><xs:element name=\"FuelType\"                type=\"vehAttrType\"/><xs:element name=\"IgnitionSystemType\"      type=\"vehAttrType\"/><xs:element name=\"Make\"                    type=\"vehAttrType\"/><xs:element name=\"MfrBodyCode\"             type=\"vehAttrType\"/><xs:element name=\"MfrLabel\"                type=\"xs:string\"/><xs:element name=\"Model\"                   type=\"vehAttrType\"/><xs:element name=\"Note\"                    type=\"noteType\"/><xs:element name=\"Part\"                    type=\"partNumberType\"/><xs:element name=\"PartType\"                type=\"partTypeType\"/><xs:element name=\"Position\"                type=\"positionType\"/><xs:element name=\"PowerOutput\"             type=\"vehAttrType\"/>	<xs:element name=\"Qty\"                     type=\"xs:string\"/><xs:element name=\"Qual\"                    type=\"qualType\"/><xs:element name=\"RearBrakeType\"           type=\"vehAttrType\"/><xs:element name=\"RearSpringType\"          type=\"vehAttrType\"/><xs:element name=\"Region\"                  type=\"vehAttrType\"/><xs:element name=\"SteeringSystem\"          type=\"vehAttrType\"/><xs:element name=\"SteeringType\"            type=\"vehAttrType\"/><xs:element name=\"SubModel\"                type=\"vehAttrType\"/><xs:element name=\"TransElecControlled\"     type=\"vehAttrType\"/><xs:element name=\"TransferDate\"            type=\"xs:date\"/><xs:element name=\"TransmissionBase\"        type=\"vehAttrType\"/><xs:element name=\"TransmissionControlType\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfr\"         type=\"vehAttrType\"/><xs:element name=\"TransmissionMfrCode\"     type=\"vehAttrType\"/><xs:element name=\"TransmissionNumSpeeds\"   type=\"vehAttrType\"/><xs:element name=\"TransmissionType\"        type=\"vehAttrType\"/><xs:element name=\"ValvesPerEngine\"         type=\"vehAttrType\"/><xs:element name=\"VehicleType\"             type=\"vehAttrType\"/><xs:element name=\"WheelBase\"               type=\"vehAttrType\"/><xs:element name=\"Years\"                   type=\"yearRangeType\"/><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element name=\"RecordCount\" type=\"xs:string\"/></xs:sequence></xs:complexType></xs:element></xs:schema>");
            ACESschemas.Add("3.0.1", "<? xml version =\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" version=\"3.0.1\" xml:lang=\"en\"><xs:annotation><xs:documentation>AAIA ACES xml schema version 3.0.1 for exchanging Automotive Aftermarket catalog application data.	(c)2003-2013 AAIA All rights reserved.	We do not enforce a default namespace or \"targetNamespace\" with this release to minimize the changes	required to existing instance documents and procedures.</xs:documentation></xs:annotation><xs:simpleType name=\"acesVersionType\"><xs:annotation><xs:documentation source=\"http://www.xfront.com/Versioning.pdf\">Ties the instance document to a schema version.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"1.0\"/><xs:enumeration value=\"2.0\"/><xs:enumeration value=\"3.0\"/><xs:enumeration value=\"3.0.1\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"actionType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"D\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetNameType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:minLength value=\"1\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"brandType\"><xs:annotation><xs:documentation source=\"http://www.regular-expressions.info/xmlcharclass.html\">Ideally four uppercase chars without vowels but legacy included some vowels so we	exclude just the ones necessary for each character position.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:pattern value=\"[B-Z-[EIOU]][B-Z-[EIO]][B-Z-[OU]][A-Z]\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"idType\"><xs:restriction base=\"xs:positiveInteger\"/></xs:simpleType><xs:simpleType name=\"partNumberBaseType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"0\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"uomType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"mm\"/><xs:enumeration value=\"cm\"/><xs:enumeration value=\"in\"/><xs:enumeration value=\"ft\"/><xs:enumeration value=\"mg\"/><xs:enumeration value=\"g\"/><xs:enumeration value=\"kg\"/><xs:enumeration value=\"oz\"/><xs:enumeration value=\"lb\"/><xs:enumeration value=\"ton\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yearType\"><xs:restriction base=\"xs:positiveInteger\"><xs:minInclusive value=\"1896\"/><xs:maxInclusive value=\"2015\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yesnoType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"yes\"/><xs:enumeration value=\"no\"/></xs:restriction></xs:simpleType><xs:complexType name=\"appItemsBaseType\" abstract=\"true\"><xs:sequence><xs:group ref=\"vehicleIdentGroup\"/><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"PowerOutput\" minOccurs=\"0\"/><xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:group ref=\"transGroup\" minOccurs=\"0\"/><xs:element ref=\"TransElecControlled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/></xs:sequence><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/><xs:attribute name=\"validate\" type=\"yesnoType\" default=\"yes\"/></xs:complexType><xs:complexType name=\"appType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"AssetName\"/><xs:element ref=\"AssetItemOrder\" minOccurs=\"0\"/><xs:element ref=\"AssetItemRef\" minOccurs=\"0\"/></xs:sequence></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"assetType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"AssetName\" minOccurs=\"1\"/></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"noteType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\"/><xs:attribute name=\"lang\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"en\"/><xs:enumeration value=\"fr\"/><xs:enumeration value=\"sp\"/></xs:restriction></xs:simpleType></xs:attribute></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partNumberType\"><xs:simpleContent><xs:extension base=\"partNumberBaseType\"><xs:attribute name=\"BrandAAIAID\" type=\"brandType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partTypeType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.2\">A Part Type references the primary key in the Parts PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"positionType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.14\">A Position references the primary key in the Position PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"qualType\"><xs:sequence><xs:element name=\"param\" type=\"paramType\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element name=\"text\" type=\"xs:string\"/></xs:sequence><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:complexType><xs:complexType name=\"paramType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\" type=\"uomType\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\" type=\"uomType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"vehAttrType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.5\">Vehicle Attributes reference the primary key in the associated VCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"yearRangeType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" type=\"yearType\" use=\"required\"/><xs:attribute name=\"to\" type=\"yearType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Asset\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"acesVersionType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"MfrCode\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"BrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\"/><xs:element name=\"ApprovedFor\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"SubmissionType\" type=\"xs:string\"/><xs:element name=\"MapperCompany\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperContact\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhone\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperEmail\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"VcdbVersionDate\" type=\"xs:date\"/><xs:element name=\"QdbVersionDate\" type=\"xs:date\"/><xs:element name=\"PcdbVersionDate\" type=\"xs:date\"/></xs:sequence></xs:complexType></xs:element><xs:group name=\"vehicleIdentGroup\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.1\">Either a Base Vehicle (which includes a year) or a Make / Year-Range combination	must be included with each application. </xs:documentation></xs:annotation><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence></xs:choice></xs:group><xs:group name=\"transGroup\"><xs:choice><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:group><xs:element name=\"App\" type=\"appType\"/><xs:element name=\"Aspiration\" type=\"vehAttrType\"/><xs:element name=\"Asset\" type=\"assetType\"/><xs:element name=\"AssetItemOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"AssetItemRef\" type=\"xs:string\"/><xs:element name=\"AssetName\" type=\"assetNameType\"/><xs:element name=\"BaseVehicle\" type=\"vehAttrType\"/><xs:element name=\"BedLength\" type=\"vehAttrType\"/><xs:element name=\"BedType\" type=\"vehAttrType\"/><xs:element name=\"BodyNumDoors\" type=\"vehAttrType\"/><xs:element name=\"BodyType\" type=\"vehAttrType\"/><xs:element name=\"BrakeABS\" type=\"vehAttrType\"/><xs:element name=\"BrakeSystem\" type=\"vehAttrType\"/><xs:element name=\"CylinderHeadType\" type=\"vehAttrType\"/><xs:element name=\"DisplayOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"DriveType\" type=\"vehAttrType\"/><xs:element name=\"EngineBase\" type=\"vehAttrType\"/><xs:element name=\"EngineDesignation\" type=\"vehAttrType\"/><xs:element name=\"EngineMfr\" type=\"vehAttrType\"/><xs:element name=\"EngineVIN\" type=\"vehAttrType\"/><xs:element name=\"EngineVersion\" type=\"vehAttrType\"/><xs:element name=\"FrontBrakeType\" type=\"vehAttrType\"/><xs:element name=\"FrontSpringType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliverySubType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliveryType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemControlType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemDesign\" type=\"vehAttrType\"/><xs:element name=\"FuelType\" type=\"vehAttrType\"/><xs:element name=\"IgnitionSystemType\" type=\"vehAttrType\"/><xs:element name=\"Make\" type=\"vehAttrType\"/><xs:element name=\"MfrBodyCode\" type=\"vehAttrType\"/><xs:element name=\"MfrLabel\" type=\"xs:string\"/><xs:element name=\"Model\" type=\"vehAttrType\"/><xs:element name=\"Note\" type=\"noteType\"/><xs:element name=\"Part\" type=\"partNumberType\"/><xs:element name=\"PartType\" type=\"partTypeType\"/><xs:element name=\"Position\" type=\"positionType\"/><xs:element name=\"PowerOutput\" type=\"vehAttrType\"/><xs:element name=\"Qty\" type=\"xs:string\"/><xs:element name=\"Qual\" type=\"qualType\"/><xs:element name=\"RearBrakeType\" type=\"vehAttrType\"/><xs:element name=\"RearSpringType\" type=\"vehAttrType\"/><xs:element name=\"Region\" type=\"vehAttrType\"/><xs:element name=\"SteeringSystem\" type=\"vehAttrType\"/><xs:element name=\"SteeringType\" type=\"vehAttrType\"/><xs:element name=\"SubModel\" type=\"vehAttrType\"/><xs:element name=\"TransElecControlled\" type=\"vehAttrType\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"TransmissionBase\" type=\"vehAttrType\"/><xs:element name=\"TransmissionControlType\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfr\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfrCode\" type=\"vehAttrType\"/><xs:element name=\"TransmissionNumSpeeds\" type=\"vehAttrType\"/><xs:element name=\"TransmissionType\" type=\"vehAttrType\"/><xs:element name=\"ValvesPerEngine\" type=\"vehAttrType\"/><xs:element name=\"VehicleType\" type=\"vehAttrType\"/><xs:element name=\"WheelBase\" type=\"vehAttrType\"/><xs:element name=\"Years\" type=\"yearRangeType\"/><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element name=\"RecordCount\" type=\"xs:string\"/></xs:sequence></xs:complexType></xs:element></xs:schema>");
            ACESschemas.Add("3.1", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" version=\"3.1\" xml:lang=\"en\"><xs:annotation><xs:documentation>AAIA ACES xml schema version 3.1 for exchanging Automotive Aftermarket catalog application data.	(c)2003-2013 AAIA All rights reserved.	We do not enforce a default namespace or \"targetNamespace\" with this release to minimize the changes	required to existing instance documents and procedures.</xs:documentation></xs:annotation><xs:simpleType name=\"acesVersionType\"><xs:annotation><xs:documentation source=\"http://www.xfront.com/Versioning.pdf\">Ties the instance document to a schema version.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"1.0\"/><xs:enumeration value=\"2.0\"/><xs:enumeration value=\"3.0\"/><xs:enumeration value=\"3.0.1\"/><xs:enumeration value=\"3.1\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"actionType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"D\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetNameType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:minLength value=\"1\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"brandType\"><xs:annotation><xs:documentation source=\"http://www.regular-expressions.info/xmlcharclass.html\">Ideally four uppercase chars without vowels but legacy included some vowels so we	exclude just the ones necessary for each character position.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:pattern value=\"[B-Z-[EIOU]][B-Z-[EIO]][B-Z-[OU]][A-Z]\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"idType\"><xs:restriction base=\"xs:positiveInteger\"/></xs:simpleType><xs:simpleType name=\"partNumberBaseType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"0\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"uomType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"mm\"/><xs:enumeration value=\"cm\"/><xs:enumeration value=\"in\"/><xs:enumeration value=\"ft\"/><xs:enumeration value=\"mg\"/><xs:enumeration value=\"g\"/><xs:enumeration value=\"kg\"/><xs:enumeration value=\"oz\"/><xs:enumeration value=\"lb\"/><xs:enumeration value=\"ton\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yearType\"><xs:restriction base=\"xs:positiveInteger\"><xs:minInclusive value=\"1896\"/><xs:maxInclusive value=\"2015\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yesnoType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"yes\"/><xs:enumeration value=\"no\"/></xs:restriction></xs:simpleType><xs:complexType name=\"appItemsBaseType\" abstract=\"true\"><xs:sequence><xs:group ref=\"vehicleIdentGroup\"/><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"PowerOutput\" minOccurs=\"0\"/><xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:group ref=\"transGroup\" minOccurs=\"0\"/><xs:element ref=\"TransElecControlled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/></xs:sequence><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/><xs:attribute name=\"validate\" type=\"yesnoType\" default=\"yes\"/></xs:complexType><xs:complexType name=\"appType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"AssetName\"/><xs:element ref=\"AssetItemOrder\" minOccurs=\"0\"/><xs:element ref=\"AssetItemRef\" minOccurs=\"0\"/></xs:sequence></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"assetType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"AssetName\" minOccurs=\"1\"/></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"noteType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\"/><xs:attribute name=\"lang\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"en\"/><xs:enumeration value=\"fr\"/><xs:enumeration value=\"sp\"/></xs:restriction></xs:simpleType></xs:attribute></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partNumberType\"><xs:simpleContent><xs:extension base=\"partNumberBaseType\"><xs:attribute name=\"BrandAAIAID\" type=\"brandType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partTypeType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.2\">A Part Type references the primary key in the Parts PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"positionType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.14\">A Position references the primary key in the Position PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"qualType\"><xs:sequence><xs:element name=\"param\" type=\"paramType\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element name=\"text\" type=\"xs:string\"/></xs:sequence><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:complexType><xs:complexType name=\"paramType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\" type=\"uomType\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\" type=\"uomType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"vehAttrType\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.5\">Vehicle Attributes reference the primary key in the associated VCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"yearRangeType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" type=\"yearType\" use=\"required\"/><xs:attribute name=\"to\" type=\"yearType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Asset\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"acesVersionType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"MfrCode\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"BrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\"/><xs:element name=\"ApprovedFor\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"SubmissionType\" type=\"xs:string\"/><xs:element name=\"MapperCompany\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperContact\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhone\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperEmail\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"VcdbVersionDate\" type=\"xs:date\"/><xs:element name=\"QdbVersionDate\" type=\"xs:date\"/><xs:element name=\"PcdbVersionDate\" type=\"xs:date\"/></xs:sequence></xs:complexType></xs:element><xs:group name=\"vehicleIdentGroup\"><xs:annotation><xs:documentation source=\"http://www.aftermarket.org/aces3.0/#section_5.7.1\">Either a Base Vehicle (which includes a year) or a Make / Year-Range combination	must be included with each application. </xs:documentation></xs:annotation><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence></xs:choice></xs:group><xs:group name=\"transGroup\"><xs:choice><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:group><xs:element name=\"App\" type=\"appType\"/><xs:element name=\"Aspiration\" type=\"vehAttrType\"/><xs:element name=\"Asset\" type=\"assetType\"/><xs:element name=\"AssetItemOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"AssetItemRef\" type=\"xs:string\"/><xs:element name=\"AssetName\" type=\"assetNameType\"/><xs:element name=\"BaseVehicle\" type=\"vehAttrType\"/><xs:element name=\"BedLength\" type=\"vehAttrType\"/><xs:element name=\"BedType\" type=\"vehAttrType\"/><xs:element name=\"BodyNumDoors\" type=\"vehAttrType\"/><xs:element name=\"BodyType\" type=\"vehAttrType\"/><xs:element name=\"BrakeABS\" type=\"vehAttrType\"/><xs:element name=\"BrakeSystem\" type=\"vehAttrType\"/><xs:element name=\"CylinderHeadType\" type=\"vehAttrType\"/><xs:element name=\"DisplayOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"DriveType\" type=\"vehAttrType\"/><xs:element name=\"EngineBase\" type=\"vehAttrType\"/><xs:element name=\"EngineDesignation\" type=\"vehAttrType\"/><xs:element name=\"EngineMfr\" type=\"vehAttrType\"/><xs:element name=\"EngineVIN\" type=\"vehAttrType\"/><xs:element name=\"EngineVersion\" type=\"vehAttrType\"/><xs:element name=\"FrontBrakeType\" type=\"vehAttrType\"/><xs:element name=\"FrontSpringType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliverySubType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliveryType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemControlType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemDesign\" type=\"vehAttrType\"/><xs:element name=\"FuelType\" type=\"vehAttrType\"/><xs:element name=\"IgnitionSystemType\" type=\"vehAttrType\"/><xs:element name=\"Make\" type=\"vehAttrType\"/><xs:element name=\"MfrBodyCode\" type=\"vehAttrType\"/><xs:element name=\"MfrLabel\" type=\"xs:string\"/><xs:element name=\"Model\" type=\"vehAttrType\"/><xs:element name=\"Note\" type=\"noteType\"/><xs:element name=\"Part\" type=\"partNumberType\"/><xs:element name=\"PartType\" type=\"partTypeType\"/><xs:element name=\"Position\" type=\"positionType\"/><xs:element name=\"PowerOutput\" type=\"vehAttrType\"/><xs:element name=\"Qty\" type=\"xs:string\"/><xs:element name=\"Qual\" type=\"qualType\"/><xs:element name=\"RearBrakeType\" type=\"vehAttrType\"/><xs:element name=\"RearSpringType\" type=\"vehAttrType\"/><xs:element name=\"Region\" type=\"vehAttrType\"/><xs:element name=\"SteeringSystem\" type=\"vehAttrType\"/><xs:element name=\"SteeringType\" type=\"vehAttrType\"/><xs:element name=\"SubModel\" type=\"vehAttrType\"/><xs:element name=\"TransElecControlled\" type=\"vehAttrType\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"TransmissionBase\" type=\"vehAttrType\"/><xs:element name=\"TransmissionControlType\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfr\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfrCode\" type=\"vehAttrType\"/><xs:element name=\"TransmissionNumSpeeds\" type=\"vehAttrType\"/><xs:element name=\"TransmissionType\" type=\"vehAttrType\"/><xs:element name=\"ValvesPerEngine\" type=\"vehAttrType\"/><xs:element name=\"VehicleType\" type=\"vehAttrType\"/><xs:element name=\"WheelBase\" type=\"vehAttrType\"/><xs:element name=\"Years\" type=\"yearRangeType\"/><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element name=\"RecordCount\" type=\"xs:string\"/></xs:sequence></xs:complexType></xs:element></xs:schema>");
            ACESschemas.Add("3.2", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" version=\"3.2\" xml:lang=\"en\"><xs:annotation><xs:documentation>Auto Care Assocation ACES xml schema version 3.2 for exchanging catalog application data.	(c)2003-2016 Auto Care Assocation All rights reserved.	We do not enforce a default namespace or \"targetNamespace\" with this release to minimize the changes	required to existing instance documents and procedures.</xs:documentation></xs:annotation><!-- simple type definitions --><xs:simpleType name=\"acesVersionType\"><xs:annotation><xs:documentation source=\"http://www.xfront.com/Versioning.pdf\">Ties the instance document to a schema version.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"1.0\"/><xs:enumeration value=\"2.0\"/><xs:enumeration value=\"3.0\"/><xs:enumeration value=\"3.0.1\"/><xs:enumeration value=\"3.1\"/><xs:enumeration value=\"3.2\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"actionType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"D\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetNameType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:minLength value=\"1\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"brandType\"><xs:annotation><xs:documentation source=\"http://www.regular-expressions.info/xmlcharclass.html\">Ideally four uppercase chars without vowels but legacy included some vowels so we	exclude just the ones necessary for each character position.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:pattern value=\"[B-Z-[EIOU]][B-Z-[EIO]][B-Z-[OU]][A-Z]\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"idType\"><xs:restriction base=\"xs:positiveInteger\"/></xs:simpleType><xs:simpleType name=\"partNumberBaseType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"0\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"uomType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"mm\"/><xs:enumeration value=\"cm\"/><xs:enumeration value=\"in\"/><xs:enumeration value=\"ft\"/><xs:enumeration value=\"mg\"/><xs:enumeration value=\"g\"/><xs:enumeration value=\"kg\"/><xs:enumeration value=\"oz\"/><xs:enumeration value=\"lb\"/><xs:enumeration value=\"ton\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yearType\"><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"4\"/><xs:minInclusive value=\"1896\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yesnoType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"yes\"/><xs:enumeration value=\"no\"/></xs:restriction></xs:simpleType><!-- complex type definitions --><xs:complexType name=\"appItemsBaseType\" abstract=\"true\"><xs:sequence><xs:group ref=\"vehicleIdentGroup\"/><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"PowerOutput\" minOccurs=\"0\"/><xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:group ref=\"transGroup\" minOccurs=\"0\"/><xs:element ref=\"TransElecControlled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/></xs:sequence><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/><xs:attribute name=\"validate\" type=\"yesnoType\" default=\"yes\"/></xs:complexType><xs:complexType name=\"appType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"AssetName\"/><xs:element ref=\"AssetItemOrder\" minOccurs=\"0\"/><xs:element ref=\"AssetItemRef\" minOccurs=\"0\"/></xs:sequence></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"assetType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"/></xs:complexContent></xs:complexType><xs:complexType name=\"noteType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\"/><xs:attribute name=\"lang\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"en\"/><xs:enumeration value=\"fr\"/><xs:enumeration value=\"sp\"/></xs:restriction></xs:simpleType></xs:attribute></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partNumberType\"><xs:simpleContent><xs:extension base=\"partNumberBaseType\"><xs:attribute name=\"BrandAAIAID\" type=\"brandType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partTypeType\"><xs:annotation><xs:documentation>A Part Type references the primary key in the Parts PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"positionType\"><xs:annotation><xs:documentation>A Position references the primary key in the Position PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"qualType\"><xs:sequence><xs:element name=\"param\" type=\"paramType\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element name=\"text\" type=\"xs:string\"/></xs:sequence><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:complexType><xs:complexType name=\"paramType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\" type=\"uomType\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\" type=\"uomType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"vehAttrType\"><xs:annotation><xs:documentation>Vehicle Attributes reference the primary key in the associated VCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"yearRangeType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" use=\"required\"><xs:simpleType><xs:restriction base=\"yearType\"/></xs:simpleType></xs:attribute><xs:attribute name=\"to\" type=\"yearType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><!-- document structure --><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Asset\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"DigitalAsset\" minOccurs=\"0\" maxOccurs=\"1\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"acesVersionType\" use=\"required\"/></xs:complexType></xs:element><!-- \"Header\" element definition --><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"MfrCode\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"BrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\"/><xs:element name=\"ApprovedFor\" type=\"approvedForType\" minOccurs=\"0\"/><xs:element name=\"SubmissionType\" type=\"xs:string\"/><xs:element name=\"MapperCompany\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperContact\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhone\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperEmail\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"VcdbVersionDate\" type=\"xs:date\"/><xs:element name=\"QdbVersionDate\" type=\"xs:date\"/><xs:element name=\"PcdbVersionDate\" type=\"xs:date\"/></xs:sequence></xs:complexType></xs:element><!-- Vehicle Identification Group definition --><xs:group name=\"vehicleIdentGroup\"><xs:annotation><xs:documentation>Either a Base Vehicle (which includes a year) or a Make / Year-Range combination	must be included with each application. </xs:documentation></xs:annotation><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence></xs:choice></xs:group><!-- Transmission Group dfinition --><xs:group name=\"transGroup\"><xs:choice><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:group><!-- element definitions  --><xs:element name=\"App\" type=\"appType\"/><xs:element name=\"Aspiration\" type=\"vehAttrType\"/><xs:element name=\"Asset\"><xs:complexType><xs:complexContent><xs:extension base=\"assetType\"><xs:sequence><xs:element ref=\"AssetName\"/></xs:sequence></xs:extension></xs:complexContent></xs:complexType></xs:element><xs:element name=\"AssetItemOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"AssetItemRef\" type=\"xs:string\"/><xs:element name=\"AssetName\" type=\"assetNameType\"/><xs:element name=\"BaseVehicle\" type=\"vehAttrType\"/><xs:element name=\"BedLength\" type=\"vehAttrType\"/><xs:element name=\"BedType\" type=\"vehAttrType\"/><xs:element name=\"BodyNumDoors\" type=\"vehAttrType\"/><xs:element name=\"BodyType\" type=\"vehAttrType\"/><xs:element name=\"BrakeABS\" type=\"vehAttrType\"/><xs:element name=\"BrakeSystem\" type=\"vehAttrType\"/><xs:element name=\"CylinderHeadType\" type=\"vehAttrType\"/><xs:element name=\"DisplayOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"DriveType\" type=\"vehAttrType\"/><xs:element name=\"EngineBase\" type=\"vehAttrType\"/><xs:element name=\"EngineDesignation\" type=\"vehAttrType\"/><xs:element name=\"EngineMfr\" type=\"vehAttrType\"/><xs:element name=\"EngineVIN\" type=\"vehAttrType\"/><xs:element name=\"EngineVersion\" type=\"vehAttrType\"/><xs:element name=\"FrontBrakeType\" type=\"vehAttrType\"/><xs:element name=\"FrontSpringType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliverySubType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliveryType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemControlType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemDesign\" type=\"vehAttrType\"/><xs:element name=\"FuelType\" type=\"vehAttrType\"/><xs:element name=\"IgnitionSystemType\" type=\"vehAttrType\"/><xs:element name=\"Make\" type=\"vehAttrType\"/><xs:element name=\"MfrBodyCode\" type=\"vehAttrType\"/><xs:element name=\"MfrLabel\" type=\"xs:string\"/><xs:element name=\"Model\" type=\"vehAttrType\"/><xs:element name=\"Note\" type=\"noteType\"/><xs:element name=\"Part\" type=\"partNumberType\"/><xs:element name=\"PartType\" type=\"partTypeType\"/><xs:element name=\"Position\" type=\"positionType\"/><xs:element name=\"PowerOutput\" type=\"vehAttrType\"/><xs:element name=\"Qty\" type=\"xs:string\"/><xs:element name=\"Qual\" type=\"qualType\"/><xs:element name=\"RearBrakeType\" type=\"vehAttrType\"/><xs:element name=\"RearSpringType\" type=\"vehAttrType\"/><xs:element name=\"Region\" type=\"vehAttrType\"/><xs:element name=\"SteeringSystem\" type=\"vehAttrType\"/><xs:element name=\"SteeringType\" type=\"vehAttrType\"/><xs:element name=\"SubModel\" type=\"vehAttrType\"/><xs:element name=\"TransElecControlled\" type=\"vehAttrType\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"TransmissionBase\" type=\"vehAttrType\"/><xs:element name=\"TransmissionControlType\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfr\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfrCode\" type=\"vehAttrType\"/><xs:element name=\"TransmissionNumSpeeds\" type=\"vehAttrType\"/><xs:element name=\"TransmissionType\" type=\"vehAttrType\"/><xs:element name=\"ValvesPerEngine\" type=\"vehAttrType\"/><xs:element name=\"VehicleType\" type=\"vehAttrType\"/><xs:element name=\"WheelBase\" type=\"vehAttrType\"/><xs:element name=\"Years\" type=\"yearRangeType\"/><xs:complexType name=\"approvedForType\"><xs:sequence><xs:element name=\"Country\" maxOccurs=\"unbounded\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence></xs:complexType><xs:element name=\"DigitalAsset\"><xs:complexType><xs:sequence><xs:element name=\"DigitalFileInformation\" type=\"digitalFileInformationType\" minOccurs=\"1\" maxOccurs=\"unbounded\"/></xs:sequence></xs:complexType></xs:element><xs:complexType name=\"digitalFileInformationType\"><xs:sequence><xs:element name=\"FileName\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetDetailType\" type=\"assetDetailType\"/><xs:element name=\"FileType\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"assetFileType\"><xs:maxLength value=\"4\"/><xs:minLength value=\"3\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Representation\" type=\"representationType\" minOccurs=\"0\"/><xs:element name=\"FileSize\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"10\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Resolution\" type=\"resolutionType\" minOccurs=\"0\"/><xs:element name=\"ColorMode\" type=\"colorModeType\" minOccurs=\"0\"/><xs:element name=\"Background\" type=\"backgroundType\" minOccurs=\"0\"/><xs:element name=\"OrientationView\" type=\"orientationViewType\" minOccurs=\"0\"/><xs:element name=\"AssetDimensions\" minOccurs=\"0\"><xs:complexType><xs:sequence><xs:element name=\"AssetHeight\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetWidth\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"UOM\" type=\"dimensionUOMType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"AssetDescription\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"FilePath\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"URI\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:anyURI\"><xs:maxLength value=\"2000\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"FileDateModified\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"ExpirationDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"Country\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"AssetName\" use=\"required\"/><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"LanguageCode\" type=\"xs:string\"/></xs:complexType><xs:simpleType name=\"assetDetailType\"><xs:annotation><xs:documentation>Code	Description	360		360 Degree Image Set	APG		Application Guide	AUD		Audio File	BRO		Brochure	BUL		Technical Bulletin	BUY		Buyers Guide	CAS		Case Study	CAT		Catalog	CER		Certificate of Origin	DAS		Datasheet	DRW	Technical Drawing	EBK		Ebook	FAB		Features and Benefits	FED		Full Engineering Drawing 	HMS		Hazardous Materials Info Sheet	INS		Installation Instructions	ISG		Illustration Guide	LIN		Line Art	LGO		Logo Image	MSD		Material Safety Data Sheet	OWN	Owner's Manual	P01		Photo – out of package	P02		Photo – in package	P03		Photo – lifestyle view	P04		Photo - Primary	P05		Photo - Close Up	P06		Photo - Mounted	P07		Photo - Unmounted	PAG		Link To Manufacturer Page	PAL		Pallet Configuration Drawing	PDB		Product Brochure	PC1		Planogram Consumer Pack 1	PC2		Planogram Consumer Pack 2	PC3		Planogram Consumer Pack 3	PI1		Planogram Inner Pack 1	PI2		Planogram Inner Pack 2	PI3		Planogram Inner Pack 3	PP1		Planogram Case Pack 1	PP2		Planogram Case Pack 2	PP3		Planogram Case Pack 3	PSS		Product Specifications Sheet	PST		Price Sheet	RES		Research Bulletin	SPE		Specification Sheet Filename 	THU		Thumbnail	TON		Tone Art	WAR	Warranty	MHP		Whitepaper	ZZ1	User 1	ZZ2	User 2	ZZ3	User 3	ZZ4	User 4	ZZ5	User 5	ZZ6	User 6	ZZ7	User 7	ZZ8	User 8	ZZ9	User 9</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"360\"/><xs:enumeration value=\"APG\"/><xs:enumeration value=\"AUD\"/><xs:enumeration value=\"BRO\"/><xs:enumeration value=\"BUL\"/><xs:enumeration value=\"BUY\"/><xs:enumeration value=\"CAS\"/><xs:enumeration value=\"CAT\"/><xs:enumeration value=\"CER\"/><xs:enumeration value=\"DAS\"/><xs:enumeration value=\"DRW\"/><xs:enumeration value=\"EBK\"/><xs:enumeration value=\"FAB\"/><xs:enumeration value=\"FED\"/><xs:enumeration value=\"HMS\"/><xs:enumeration value=\"INS\"/><xs:enumeration value=\"ISG\"/><xs:enumeration value=\"LIN\"/><xs:enumeration value=\"LGO\"/><xs:enumeration value=\"MSD\"/><xs:enumeration value=\"OWN\"/><xs:enumeration value=\"P01\"/><xs:enumeration value=\"P02\"/><xs:enumeration value=\"P03\"/><xs:enumeration value=\"P04\"/><xs:enumeration value=\"P05\"/><xs:enumeration value=\"P06\"/><xs:enumeration value=\"P07\"/><xs:enumeration value=\"PAG\"/><xs:enumeration value=\"PAL\"/><xs:enumeration value=\"PDB\"/><xs:enumeration value=\"PC1\"/><xs:enumeration value=\"PC2\"/><xs:enumeration value=\"PC3\"/><xs:enumeration value=\"PI1\"/><xs:enumeration value=\"PI2\"/><xs:enumeration value=\"PI3\"/><xs:enumeration value=\"PP1\"/><xs:enumeration value=\"PP2\"/><xs:enumeration value=\"PP3\"/><xs:enumeration value=\"PSS\"/><xs:enumeration value=\"PST\"/><xs:enumeration value=\"RES\"/><xs:enumeration value=\"SPE\"/><xs:enumeration value=\"THU\"/><xs:enumeration value=\"TON\"/><xs:enumeration value=\"WAR\"/><xs:enumeration value=\"WHP\"/><xs:enumeration value=\"ZZ1\"/><xs:enumeration value=\"ZZ2\"/><xs:enumeration value=\"ZZ3\"/><xs:enumeration value=\"ZZ4\"/><xs:enumeration value=\"ZZ5\"/><xs:enumeration value=\"ZZ6\"/><xs:enumeration value=\"ZZ7\"/><xs:enumeration value=\"ZZ8\"/><xs:enumeration value=\"ZZ9\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetFileType\"><xs:annotation><xs:documentation>Code	Description	TIF		Tagged Image File	JPG		Joint Photographic Experts Group	EPS		Encapsulated PostScript	TXT		.txt TEXT FILE	FLV		.flv VIDEO FILE	F4V		.f4v VIDEO FILE	AVI		.avi VIDEO FILE	WEBM	.webm VIDEO FILE	OGV		.ogv VIDEO VILE	MP4		.mp4 VIDEO FILE	MKV		.mkv VIDEO FILE	AIF		.aif AUDIO FILE	WAV	.wav AUDIO FILE	WMA	.wma AUDIO FILE	OGG	.ogg AUDIO FILE	PCM		.pcm AUDIO FILE	AC3		.ac3 AUDIO FILE	MIDI		.mid AUDIO FILE	MP3		.mp3 AUDIO FILE	AAC		.aac AUDIO FILE	GIF		Graphics Interchange Format	BMP		Bitmap Image	PNG		Portable Network Graphics	PDF		Portable Document Format	DOC		MS Word	XLS		MS Excel</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"TIF\"/><xs:enumeration value=\"JPG\"/><xs:enumeration value=\"EPS\"/><xs:enumeration value=\"TXT\"/><xs:enumeration value=\"FLV\"/><xs:enumeration value=\"F4V\"/><xs:enumeration value=\"AVI\"/><xs:enumeration value=\"WEBM\"/><xs:enumeration value=\"OGV\"/><xs:enumeration value=\"MP4\"/><xs:enumeration value=\"MKV\"/><xs:enumeration value=\"AIF\"/><xs:enumeration value=\"WAV\"/><xs:enumeration value=\"WMA\"/><xs:enumeration value=\"OGG\"/><xs:enumeration value=\"PCM\"/><xs:enumeration value=\"AC3\"/><xs:enumeration value=\"MIDI\"/><xs:enumeration value=\"MP3\"/><xs:enumeration value=\"AAC\"/><xs:enumeration value=\"GIF\"/><xs:enumeration value=\"BMP\"/><xs:enumeration value=\"PNG\"/><xs:enumeration value=\"PDF\"/><xs:enumeration value=\"DOC\"/><xs:enumeration value=\"XLS\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"resolutionType\"><xs:annotation><xs:documentation>Code	Description	72	96	300	600	800	1200</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"72\"/><xs:enumeration value=\"96\"/><xs:enumeration value=\"300\"/><xs:enumeration value=\"600\"/><xs:enumeration value=\"800\"/><xs:enumeration value=\"1200\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"representationType\"><xs:annotation><xs:documentation>Code	Description	A	Actual	R	Representative</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"R\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"colorModeType\"><xs:annotation><xs:documentation>Code	Description	RGB	RGB	CMY	CMYK	GRA	Gray Scale	OTH	Other	WEB	Vector B/W	VEC	Vector Color	BIT	Bitmap</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"RGB\"/><xs:enumeration value=\"CMY\"/><xs:enumeration value=\"GRA\"/><xs:enumeration value=\"OTH\"/><xs:enumeration value=\"WEB\"/><xs:enumeration value=\"VEC\"/><xs:enumeration value=\"BIT\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"backgroundType\"><xs:annotation><xs:documentation>Code	Description	WHI	White	CLI	White w/clipping path	TRA	Transparent	OTH	Other	NUL	N/A</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"WHI\"/><xs:enumeration value=\"CLI\"/><xs:enumeration value=\"TRA\"/><xs:enumeration value=\"OTH\"/><xs:enumeration value=\"NUL\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"orientationViewType\"><xs:annotation><xs:documentation>Code	Description	ANG	Angle	BAC	Back	BOT	Bottom	CON	Connector	FRO	Front	KIT	Kit	LEF	Left	LIF	Lifestyle	NUL	Not Applicable	OTH	Other	RIT	Right	SID	Side	TOP	Top	ZZ1	User 1	ZZ2	User 2	ZZ3	User 3	ZZ4	User 4	ZZ5	User 5	ZZ6	User 6	ZZ7	User 7	ZZ8	User 8	ZZ9	User 9</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"ANG\"/><xs:enumeration value=\"BAC\"/><xs:enumeration value=\"BOT\"/><xs:enumeration value=\"CON\"/><xs:enumeration value=\"FRO\"/><xs:enumeration value=\"KIT\"/><xs:enumeration value=\"LEF\"/><xs:enumeration value=\"LIF\"/><xs:enumeration value=\"NUL\"/><xs:enumeration value=\"RIT\"/><xs:enumeration value=\"SID\"/><xs:enumeration value=\"TOP\"/><xs:enumeration value=\"ZZ1\"/><xs:enumeration value=\"ZZ2\"/><xs:enumeration value=\"ZZ3\"/><xs:enumeration value=\"ZZ4\"/><xs:enumeration value=\"ZZ5\"/><xs:enumeration value=\"ZZ6\"/><xs:enumeration value=\"ZZ7\"/><xs:enumeration value=\"ZZ8\"/><xs:enumeration value=\"ZZ9\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"dimensionUOMType\"><xs:annotation><xs:documentation>Code	Description	PX	Pixels	IN	Inches	CM	Centimeters</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"PX\"/><xs:enumeration value=\"IN\"/><xs:enumeration value=\"CM\"/></xs:restriction></xs:simpleType><!-- \"Footer\" element definition --><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element name=\"RecordCount\" type=\"xs:string\"/></xs:sequence></xs:complexType></xs:element></xs:schema>");
            ACESschemas.Add("4.0", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" version=\"4.0\" xml:lang=\"en\"><xs:annotation><xs:documentation>Auto Care Assocation ACES xml schema version 4.0 for exchanging catalog application data.	(c)2003-2018 Auto Care Assocation All rights reserved.	We do not enforce a default namespace or \"targetNamespace\" with this release to minimize the changes	required to existing instance documents and procedures.</xs:documentation></xs:annotation><!-- simple type definitions --><xs:simpleType name=\"acesVersionType\"><xs:annotation><xs:documentation source=\"http://www.xfront.com/Versioning.pdf\">Ties the instance document to a schema version.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"1.0\"/><xs:enumeration value=\"2.0\"/><xs:enumeration value=\"3.0\"/><xs:enumeration value=\"3.0.1\"/><xs:enumeration value=\"3.1\"/><xs:enumeration value=\"3.2\"/><xs:enumeration value=\"4.0\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"actionType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"D\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetNameType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:minLength value=\"1\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"brandType\"><xs:annotation><xs:documentation source=\"http://www.regular-expressions.info/xmlcharclass.html\">Ideally four uppercase chars without vowels but legacy included some vowels so we	exclude just the ones necessary for each character position.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:pattern value=\"[B-Z-[EIOU]][B-Z-[EIO]][B-Z-[OU]][A-Z]\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"idType\"><xs:restriction base=\"xs:positiveInteger\"/></xs:simpleType><xs:simpleType name=\"partNumberBaseType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"0\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"uomType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"mm\"/><xs:enumeration value=\"cm\"/><xs:enumeration value=\"in\"/><xs:enumeration value=\"ft\"/><xs:enumeration value=\"mg\"/><xs:enumeration value=\"g\"/><xs:enumeration value=\"kg\"/><xs:enumeration value=\"oz\"/><xs:enumeration value=\"lb\"/><xs:enumeration value=\"ton\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yearType\"><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"4\"/><xs:minInclusive value=\"1896\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yesnoType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"yes\"/><xs:enumeration value=\"no\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"submissionType\"><xs:restriction base=\"xs:string\"><xs:enumeration value=\"FULL\"/><xs:enumeration value=\"UPDATE\"/><xs:enumeration value=\"TEST\"/></xs:restriction></xs:simpleType><!-- complex type definitions --><xs:complexType name=\"appItemsBaseType\" abstract=\"true\"><xs:sequence><xs:group ref=\"vehicleIdentGroup\"/><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineBlock\" minOccurs=\"0\"/><xs:element ref=\"EngineBoreStroke\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"PowerOutput\" minOccurs=\"0\"/><xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:group ref=\"transGroup\" minOccurs=\"0\"/><xs:element ref=\"TransElecControlled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/></xs:sequence><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/><xs:attribute name=\"validate\" type=\"yesnoType\" default=\"yes\"/></xs:complexType><xs:complexType name=\"appType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"AssetName\"/><xs:element ref=\"AssetItemOrder\" minOccurs=\"0\"/><xs:element ref=\"AssetItemRef\" minOccurs=\"0\"/></xs:sequence></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"assetType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"/></xs:complexContent></xs:complexType><xs:complexType name=\"noteType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\"/><xs:attribute name=\"lang\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"en\"/><xs:enumeration value=\"fr\"/><xs:enumeration value=\"sp\"/></xs:restriction></xs:simpleType></xs:attribute></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partNumberType\"><xs:simpleContent><xs:extension base=\"partNumberBaseType\"><xs:attribute name=\"BrandAAIAID\" type=\"brandType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partTypeType\"><xs:annotation><xs:documentation>A Part Type references the primary key in the Parts PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"positionType\"><xs:annotation><xs:documentation>A Position references the primary key in the Position PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"qualType\"><xs:sequence><xs:element name=\"param\" type=\"paramType\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element name=\"text\" type=\"xs:string\"/></xs:sequence><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:complexType><xs:complexType name=\"paramType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\" type=\"uomType\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\" type=\"uomType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"vehAttrType\"><xs:annotation><xs:documentation>Vehicle Attributes reference the primary key in the associated VCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"yearRangeType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" use=\"required\"><xs:simpleType><xs:restriction base=\"yearType\"/></xs:simpleType></xs:attribute><xs:attribute name=\"to\" type=\"yearType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><!-- document structure --><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Asset\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"DigitalAsset\" minOccurs=\"0\" maxOccurs=\"1\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"acesVersionType\" use=\"required\"/></xs:complexType></xs:element><!-- \"Header\" element definition --><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"MfrCode\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"BrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\"/><xs:element name=\"ApprovedFor\" type=\"approvedForType\" minOccurs=\"0\"/><xs:element name=\"SubmissionType\" type=\"submissionType\"/><xs:element name=\"MapperCompany\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperContact\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhone\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperEmail\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"VcdbVersionDate\" type=\"xs:date\"/><xs:element name=\"QdbVersionDate\" type=\"xs:date\"/><xs:element name=\"PcdbVersionDate\" type=\"xs:date\"/></xs:sequence></xs:complexType></xs:element><!-- Vehicle Identification Group definition --><xs:group name=\"vehicleIdentGroup\"><xs:annotation><xs:documentation>One of the following must be sent in the Vehicle Ident Group:	- A Base Vehicle ID	- A Make / Year or Make / Year-Range combination must be included with each application. 	- A Base Equipment ID	- A Mfr / Equipment Model / Vehicle Type</xs:documentation></xs:annotation><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence><xs:sequence><xs:element ref=\"EquipmentBase\"/></xs:sequence><xs:sequence><xs:element ref=\"Mfr\"/><xs:element ref=\"EquipmentModel\"/><xs:element ref=\"VehicleType\"/></xs:sequence></xs:choice></xs:group><!-- Transmission Group definition --><xs:group name=\"transGroup\"><xs:choice><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:group><!-- element definitions  --><xs:element name=\"App\" type=\"appType\"/><xs:element name=\"Aspiration\" type=\"vehAttrType\"/><xs:element name=\"Asset\"><xs:complexType><xs:complexContent><xs:extension base=\"assetType\"><xs:sequence><xs:element ref=\"AssetName\"/></xs:sequence></xs:extension></xs:complexContent></xs:complexType></xs:element><xs:element name=\"AssetItemOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"AssetItemRef\" type=\"xs:string\"/><xs:element name=\"AssetName\" type=\"assetNameType\"/><xs:element name=\"BaseVehicle\" type=\"vehAttrType\"/><xs:element name=\"BedLength\" type=\"vehAttrType\"/><xs:element name=\"BedType\" type=\"vehAttrType\"/><xs:element name=\"BodyNumDoors\" type=\"vehAttrType\"/><xs:element name=\"BodyType\" type=\"vehAttrType\"/><xs:element name=\"BrakeABS\" type=\"vehAttrType\"/><xs:element name=\"BrakeSystem\" type=\"vehAttrType\"/><xs:element name=\"CylinderHeadType\" type=\"vehAttrType\"/><xs:element name=\"DisplayOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"DriveType\" type=\"vehAttrType\"/><xs:element name=\"EngineBase\" type=\"vehAttrType\"/><xs:element name=\"EngineBlock\" type=\"vehAttrType\"/><xs:element name=\"EngineBoreStroke\" type=\"vehAttrType\"/><xs:element name=\"EngineDesignation\" type=\"vehAttrType\"/><xs:element name=\"EngineMfr\" type=\"vehAttrType\"/><xs:element name=\"EngineVIN\" type=\"vehAttrType\"/><xs:element name=\"EngineVersion\" type=\"vehAttrType\"/><xs:element name=\"EquipmentBase\" type=\"vehAttrType\"/><xs:element name=\"EquipmentModel\" type=\"vehAttrType\"/><xs:element name=\"FrontBrakeType\" type=\"vehAttrType\"/><xs:element name=\"FrontSpringType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliverySubType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliveryType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemControlType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemDesign\" type=\"vehAttrType\"/><xs:element name=\"FuelType\" type=\"vehAttrType\"/><xs:element name=\"IgnitionSystemType\" type=\"vehAttrType\"/><xs:element name=\"Make\" type=\"vehAttrType\"/><xs:element name=\"Mfr\" type=\"vehAttrType\"/><xs:element name=\"MfrBodyCode\" type=\"vehAttrType\"/><xs:element name=\"MfrLabel\" type=\"xs:string\"/><xs:element name=\"Model\" type=\"vehAttrType\"/><xs:element name=\"Note\" type=\"noteType\"/><xs:element name=\"Part\" type=\"partNumberType\"/><xs:element name=\"PartType\" type=\"partTypeType\"/><xs:element name=\"Position\" type=\"positionType\"/><xs:element name=\"PowerOutput\" type=\"vehAttrType\"/><xs:element name=\"Qty\" type=\"xs:string\"/><xs:element name=\"Qual\" type=\"qualType\"/><xs:element name=\"RearBrakeType\" type=\"vehAttrType\"/><xs:element name=\"RearSpringType\" type=\"vehAttrType\"/><xs:element name=\"Region\" type=\"vehAttrType\"/><xs:element name=\"SteeringSystem\" type=\"vehAttrType\"/><xs:element name=\"SteeringType\" type=\"vehAttrType\"/><xs:element name=\"SubModel\" type=\"vehAttrType\"/><xs:element name=\"TransElecControlled\" type=\"vehAttrType\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"TransmissionBase\" type=\"vehAttrType\"/><xs:element name=\"TransmissionControlType\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfr\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfrCode\" type=\"vehAttrType\"/><xs:element name=\"TransmissionNumSpeeds\" type=\"vehAttrType\"/><xs:element name=\"TransmissionType\" type=\"vehAttrType\"/><xs:element name=\"ValvesPerEngine\" type=\"vehAttrType\"/><xs:element name=\"VehicleType\" type=\"vehAttrType\"/><xs:element name=\"WheelBase\" type=\"vehAttrType\"/><xs:element name=\"Years\" type=\"yearRangeType\"/><xs:complexType name=\"approvedForType\"><xs:sequence><xs:element name=\"Country\" maxOccurs=\"unbounded\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence></xs:complexType><xs:element name=\"DigitalAsset\"><xs:complexType><xs:sequence><xs:element name=\"DigitalFileInformation\" type=\"digitalFileInformationType\" minOccurs=\"1\" maxOccurs=\"unbounded\"/></xs:sequence></xs:complexType></xs:element><xs:complexType name=\"digitalFileInformationType\"><xs:sequence><xs:element name=\"FileName\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetDetailType\" type=\"assetDetailType\"/><xs:element name=\"FileType\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"assetFileType\"><xs:maxLength value=\"4\"/><xs:minLength value=\"3\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Representation\" type=\"representationType\" minOccurs=\"0\"/><xs:element name=\"FileSize\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"10\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Resolution\" type=\"resolutionType\" minOccurs=\"0\"/><xs:element name=\"ColorMode\" type=\"colorModeType\" minOccurs=\"0\"/><xs:element name=\"Background\" type=\"backgroundType\" minOccurs=\"0\"/><xs:element name=\"OrientationView\" type=\"orientationViewType\" minOccurs=\"0\"/><xs:element name=\"AssetDimensions\" minOccurs=\"0\"><xs:complexType><xs:sequence><xs:element name=\"AssetHeight\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetWidth\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"UOM\" type=\"dimensionUOMType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"AssetDescription\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"FilePath\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"URI\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:anyURI\"><xs:maxLength value=\"2000\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"FileDateModified\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"ExpirationDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"Country\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"AssetName\" use=\"required\"/><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"LanguageCode\" type=\"xs:string\"/></xs:complexType><xs:simpleType name=\"assetDetailType\"><xs:annotation><xs:documentation>Code	Description	360		360 Degree Image Set	APG		Application Guide	AUD		Audio File	BRO		Brochure	BUL		Technical Bulletin	BUY		Buyers Guide	CAS		Case Study	CAT		Catalog	CER		Certificate of Origin	DAS		Datasheet	DRW	Technical Drawing	EBK		Ebook	FAB		Features and Benefits	FED		Full Engineering Drawing 	HMS		Hazardous Materials Info Sheet	INS		Installation Instructions	ISG		Illustration Guide	LIN		Line Art	LGO		Logo Image	MSD		Material Safety Data Sheet	OWN	Owner's Manual	P01		Photo – out of package	P02		Photo – in package	P03		Photo – lifestyle view	P04		Photo - Primary	P05		Photo - Close Up	P06		Photo - Mounted	P07		Photo - Unmounted	PAG		Link To Manufacturer Page	PAL		Pallet Configuration Drawing	PDB		Product Brochure	PC1		Planogram Consumer Pack 1	PC2		Planogram Consumer Pack 2	PC3		Planogram Consumer Pack 3	PI1		Planogram Inner Pack 1	PI2		Planogram Inner Pack 2	PI3		Planogram Inner Pack 3	PP1		Planogram Case Pack 1	PP2		Planogram Case Pack 2	PP3		Planogram Case Pack 3	PSS		Product Specifications Sheet	PST		Price Sheet	RES		Research Bulletin	SPE		Specification Sheet Filename 	THU		Thumbnail	TON		Tone Art	WAR	Warranty	MHP		Whitepaper	ZZ1	User 1	ZZ2	User 2	ZZ3	User 3	ZZ4	User 4	ZZ5	User 5	ZZ6	User 6	ZZ7	User 7	ZZ8	User 8	ZZ9	User 9</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"360\"/><xs:enumeration value=\"APG\"/><xs:enumeration value=\"AUD\"/><xs:enumeration value=\"BRO\"/><xs:enumeration value=\"BUL\"/><xs:enumeration value=\"BUY\"/><xs:enumeration value=\"CAS\"/><xs:enumeration value=\"CAT\"/><xs:enumeration value=\"CER\"/><xs:enumeration value=\"DAS\"/><xs:enumeration value=\"DRW\"/><xs:enumeration value=\"EBK\"/><xs:enumeration value=\"FAB\"/><xs:enumeration value=\"FED\"/><xs:enumeration value=\"HMS\"/><xs:enumeration value=\"INS\"/><xs:enumeration value=\"ISG\"/><xs:enumeration value=\"LIN\"/><xs:enumeration value=\"LGO\"/><xs:enumeration value=\"MSD\"/><xs:enumeration value=\"OWN\"/><xs:enumeration value=\"P01\"/><xs:enumeration value=\"P02\"/><xs:enumeration value=\"P03\"/><xs:enumeration value=\"P04\"/><xs:enumeration value=\"P05\"/><xs:enumeration value=\"P06\"/><xs:enumeration value=\"P07\"/><xs:enumeration value=\"PAG\"/><xs:enumeration value=\"PAL\"/><xs:enumeration value=\"PDB\"/><xs:enumeration value=\"PC1\"/><xs:enumeration value=\"PC2\"/><xs:enumeration value=\"PC3\"/><xs:enumeration value=\"PI1\"/><xs:enumeration value=\"PI2\"/><xs:enumeration value=\"PI3\"/><xs:enumeration value=\"PP1\"/><xs:enumeration value=\"PP2\"/><xs:enumeration value=\"PP3\"/><xs:enumeration value=\"PSS\"/><xs:enumeration value=\"PST\"/><xs:enumeration value=\"RES\"/><xs:enumeration value=\"SPE\"/><xs:enumeration value=\"THU\"/><xs:enumeration value=\"TON\"/><xs:enumeration value=\"WAR\"/><xs:enumeration value=\"WHP\"/><xs:enumeration value=\"ZZ1\"/><xs:enumeration value=\"ZZ2\"/><xs:enumeration value=\"ZZ3\"/><xs:enumeration value=\"ZZ4\"/><xs:enumeration value=\"ZZ5\"/><xs:enumeration value=\"ZZ6\"/><xs:enumeration value=\"ZZ7\"/><xs:enumeration value=\"ZZ8\"/><xs:enumeration value=\"ZZ9\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetFileType\"><xs:annotation><xs:documentation>Code	Description	TIF		Tagged Image File	JPG		Joint Photographic Experts Group	EPS		Encapsulated PostScript	TXT		.txt TEXT FILE	FLV		.flv VIDEO FILE	F4V		.f4v VIDEO FILE	AVI		.avi VIDEO FILE	WEBM	.webm VIDEO FILE	OGV		.ogv VIDEO VILE	MP4		.mp4 VIDEO FILE	MKV		.mkv VIDEO FILE	AIF		.aif AUDIO FILE	WAV	.wav AUDIO FILE	WMA	.wma AUDIO FILE	OGG	.ogg AUDIO FILE	PCM		.pcm AUDIO FILE	AC3		.ac3 AUDIO FILE	MIDI		.mid AUDIO FILE	MP3		.mp3 AUDIO FILE	AAC		.aac AUDIO FILE	GIF		Graphics Interchange Format	BMP		Bitmap Image	PNG		Portable Network Graphics	PDF		Portable Document Format	DOC		MS Word	XLS		MS Excel</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"TIF\"/><xs:enumeration value=\"JPG\"/><xs:enumeration value=\"EPS\"/><xs:enumeration value=\"TXT\"/><xs:enumeration value=\"FLV\"/><xs:enumeration value=\"F4V\"/><xs:enumeration value=\"AVI\"/><xs:enumeration value=\"WEBM\"/><xs:enumeration value=\"OGV\"/><xs:enumeration value=\"MP4\"/><xs:enumeration value=\"MKV\"/><xs:enumeration value=\"AIF\"/><xs:enumeration value=\"WAV\"/><xs:enumeration value=\"WMA\"/><xs:enumeration value=\"OGG\"/><xs:enumeration value=\"PCM\"/><xs:enumeration value=\"AC3\"/><xs:enumeration value=\"MIDI\"/><xs:enumeration value=\"MP3\"/><xs:enumeration value=\"AAC\"/><xs:enumeration value=\"GIF\"/><xs:enumeration value=\"BMP\"/><xs:enumeration value=\"PNG\"/><xs:enumeration value=\"PDF\"/><xs:enumeration value=\"DOC\"/><xs:enumeration value=\"XLS\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"resolutionType\"><xs:annotation><xs:documentation>Code	Description	72	96	300	600	800	1200</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"72\"/><xs:enumeration value=\"96\"/><xs:enumeration value=\"300\"/><xs:enumeration value=\"600\"/><xs:enumeration value=\"800\"/><xs:enumeration value=\"1200\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"representationType\"><xs:annotation><xs:documentation>Code	Description	A	Actual	R	Representative</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"R\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"colorModeType\"><xs:annotation><xs:documentation>Code	Description	RGB	RGB	CMY	CMYK	GRA	Gray Scale	OTH	Other	WEB	Vector B/W	VEC	Vector Color	BIT	Bitmap</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"RGB\"/><xs:enumeration value=\"CMY\"/><xs:enumeration value=\"GRA\"/><xs:enumeration value=\"OTH\"/><xs:enumeration value=\"WEB\"/><xs:enumeration value=\"VEC\"/><xs:enumeration value=\"BIT\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"backgroundType\"><xs:annotation><xs:documentation>Code	Description	WHI	White	CLI	White w/clipping path	TRA	Transparent	OTH	Other	NUL	N/A</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"WHI\"/><xs:enumeration value=\"CLI\"/><xs:enumeration value=\"TRA\"/><xs:enumeration value=\"OTH\"/><xs:enumeration value=\"NUL\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"orientationViewType\"><xs:annotation><xs:documentation>Code	Description	ANG	Angle	BAC	Back	BOT	Bottom	CON	Connector	FRO	Front	KIT	Kit	LEF	Left	LIF	Lifestyle	NUL	Not Applicable	OTH	Other	RIT	Right	SID	Side	TOP	Top	ZZ1	User 1	ZZ2	User 2	ZZ3	User 3	ZZ4	User 4	ZZ5	User 5	ZZ6	User 6	ZZ7	User 7	ZZ8	User 8	ZZ9	User 9</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"ANG\"/><xs:enumeration value=\"BAC\"/><xs:enumeration value=\"BOT\"/><xs:enumeration value=\"CON\"/><xs:enumeration value=\"FRO\"/><xs:enumeration value=\"KIT\"/><xs:enumeration value=\"LEF\"/><xs:enumeration value=\"LIF\"/><xs:enumeration value=\"NUL\"/><xs:enumeration value=\"RIT\"/><xs:enumeration value=\"SID\"/><xs:enumeration value=\"TOP\"/><xs:enumeration value=\"ZZ1\"/><xs:enumeration value=\"ZZ2\"/><xs:enumeration value=\"ZZ3\"/><xs:enumeration value=\"ZZ4\"/><xs:enumeration value=\"ZZ5\"/><xs:enumeration value=\"ZZ6\"/><xs:enumeration value=\"ZZ7\"/><xs:enumeration value=\"ZZ8\"/><xs:enumeration value=\"ZZ9\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"dimensionUOMType\"><xs:annotation><xs:documentation>Code	Description	PX	Pixels	IN	Inches	CM	Centimeters</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"PX\"/><xs:enumeration value=\"IN\"/><xs:enumeration value=\"CM\"/></xs:restriction></xs:simpleType><!-- \"Footer\" element definition --><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element name=\"RecordCount\" type=\"xs:string\"/></xs:sequence></xs:complexType></xs:element></xs:schema>");
            ACESschemas.Add("4.1", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" version=\"4.1\" xml:lang=\"en\"><xs:annotation><xs:documentation>Auto Care Assocation ACES xml schema version 4.1 for exchanging catalog application data.	(c)2003-2020 Auto Care Assocation All rights reserved.	We do not enforce a default namespace or \"targetNamespace\" with this release to minimize the changes	required to existing instance documents and procedures.</xs:documentation></xs:annotation><!-- simple type definitions --><xs:simpleType name=\"acesVersionType\"><xs:annotation><xs:documentation source=\"http://www.xfront.com/Versioning.pdf\">Ties the instance document to a schema version.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"1.0\"/><xs:enumeration value=\"2.0\"/><xs:enumeration value=\"3.0\"/><xs:enumeration value=\"3.0.1\"/><xs:enumeration value=\"3.1\"/><xs:enumeration value=\"3.2\"/><xs:enumeration value=\"4.0\"/><xs:enumeration value=\"4.1\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"actionType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"D\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetNameType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:minLength value=\"1\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"brandType\"><xs:annotation><xs:documentation source=\"http://www.regular-expressions.info/xmlcharclass.html\">Ideally four uppercase chars without vowels but legacy included some vowels so we	exclude just the ones necessary for each character position.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:pattern value=\"[B-Z-[EIOU]][B-Z-[EIO]][B-Z-[OU]][A-Z]\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"idType\"><xs:restriction base=\"xs:positiveInteger\"/></xs:simpleType><xs:simpleType name=\"partNumberBaseType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"0\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"uomType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"mm\"/><xs:enumeration value=\"cm\"/><xs:enumeration value=\"in\"/><xs:enumeration value=\"ft\"/><xs:enumeration value=\"mg\"/><xs:enumeration value=\"g\"/><xs:enumeration value=\"kg\"/><xs:enumeration value=\"oz\"/><xs:enumeration value=\"lb\"/><xs:enumeration value=\"ton\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yearType\"><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"4\"/><xs:minInclusive value=\"1896\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yesnoType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"yes\"/><xs:enumeration value=\"no\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"submissionType\"><xs:restriction base=\"xs:string\"><xs:enumeration value=\"FULL\"/><xs:enumeration value=\"UPDATE\"/><xs:enumeration value=\"TEST\"/></xs:restriction></xs:simpleType><!-- complex type definitions --><xs:complexType name=\"appItemsBaseType\" abstract=\"true\"><xs:sequence><xs:group ref=\"vehicleIdentGroup\"/><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineBlock\" minOccurs=\"0\"/><xs:element ref=\"EngineBoreStroke\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"PowerOutput\" minOccurs=\"0\"/><xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:group ref=\"transGroup\" minOccurs=\"0\"/><xs:element ref=\"TransElecControlled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/></xs:sequence><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/><xs:attribute name=\"validate\" type=\"yesnoType\" default=\"yes\"/></xs:complexType><xs:complexType name=\"appType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"AssetName\"/><xs:element ref=\"AssetItemOrder\" minOccurs=\"0\"/><xs:element ref=\"AssetItemRef\" minOccurs=\"0\"/></xs:sequence></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"assetType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"/></xs:complexContent></xs:complexType><xs:complexType name=\"noteType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\"/><xs:attribute name=\"lang\"><xs:simpleType><xs:restriction base=\"xs:NMTOKEN\"><xs:enumeration value=\"en\"/><xs:enumeration value=\"fr\"/><xs:enumeration value=\"es\"/></xs:restriction></xs:simpleType></xs:attribute></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partNumberType\"><xs:simpleContent><xs:extension base=\"partNumberBaseType\"><xs:attribute name=\"BrandAAIAID\" type=\"brandType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partTypeType\"><xs:annotation><xs:documentation>A Part Type references the primary key in the Parts PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"positionType\"><xs:annotation><xs:documentation>A Position references the primary key in the Position PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"qualType\"><xs:sequence><xs:element name=\"param\" type=\"paramType\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element name=\"text\" type=\"xs:string\"/></xs:sequence><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:complexType><xs:complexType name=\"paramType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\" type=\"uomType\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\" type=\"uomType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"vehAttrType\"><xs:annotation><xs:documentation>Vehicle Attributes reference the primary key in the associated VCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"yearRangeType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" use=\"required\"><xs:simpleType><xs:restriction base=\"yearType\"/></xs:simpleType></xs:attribute><xs:attribute name=\"to\" type=\"yearType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><!-- document structure --><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Asset\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"DigitalAsset\" minOccurs=\"0\" maxOccurs=\"1\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"acesVersionType\" use=\"required\"/></xs:complexType></xs:element><!-- \"Header\" element definition --><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"MfrCode\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"BrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\"/><xs:element name=\"ApprovedFor\" type=\"approvedForType\" minOccurs=\"0\"/><xs:element name=\"SubmissionType\" type=\"submissionType\"/><xs:element name=\"MapperCompany\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperContact\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhone\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperEmail\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"VcdbVersionDate\" type=\"xs:date\"/><xs:element name=\"QdbVersionDate\" type=\"xs:date\"/><xs:element name=\"PcdbVersionDate\" type=\"xs:date\"/></xs:sequence></xs:complexType></xs:element><!-- Vehicle Identification Group definition --><xs:group name=\"vehicleIdentGroup\"><xs:annotation><xs:documentation>One of the following must be sent in the Vehicle Ident Group:	- A Base Vehicle ID	- A Make / Year or Make / Year-Range combination must be included with each application. 	- A Base Equipment ID	- A Mfr / Equipment Model / Vehicle Type</xs:documentation></xs:annotation><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence><xs:sequence><xs:element ref=\"EquipmentBase\"/></xs:sequence><xs:sequence><xs:element ref=\"Mfr\"/><xs:element ref=\"EquipmentModel\"/><xs:element ref=\"VehicleType\"/><xs:element name=\"ProductionYears\" minOccurs=\"0\"><xs:complexType><xs:attribute name=\"ProductionStart\" type=\"yearType\"/><xs:attribute name=\"ProductionEnd\" type=\"yearType\"/></xs:complexType></xs:element></xs:sequence></xs:choice></xs:group><!-- Transmission Group definition --><xs:group name=\"transGroup\"><xs:choice><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:group><!-- element definitions  --><xs:element name=\"App\" type=\"appType\"/><xs:element name=\"Aspiration\" type=\"vehAttrType\"/><xs:element name=\"Asset\"><xs:complexType><xs:complexContent><xs:extension base=\"assetType\"><xs:sequence><xs:element ref=\"AssetName\"/></xs:sequence></xs:extension></xs:complexContent></xs:complexType></xs:element><xs:element name=\"AssetItemOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"AssetItemRef\" type=\"xs:string\"/><xs:element name=\"AssetName\" type=\"assetNameType\"/><xs:element name=\"BaseVehicle\" type=\"vehAttrType\"/><xs:element name=\"BedLength\" type=\"vehAttrType\"/><xs:element name=\"BedType\" type=\"vehAttrType\"/><xs:element name=\"BodyNumDoors\" type=\"vehAttrType\"/><xs:element name=\"BodyType\" type=\"vehAttrType\"/><xs:element name=\"BrakeABS\" type=\"vehAttrType\"/><xs:element name=\"BrakeSystem\" type=\"vehAttrType\"/><xs:element name=\"CylinderHeadType\" type=\"vehAttrType\"/><xs:element name=\"DisplayOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"DriveType\" type=\"vehAttrType\"/><xs:element name=\"EngineBase\" type=\"vehAttrType\"/><xs:element name=\"EngineBlock\" type=\"vehAttrType\"/><xs:element name=\"EngineBoreStroke\" type=\"vehAttrType\"/><xs:element name=\"EngineDesignation\" type=\"vehAttrType\"/><xs:element name=\"EngineMfr\" type=\"vehAttrType\"/><xs:element name=\"EngineVIN\" type=\"vehAttrType\"/><xs:element name=\"EngineVersion\" type=\"vehAttrType\"/><xs:element name=\"EquipmentBase\" type=\"vehAttrType\"/><xs:element name=\"EquipmentModel\" type=\"vehAttrType\"/><xs:element name=\"FrontBrakeType\" type=\"vehAttrType\"/><xs:element name=\"FrontSpringType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliverySubType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliveryType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemControlType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemDesign\" type=\"vehAttrType\"/><xs:element name=\"FuelType\" type=\"vehAttrType\"/><xs:element name=\"IgnitionSystemType\" type=\"vehAttrType\"/><xs:element name=\"Make\" type=\"vehAttrType\"/><xs:element name=\"Mfr\" type=\"vehAttrType\"/><xs:element name=\"MfrBodyCode\" type=\"vehAttrType\"/><xs:element name=\"MfrLabel\" type=\"xs:string\"/><xs:element name=\"Model\" type=\"vehAttrType\"/><xs:element name=\"Note\" type=\"noteType\"/><xs:element name=\"Part\" type=\"partNumberType\"/><xs:element name=\"PartType\" type=\"partTypeType\"/><xs:element name=\"Position\" type=\"positionType\"/><xs:element name=\"PowerOutput\" type=\"vehAttrType\"/><xs:element name=\"Qty\" type=\"xs:positiveInteger\"/><xs:element name=\"Qual\" type=\"qualType\"/><xs:element name=\"RearBrakeType\" type=\"vehAttrType\"/><xs:element name=\"RearSpringType\" type=\"vehAttrType\"/><xs:element name=\"Region\" type=\"vehAttrType\"/><xs:element name=\"SteeringSystem\" type=\"vehAttrType\"/><xs:element name=\"SteeringType\" type=\"vehAttrType\"/><xs:element name=\"SubModel\" type=\"vehAttrType\"/><xs:element name=\"TransElecControlled\" type=\"vehAttrType\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"TransmissionBase\" type=\"vehAttrType\"/><xs:element name=\"TransmissionControlType\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfr\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfrCode\" type=\"vehAttrType\"/><xs:element name=\"TransmissionNumSpeeds\" type=\"vehAttrType\"/><xs:element name=\"TransmissionType\" type=\"vehAttrType\"/><xs:element name=\"ValvesPerEngine\" type=\"vehAttrType\"/><xs:element name=\"VehicleType\" type=\"vehAttrType\"/><xs:element name=\"WheelBase\" type=\"vehAttrType\"/><xs:element name=\"Years\" type=\"yearRangeType\"/><xs:complexType name=\"approvedForType\"><xs:sequence><xs:element name=\"Country\" maxOccurs=\"unbounded\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence></xs:complexType><xs:element name=\"DigitalAsset\"><xs:complexType><xs:sequence><xs:element name=\"DigitalFileInformation\" type=\"digitalFileInformationType\" minOccurs=\"1\" maxOccurs=\"unbounded\"/></xs:sequence></xs:complexType></xs:element><xs:complexType name=\"digitalFileInformationType\"><xs:sequence><xs:element name=\"FileName\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetDetailType\" type=\"assetDetailType\"/><xs:element name=\"FileType\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"assetFileType\"><xs:maxLength value=\"4\"/><xs:minLength value=\"3\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Representation\" type=\"representationType\" minOccurs=\"0\"/><xs:element name=\"FileSize\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"10\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Resolution\" type=\"resolutionType\" minOccurs=\"0\"/><xs:element name=\"ColorMode\" type=\"colorModeType\" minOccurs=\"0\"/><xs:element name=\"Background\" type=\"backgroundType\" minOccurs=\"0\"/><xs:element name=\"OrientationView\" type=\"orientationViewType\" minOccurs=\"0\"/><xs:element name=\"AssetDimensions\" minOccurs=\"0\"><xs:complexType><xs:sequence><xs:element name=\"AssetHeight\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetWidth\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"UOM\" type=\"dimensionUOMType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"AssetDescription\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"FilePath\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"URI\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:anyURI\"><xs:maxLength value=\"2000\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"FileDateModified\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"ExpirationDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"Country\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"AssetName\" use=\"required\"/><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"LanguageCode\" type=\"xs:string\"/></xs:complexType><xs:simpleType name=\"assetDetailType\"><xs:annotation><xs:documentation>Code	Description	360		360 Degree Image Set	APG		Application Guide	AUD		Audio File	BRO		Brochure	BUL		Technical Bulletin	BUY		Buyers Guide	CAS		Case Study	CAT		Catalog	CER		Certificate of Origin	DAS		Datasheet	DRW	Technical Drawing	EBK		Ebook	FAB		Features and Benefits	FED		Full Engineering Drawing 	HMS		Hazardous Materials Info Sheet	INS		Installation Instructions	ISG		Illustration Guide	LIN		Line Art	LGO		Logo Image	MSD		Material Safety Data Sheet	OWN	Owner's Manual	P01		Photo – out of package	P02		Photo – in package	P03		Photo – lifestyle view	P04		Photo - Primary	P05		Photo - Close Up	P06		Photo - Mounted	P07		Photo - Unmounted	PAG		Link To Manufacturer Page	PAL		Pallet Configuration Drawing	PDB		Product Brochure	PC1		Planogram Consumer Pack 1	PC2		Planogram Consumer Pack 2	PC3		Planogram Consumer Pack 3	PI1		Planogram Inner Pack 1	PI2		Planogram Inner Pack 2	PI3		Planogram Inner Pack 3	PP1		Planogram Case Pack 1	PP2		Planogram Case Pack 2	PP3		Planogram Case Pack 3	PSS		Product Specifications Sheet	PST		Price Sheet	RES		Research Bulletin	SPE		Specification Sheet Filename 	THU		Thumbnail	TON		Tone Art	WAR	Warranty	MHP		Whitepaper	ZZ1	User 1	ZZ2	User 2	ZZ3	User 3	ZZ4	User 4	ZZ5	User 5	ZZ6	User 6	ZZ7	User 7	ZZ8	User 8	ZZ9	User 9</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"360\"/><xs:enumeration value=\"APG\"/><xs:enumeration value=\"AUD\"/><xs:enumeration value=\"BRO\"/><xs:enumeration value=\"BUL\"/><xs:enumeration value=\"BUY\"/><xs:enumeration value=\"CAS\"/><xs:enumeration value=\"CAT\"/><xs:enumeration value=\"CER\"/><xs:enumeration value=\"DAS\"/><xs:enumeration value=\"DRW\"/><xs:enumeration value=\"EBK\"/><xs:enumeration value=\"FAB\"/><xs:enumeration value=\"FED\"/><xs:enumeration value=\"HMS\"/><xs:enumeration value=\"INS\"/><xs:enumeration value=\"ISG\"/><xs:enumeration value=\"LIN\"/><xs:enumeration value=\"LGO\"/><xs:enumeration value=\"MSD\"/><xs:enumeration value=\"OWN\"/><xs:enumeration value=\"P01\"/><xs:enumeration value=\"P02\"/><xs:enumeration value=\"P03\"/><xs:enumeration value=\"P04\"/><xs:enumeration value=\"P05\"/><xs:enumeration value=\"P06\"/><xs:enumeration value=\"P07\"/><xs:enumeration value=\"PAG\"/><xs:enumeration value=\"PAL\"/><xs:enumeration value=\"PDB\"/><xs:enumeration value=\"PC1\"/><xs:enumeration value=\"PC2\"/><xs:enumeration value=\"PC3\"/><xs:enumeration value=\"PI1\"/><xs:enumeration value=\"PI2\"/><xs:enumeration value=\"PI3\"/><xs:enumeration value=\"PP1\"/><xs:enumeration value=\"PP2\"/><xs:enumeration value=\"PP3\"/><xs:enumeration value=\"PSS\"/><xs:enumeration value=\"PST\"/><xs:enumeration value=\"RES\"/><xs:enumeration value=\"SPE\"/><xs:enumeration value=\"THU\"/><xs:enumeration value=\"TON\"/><xs:enumeration value=\"WAR\"/><xs:enumeration value=\"WHP\"/><xs:enumeration value=\"ZZ1\"/><xs:enumeration value=\"ZZ2\"/><xs:enumeration value=\"ZZ3\"/><xs:enumeration value=\"ZZ4\"/><xs:enumeration value=\"ZZ5\"/><xs:enumeration value=\"ZZ6\"/><xs:enumeration value=\"ZZ7\"/><xs:enumeration value=\"ZZ8\"/><xs:enumeration value=\"ZZ9\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetFileType\"><xs:annotation><xs:documentation>Code	Description	TIF		Tagged Image File	JPG		Joint Photographic Experts Group	EPS		Encapsulated PostScript	TXT		.txt TEXT FILE	FLV		.flv VIDEO FILE	F4V		.f4v VIDEO FILE	AVI		.avi VIDEO FILE	WEBM	.webm VIDEO FILE	OGV		.ogv VIDEO VILE	MP4		.mp4 VIDEO FILE	MKV		.mkv VIDEO FILE	AIF		.aif AUDIO FILE	WAV	.wav AUDIO FILE	WMA	.wma AUDIO FILE	OGG	.ogg AUDIO FILE	PCM		.pcm AUDIO FILE	AC3		.ac3 AUDIO FILE	MIDI		.mid AUDIO FILE	MP3		.mp3 AUDIO FILE	AAC		.aac AUDIO FILE	GIF		Graphics Interchange Format	BMP		Bitmap Image	PNG		Portable Network Graphics	PDF		Portable Document Format	DOC		MS Word	XLS		MS Excel</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"TIF\"/><xs:enumeration value=\"JPG\"/><xs:enumeration value=\"EPS\"/><xs:enumeration value=\"TXT\"/><xs:enumeration value=\"FLV\"/><xs:enumeration value=\"F4V\"/><xs:enumeration value=\"AVI\"/><xs:enumeration value=\"WEBM\"/><xs:enumeration value=\"OGV\"/><xs:enumeration value=\"MP4\"/><xs:enumeration value=\"MKV\"/><xs:enumeration value=\"AIF\"/><xs:enumeration value=\"WAV\"/><xs:enumeration value=\"WMA\"/><xs:enumeration value=\"OGG\"/><xs:enumeration value=\"PCM\"/><xs:enumeration value=\"AC3\"/><xs:enumeration value=\"MIDI\"/><xs:enumeration value=\"MP3\"/><xs:enumeration value=\"AAC\"/><xs:enumeration value=\"GIF\"/><xs:enumeration value=\"BMP\"/><xs:enumeration value=\"PNG\"/><xs:enumeration value=\"PDF\"/><xs:enumeration value=\"DOC\"/><xs:enumeration value=\"XLS\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"resolutionType\"><xs:annotation><xs:documentation>Code	Description	72	96	300	600	800	1200</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"72\"/><xs:enumeration value=\"96\"/><xs:enumeration value=\"300\"/><xs:enumeration value=\"600\"/><xs:enumeration value=\"800\"/><xs:enumeration value=\"1200\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"representationType\"><xs:annotation><xs:documentation>Code	Description	A	Actual	R	Representative</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"A\"/><xs:enumeration value=\"R\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"colorModeType\"><xs:annotation><xs:documentation>Code	Description	RGB	RGB	CMY	CMYK	GRA	Gray Scale	OTH	Other	WEB	Vector B/W	VEC	Vector Color	BIT	Bitmap</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"RGB\"/><xs:enumeration value=\"CMY\"/><xs:enumeration value=\"GRA\"/><xs:enumeration value=\"OTH\"/><xs:enumeration value=\"WEB\"/><xs:enumeration value=\"VEC\"/><xs:enumeration value=\"BIT\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"backgroundType\"><xs:annotation><xs:documentation>Code	Description	WHI	White	CLI	White w/clipping path	TRA	Transparent	OTH	Other	NUL	N/A</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"WHI\"/><xs:enumeration value=\"CLI\"/><xs:enumeration value=\"TRA\"/><xs:enumeration value=\"OTH\"/><xs:enumeration value=\"NUL\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"orientationViewType\"><xs:annotation><xs:documentation>Code	Description	ANG	Angle	BAC	Back	BOT	Bottom	CON	Connector	FRO	Front	KIT	Kit	LEF	Left	LIF	Lifestyle	NUL	Not Applicable	OTH	Other	RIT	Right	SID	Side	TOP	Top	ZZ1	User 1	ZZ2	User 2	ZZ3	User 3	ZZ4	User 4	ZZ5	User 5	ZZ6	User 6	ZZ7	User 7	ZZ8	User 8	ZZ9	User 9</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"ANG\"/><xs:enumeration value=\"BAC\"/><xs:enumeration value=\"BOT\"/><xs:enumeration value=\"CON\"/><xs:enumeration value=\"FRO\"/><xs:enumeration value=\"KIT\"/><xs:enumeration value=\"LEF\"/><xs:enumeration value=\"LIF\"/><xs:enumeration value=\"NUL\"/><xs:enumeration value=\"RIT\"/><xs:enumeration value=\"SID\"/><xs:enumeration value=\"TOP\"/><xs:enumeration value=\"ZZ1\"/><xs:enumeration value=\"ZZ2\"/><xs:enumeration value=\"ZZ3\"/><xs:enumeration value=\"ZZ4\"/><xs:enumeration value=\"ZZ5\"/><xs:enumeration value=\"ZZ6\"/><xs:enumeration value=\"ZZ7\"/><xs:enumeration value=\"ZZ8\"/><xs:enumeration value=\"ZZ9\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"dimensionUOMType\"><xs:annotation><xs:documentation>Code	Description	PX	Pixels	IN	Inches	CM	Centimeters</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:enumeration value=\"PX\"/><xs:enumeration value=\"IN\"/><xs:enumeration value=\"CM\"/></xs:restriction></xs:simpleType><!-- \"Footer\" element definition --><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element name=\"RecordCount\" type=\"xs:string\"/></xs:sequence></xs:complexType></xs:element></xs:schema>");
    // rev1 ACESschemas.Add("4.2", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" version=\"4.2\" xml:lang=\"en\"><xs:annotation><xs:documentation>Auto Care Assocation ACES xml schema version 4.2 for exchanging catalog application data.	(c)2003-2021 Auto Care Assocation All rights reserved.	We do not enforce a default namespace or \"targetNamespace\" with this release to minimize the changes	required to existing instance documents and procedures.</xs:documentation></xs:annotation><xs:simpleType name=\"acesVersionType\"><xs:annotation><xs:documentation source=\"http://www.xfront.com/Versioning.pdf\">Ties the instance document to a schema version.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:minLength value=\"3\"/><xs:maxLength value=\"5\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"actionType\"><xs:restriction base=\"xs:token\"><xs:length value=\"1\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetNameType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:minLength value=\"1\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"brandType\"><xs:annotation><xs:documentation source=\"http://www.regular-expressions.info/xmlcharclass.html\">Ideally four uppercase chars without vowels but legacy included some vowels so we	exclude just the ones necessary for each character position.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:pattern value=\"[B-Z-[EIOU]][B-Z-[EIO]][B-Z-[OU]][A-Z]\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"idType\"><xs:restriction base=\"xs:positiveInteger\"/></xs:simpleType><xs:simpleType name=\"partNumberBaseType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"0\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"uomType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"2\"/><xs:maxLength value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yearType\"><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"4\"/><xs:minInclusive value=\"1896\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yesnoType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"2\"/><xs:maxLength value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"submissionType\"><xs:restriction base=\"xs:string\"><xs:minLength value=\"4\"/><xs:maxLength value=\"6\"/></xs:restriction></xs:simpleType><xs:complexType name=\"appItemsBaseType\" abstract=\"true\"><xs:sequence><xs:group ref=\"vehicleIdentGroup\"/><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineBlock\" minOccurs=\"0\"/><xs:element ref=\"EngineBoreStroke\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"PowerOutput\" minOccurs=\"0\"/><xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:group ref=\"transGroup\" minOccurs=\"0\"/><xs:element ref=\"TransElecControlled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/></xs:sequence><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/><xs:attribute name=\"validate\" type=\"yesnoType\" default=\"yes\"/></xs:complexType><xs:complexType name=\"appType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"AssetName\"/><xs:element ref=\"AssetItemOrder\" minOccurs=\"0\"/><xs:element ref=\"AssetItemRef\" minOccurs=\"0\"/></xs:sequence></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"assetType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"/></xs:complexContent></xs:complexType><xs:complexType name=\"noteType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\"/><xs:attribute name=\"lang\" type=\"xs:token\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partNumberType\"><xs:simpleContent><xs:extension base=\"partNumberBaseType\"><xs:attribute name=\"BrandAAIAID\" type=\"brandType\"/><xs:attribute name=\"SubBrandAAIAID\" type=\"brandType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partTypeType\"><xs:annotation><xs:documentation>A Part Type references the primary key in the Parts PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"positionType\"><xs:annotation><xs:documentation>A Position references the primary key in the Position PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"qualType\"><xs:sequence><xs:element name=\"param\" type=\"paramType\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element name=\"text\" type=\"xs:string\"/></xs:sequence><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:complexType><xs:complexType name=\"paramType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\" type=\"uomType\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\" type=\"uomType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"vehAttrType\"><xs:annotation><xs:documentation>Vehicle Attributes reference the primary key in the associated VCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"yearRangeType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" use=\"required\"><xs:simpleType><xs:restriction base=\"yearType\"/></xs:simpleType></xs:attribute><xs:attribute name=\"to\" type=\"yearType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Asset\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"DigitalAsset\" minOccurs=\"0\" maxOccurs=\"1\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"acesVersionType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"MfrCode\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"BrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"SubBrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\"/><xs:element name=\"ApprovedFor\" type=\"approvedForType\" minOccurs=\"0\"/><xs:element name=\"PartsApprovedFor\" type=\"approvedForType\" minOccurs=\"0\"/><xs:element name=\"RegionFor\" type=\"RegionType\" minOccurs=\"0\"/><xs:element name=\"SubmissionType\" type=\"submissionType\"/><xs:element name=\"MapperCompany\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperContact\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhone\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperEmail\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"VcdbVersionDate\" type=\"xs:date\"/><xs:element name=\"QdbVersionDate\" type=\"xs:date\"/><xs:element name=\"PcdbVersionDate\" type=\"xs:date\"/></xs:sequence></xs:complexType></xs:element><xs:group name=\"vehicleIdentGroup\"><xs:annotation><xs:documentation>One of the following must be sent in the Vehicle Ident Group:	- A Base Vehicle ID	- A Make / Year or Make / Year-Range combination must be included with each application. 	- A Base Equipment ID	- A Mfr / Equipment Model / Vehicle Type</xs:documentation></xs:annotation><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence><xs:sequence><xs:element ref=\"EquipmentBase\"/></xs:sequence><xs:sequence><xs:element ref=\"Mfr\"/><xs:element ref=\"EquipmentModel\"/><xs:element ref=\"VehicleType\"/><xs:element name=\"ProductionYears\" minOccurs=\"0\"><xs:complexType><xs:attribute name=\"ProductionStart\" type=\"yearType\"/><xs:attribute name=\"ProductionEnd\" type=\"yearType\"/></xs:complexType></xs:element></xs:sequence></xs:choice></xs:group><xs:group name=\"transGroup\"><xs:choice><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:group><xs:element name=\"App\" type=\"appType\"/><xs:element name=\"Aspiration\" type=\"vehAttrType\"/><xs:element name=\"Asset\"><xs:complexType><xs:complexContent><xs:extension base=\"assetType\"><xs:sequence><xs:element ref=\"AssetName\"/></xs:sequence></xs:extension></xs:complexContent></xs:complexType></xs:element><xs:element name=\"AssetItemOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"AssetItemRef\" type=\"xs:string\"/><xs:element name=\"AssetName\" type=\"assetNameType\"/><xs:element name=\"BaseVehicle\" type=\"vehAttrType\"/><xs:element name=\"BedLength\" type=\"vehAttrType\"/><xs:element name=\"BedType\" type=\"vehAttrType\"/><xs:element name=\"BodyNumDoors\" type=\"vehAttrType\"/><xs:element name=\"BodyType\" type=\"vehAttrType\"/><xs:element name=\"BrakeABS\" type=\"vehAttrType\"/><xs:element name=\"BrakeSystem\" type=\"vehAttrType\"/><xs:element name=\"CylinderHeadType\" type=\"vehAttrType\"/><xs:element name=\"DisplayOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"DriveType\" type=\"vehAttrType\"/><xs:element name=\"EngineBase\" type=\"vehAttrType\"/><xs:element name=\"EngineBlock\" type=\"vehAttrType\"/><xs:element name=\"EngineBoreStroke\" type=\"vehAttrType\"/><xs:element name=\"EngineDesignation\" type=\"vehAttrType\"/><xs:element name=\"EngineMfr\" type=\"vehAttrType\"/><xs:element name=\"EngineVIN\" type=\"vehAttrType\"/><xs:element name=\"EngineVersion\" type=\"vehAttrType\"/><xs:element name=\"EquipmentBase\" type=\"vehAttrType\"/><xs:element name=\"EquipmentModel\" type=\"vehAttrType\"/><xs:element name=\"FrontBrakeType\" type=\"vehAttrType\"/><xs:element name=\"FrontSpringType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliverySubType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliveryType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemControlType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemDesign\" type=\"vehAttrType\"/><xs:element name=\"FuelType\" type=\"vehAttrType\"/><xs:element name=\"IgnitionSystemType\" type=\"vehAttrType\"/><xs:element name=\"Make\" type=\"vehAttrType\"/><xs:element name=\"Mfr\" type=\"vehAttrType\"/><xs:element name=\"MfrBodyCode\" type=\"vehAttrType\"/><xs:element name=\"MfrLabel\" type=\"xs:string\"/><xs:element name=\"Model\" type=\"vehAttrType\"/><xs:element name=\"Note\" type=\"noteType\"/><xs:element name=\"Part\" type=\"partNumberType\"/><xs:element name=\"PartType\" type=\"partTypeType\"/><xs:element name=\"Position\" type=\"positionType\"/><xs:element name=\"PowerOutput\" type=\"vehAttrType\"/><xs:element name=\"Qty\" type=\"xs:positiveInteger\"/><xs:element name=\"Qual\" type=\"qualType\"/><xs:element name=\"RearBrakeType\" type=\"vehAttrType\"/><xs:element name=\"RearSpringType\" type=\"vehAttrType\"/><xs:element name=\"Region\" type=\"vehAttrType\"/><xs:element name=\"SteeringSystem\" type=\"vehAttrType\"/><xs:element name=\"SteeringType\" type=\"vehAttrType\"/><xs:element name=\"SubModel\" type=\"vehAttrType\"/><xs:element name=\"TransElecControlled\" type=\"vehAttrType\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"TransmissionBase\" type=\"vehAttrType\"/><xs:element name=\"TransmissionControlType\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfr\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfrCode\" type=\"vehAttrType\"/><xs:element name=\"TransmissionNumSpeeds\" type=\"vehAttrType\"/><xs:element name=\"TransmissionType\" type=\"vehAttrType\"/><xs:element name=\"ValvesPerEngine\" type=\"vehAttrType\"/><xs:element name=\"VehicleType\" type=\"vehAttrType\"/><xs:element name=\"WheelBase\" type=\"vehAttrType\"/><xs:element name=\"Years\" type=\"yearRangeType\"/><xs:complexType name=\"RegionType\"><xs:sequence><xs:element name=\"RegionID\" maxOccurs=\"unbounded\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence></xs:complexType><xs:complexType name=\"approvedForType\"><xs:sequence><xs:element name=\"Country\" maxOccurs=\"unbounded\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence></xs:complexType><xs:element name=\"DigitalAsset\"><xs:complexType><xs:sequence><xs:element name=\"DigitalFileInformation\" type=\"digitalFileInformationType\" minOccurs=\"1\" maxOccurs=\"unbounded\"/></xs:sequence></xs:complexType></xs:element><xs:complexType name=\"digitalFileInformationType\"><xs:sequence><xs:element name=\"FileName\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetDetailType\" type=\"assetDetailType\"/><xs:element name=\"FileType\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"assetFileType\"><xs:maxLength value=\"4\"/><xs:minLength value=\"3\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Representation\" type=\"representationType\" minOccurs=\"0\"/><xs:element name=\"FileSize\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"10\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Resolution\" type=\"resolutionType\" minOccurs=\"0\"/><xs:element name=\"ColorMode\" type=\"colorModeType\" minOccurs=\"0\"/><xs:element name=\"Background\" type=\"backgroundType\" minOccurs=\"0\"/><xs:element name=\"OrientationView\" type=\"orientationViewType\" minOccurs=\"0\"/><xs:element name=\"AssetDimensions\" minOccurs=\"0\"><xs:complexType><xs:sequence><xs:element name=\"AssetHeight\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetWidth\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"UOM\" type=\"dimensionUOMType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"AssetDescription\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"FilePath\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"URI\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:anyURI\"><xs:maxLength value=\"2000\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"FileDateModified\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"ExpirationDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"Country\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"AssetName\" use=\"required\"/><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"LanguageCode\" type=\"xs:string\"/></xs:complexType><xs:simpleType name=\"assetDetailType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetFileType\"><xs:annotation><xs:documentation></xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:minLength value=\"2\"/><xs:maxLength value=\"4\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"resolutionType\"><xs:annotation><xs:documentation></xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:minLength value=\"2\"/><xs:maxLength value=\"4\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"representationType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"1\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"colorModeType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"backgroundType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"orientationViewType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"dimensionUOMType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element name=\"RecordCount\" type=\"xs:string\"/></xs:sequence></xs:complexType></xs:element></xs:schema>");
            ACESschemas.Add("4.2", "<?xml version=\"1.0\" encoding=\"UTF-8\"?><xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" elementFormDefault=\"qualified\" version=\"4.2\" xml:lang=\"en\"><xs:annotation><xs:documentation>Auto Care Assocation ACES xml schema version 4.2 for exchanging catalog application data.  (c)2003-2021 Auto Care Assocation All rights reserved.	We do not enforce a default namespace or \"targetNamespace\" with this release to minimize the changes  required to existing instance documents and procedures.</xs:documentation></xs:annotation><xs:simpleType name=\"acesVersionType\"><xs:annotation><xs:documentation source=\"http://www.xfront.com/Versioning.pdf\">Ties the instance document to a schema version.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:minLength value=\"3\"/><xs:maxLength value=\"5\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"actionType\"><xs:restriction base=\"xs:token\"><xs:length value=\"1\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetNameType\"><xs:restriction base=\"xs:NMTOKEN\"><xs:minLength value=\"1\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"brandType\"><xs:annotation><xs:documentation source=\"http://www.regular-expressions.info/xmlcharclass.html\">Ideally four uppercase chars without vowels but legacy included some vowels so we    exclude just the ones necessary for each character position.</xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:pattern value=\"[B-Z-[EIOU]][B-Z-[EIO]][B-Z-[OU]][A-Z]\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"idType\"><xs:restriction base=\"xs:positiveInteger\"/></xs:simpleType><xs:simpleType name=\"partNumberBaseType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"0\"/><xs:maxLength value=\"45\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"uomType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"1\"/><xs:maxLength value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yearType\"><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"4\"/><xs:minInclusive value=\"1896\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"yesnoType\"><xs:restriction base=\"xs:token\"><xs:minLength value=\"2\"/><xs:maxLength value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"submissionType\"><xs:restriction base=\"xs:string\"><xs:minLength value=\"4\"/><xs:maxLength value=\"6\"/></xs:restriction></xs:simpleType><xs:complexType name=\"appItemsBaseType\" abstract=\"true\"><xs:sequence><xs:group ref=\"vehicleIdentGroup\"/><xs:element ref=\"MfrBodyCode\" minOccurs=\"0\"/><xs:element ref=\"BodyNumDoors\" minOccurs=\"0\"/><xs:element ref=\"BodyType\" minOccurs=\"0\"/><xs:element ref=\"DriveType\" minOccurs=\"0\"/><xs:element ref=\"EngineBase\" minOccurs=\"0\"/><xs:element ref=\"EngineBlock\" minOccurs=\"0\"/><xs:element ref=\"EngineBoreStroke\" minOccurs=\"0\"/><xs:element ref=\"EngineDesignation\" minOccurs=\"0\"/><xs:element ref=\"EngineVIN\" minOccurs=\"0\"/><xs:element ref=\"EngineVersion\" minOccurs=\"0\"/><xs:element ref=\"EngineMfr\" minOccurs=\"0\"/><xs:element ref=\"PowerOutput\" minOccurs=\"0\"/><xs:element ref=\"ValvesPerEngine\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliveryType\" minOccurs=\"0\"/><xs:element ref=\"FuelDeliverySubType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemControlType\" minOccurs=\"0\"/><xs:element ref=\"FuelSystemDesign\" minOccurs=\"0\"/><xs:element ref=\"Aspiration\" minOccurs=\"0\"/><xs:element ref=\"CylinderHeadType\" minOccurs=\"0\"/><xs:element ref=\"FuelType\" minOccurs=\"0\"/><xs:element ref=\"IgnitionSystemType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfrCode\" minOccurs=\"0\"/><xs:group ref=\"transGroup\" minOccurs=\"0\"/><xs:element ref=\"TransElecControlled\" minOccurs=\"0\"/><xs:element ref=\"TransmissionMfr\" minOccurs=\"0\"/><xs:element ref=\"BedLength\" minOccurs=\"0\"/><xs:element ref=\"BedType\" minOccurs=\"0\"/><xs:element ref=\"WheelBase\" minOccurs=\"0\"/><xs:element ref=\"BrakeSystem\" minOccurs=\"0\"/><xs:element ref=\"FrontBrakeType\" minOccurs=\"0\"/><xs:element ref=\"RearBrakeType\" minOccurs=\"0\"/><xs:element ref=\"BrakeABS\" minOccurs=\"0\"/><xs:element ref=\"FrontSpringType\" minOccurs=\"0\"/><xs:element ref=\"RearSpringType\" minOccurs=\"0\"/><xs:element ref=\"SteeringSystem\" minOccurs=\"0\"/><xs:element ref=\"SteeringType\" minOccurs=\"0\"/><xs:element ref=\"Region\" minOccurs=\"0\"/><xs:element ref=\"Qual\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Note\" minOccurs=\"0\" maxOccurs=\"unbounded\"/></xs:sequence><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/><xs:attribute name=\"ref\" type=\"xs:string\"/><xs:attribute name=\"validate\" type=\"yesnoType\" default=\"yes\"/></xs:complexType><xs:complexType name=\"appType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"><xs:sequence><xs:element ref=\"Qty\"/><xs:element ref=\"PartType\"/><xs:element ref=\"MfrLabel\" minOccurs=\"0\"/><xs:element ref=\"Position\" minOccurs=\"0\"/><xs:element ref=\"Part\"/><xs:element ref=\"DisplayOrder\" minOccurs=\"0\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"AssetName\"/><xs:element ref=\"AssetItemOrder\" minOccurs=\"0\"/><xs:element ref=\"AssetItemRef\" minOccurs=\"0\"/></xs:sequence></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name=\"assetType\"><xs:complexContent><xs:extension base=\"appItemsBaseType\"/></xs:complexContent></xs:complexType><xs:complexType name=\"noteType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\"/><xs:attribute name=\"lang\" type=\"xs:token\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partNumberType\"><xs:simpleContent><xs:extension base=\"partNumberBaseType\"><xs:attribute name=\"BrandAAIAID\" type=\"brandType\"/><xs:attribute name=\"SubBrandAAIAID\" type=\"brandType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"partTypeType\"><xs:annotation><xs:documentation>A Part Type references the primary key in the Parts PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"positionType\"><xs:annotation><xs:documentation>A Position references the primary key in the Position PCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"qualType\"><xs:sequence><xs:element name=\"param\" type=\"paramType\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element name=\"text\" type=\"xs:string\"/></xs:sequence><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:complexType><xs:complexType name=\"paramType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"value\" type=\"xs:string\" use=\"required\"/><xs:attribute name=\"uom\" type=\"uomType\"/><xs:attribute name=\"altvalue\" type=\"xs:string\"/><xs:attribute name=\"altuom\" type=\"uomType\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"vehAttrType\"><xs:annotation><xs:documentation>Vehicle Attributes reference the primary key in the associated VCdb table.</xs:documentation></xs:annotation><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"id\" type=\"idType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name=\"yearRangeType\"><xs:simpleContent><xs:extension base=\"xs:string\"><xs:attribute name=\"from\" use=\"required\"><xs:simpleType><xs:restriction base=\"yearType\"/></xs:simpleType></xs:attribute><xs:attribute name=\"to\" type=\"yearType\" use=\"required\"/></xs:extension></xs:simpleContent></xs:complexType><xs:element name=\"ACES\"><xs:complexType><xs:sequence><xs:element ref=\"Header\"/><xs:element ref=\"App\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"Asset\" minOccurs=\"0\" maxOccurs=\"unbounded\"/><xs:element ref=\"DigitalAsset\" minOccurs=\"0\" maxOccurs=\"1\"/><xs:element ref=\"Footer\"/></xs:sequence><xs:attribute name=\"version\" type=\"acesVersionType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"Header\"><xs:complexType><xs:sequence><xs:element name=\"Company\" type=\"xs:string\"/><xs:element name=\"SenderName\" type=\"xs:string\"/><xs:element name=\"SenderPhone\" type=\"xs:string\"/><xs:element name=\"SenderPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"MfrCode\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"BrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"SubBrandAAIAID\" type=\"brandType\" minOccurs=\"0\"/><xs:element name=\"DocumentTitle\" type=\"xs:string\"/><xs:element name=\"DocFormNumber\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\"/><xs:element name=\"ApprovedFor\" type=\"approvedForType\" minOccurs=\"0\"/><xs:element name=\"PartsApprovedFor\" type=\"approvedForType\" minOccurs=\"0\"/><xs:element name=\"RegionFor\" type=\"RegionType\" minOccurs=\"0\"/><xs:element name=\"SubmissionType\" type=\"submissionType\"/><xs:element name=\"MapperCompany\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperContact\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhone\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperPhoneExt\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"MapperEmail\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"VcdbVersionDate\" type=\"xs:date\"/><xs:element name=\"QdbVersionDate\" type=\"xs:date\"/><xs:element name=\"PcdbVersionDate\" type=\"xs:date\"/></xs:sequence></xs:complexType></xs:element><xs:group name=\"vehicleIdentGroup\"><xs:annotation><xs:documentation>One of the following must be sent in the Vehicle Ident Group:    - A Base Vehicle ID - A Make / Year or Make / Year-Range combination must be included with each application.    - A Base Equipment ID   - A Mfr / Equipment Model / Vehicle Type</xs:documentation></xs:annotation><xs:choice><xs:sequence><xs:element ref=\"BaseVehicle\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence><xs:sequence><xs:element ref=\"Years\"/><xs:element ref=\"Make\"/><xs:choice minOccurs=\"0\"><xs:element ref=\"VehicleType\"/><xs:sequence minOccurs=\"0\"><xs:element ref=\"Model\"/><xs:element ref=\"SubModel\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:sequence><xs:sequence><xs:element ref=\"EquipmentBase\"/></xs:sequence><xs:sequence><xs:element ref=\"Mfr\"/><xs:element ref=\"EquipmentModel\"/><xs:element ref=\"VehicleType\"/><xs:element name=\"ProductionYears\" minOccurs=\"0\"><xs:complexType><xs:attribute name=\"ProductionStart\" type=\"yearType\"/><xs:attribute name=\"ProductionEnd\" type=\"yearType\"/></xs:complexType></xs:element></xs:sequence></xs:choice></xs:group><xs:group name=\"transGroup\"><xs:choice><xs:element ref=\"TransmissionBase\"/><xs:sequence><xs:element ref=\"TransmissionType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionControlType\" minOccurs=\"0\"/><xs:element ref=\"TransmissionNumSpeeds\" minOccurs=\"0\"/></xs:sequence></xs:choice></xs:group><xs:element name=\"App\" type=\"appType\"/><xs:element name=\"Aspiration\" type=\"vehAttrType\"/><xs:element name=\"Asset\"><xs:complexType><xs:complexContent><xs:extension base=\"assetType\"><xs:sequence><xs:element ref=\"AssetName\"/></xs:sequence></xs:extension></xs:complexContent></xs:complexType></xs:element><xs:element name=\"AssetItemOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"AssetItemRef\" type=\"xs:string\"/><xs:element name=\"AssetName\" type=\"assetNameType\"/><xs:element name=\"BaseVehicle\" type=\"vehAttrType\"/><xs:element name=\"BedLength\" type=\"vehAttrType\"/><xs:element name=\"BedType\" type=\"vehAttrType\"/><xs:element name=\"BodyNumDoors\" type=\"vehAttrType\"/><xs:element name=\"BodyType\" type=\"vehAttrType\"/><xs:element name=\"BrakeABS\" type=\"vehAttrType\"/><xs:element name=\"BrakeSystem\" type=\"vehAttrType\"/><xs:element name=\"CylinderHeadType\" type=\"vehAttrType\"/><xs:element name=\"DisplayOrder\" type=\"xs:positiveInteger\"/><xs:element name=\"DriveType\" type=\"vehAttrType\"/><xs:element name=\"EngineBase\" type=\"vehAttrType\"/><xs:element name=\"EngineBlock\" type=\"vehAttrType\"/><xs:element name=\"EngineBoreStroke\" type=\"vehAttrType\"/><xs:element name=\"EngineDesignation\" type=\"vehAttrType\"/><xs:element name=\"EngineMfr\" type=\"vehAttrType\"/><xs:element name=\"EngineVIN\" type=\"vehAttrType\"/><xs:element name=\"EngineVersion\" type=\"vehAttrType\"/><xs:element name=\"EquipmentBase\" type=\"vehAttrType\"/><xs:element name=\"EquipmentModel\" type=\"vehAttrType\"/><xs:element name=\"FrontBrakeType\" type=\"vehAttrType\"/><xs:element name=\"FrontSpringType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliverySubType\" type=\"vehAttrType\"/><xs:element name=\"FuelDeliveryType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemControlType\" type=\"vehAttrType\"/><xs:element name=\"FuelSystemDesign\" type=\"vehAttrType\"/><xs:element name=\"FuelType\" type=\"vehAttrType\"/><xs:element name=\"IgnitionSystemType\" type=\"vehAttrType\"/><xs:element name=\"Make\" type=\"vehAttrType\"/><xs:element name=\"Mfr\" type=\"vehAttrType\"/><xs:element name=\"MfrBodyCode\" type=\"vehAttrType\"/><xs:element name=\"MfrLabel\" type=\"xs:string\"/><xs:element name=\"Model\" type=\"vehAttrType\"/><xs:element name=\"Note\" type=\"noteType\"/><xs:element name=\"Part\" type=\"partNumberType\"/><xs:element name=\"PartType\" type=\"partTypeType\"/><xs:element name=\"Position\" type=\"positionType\"/><xs:element name=\"PowerOutput\" type=\"vehAttrType\"/><xs:element name=\"Qty\" type=\"xs:positiveInteger\"/><xs:element name=\"Qual\" type=\"qualType\"/><xs:element name=\"RearBrakeType\" type=\"vehAttrType\"/><xs:element name=\"RearSpringType\" type=\"vehAttrType\"/><xs:element name=\"Region\" type=\"vehAttrType\"/><xs:element name=\"SteeringSystem\" type=\"vehAttrType\"/><xs:element name=\"SteeringType\" type=\"vehAttrType\"/><xs:element name=\"SubModel\" type=\"vehAttrType\"/><xs:element name=\"TransElecControlled\" type=\"vehAttrType\"/><xs:element name=\"TransferDate\" type=\"xs:date\"/><xs:element name=\"TransmissionBase\" type=\"vehAttrType\"/><xs:element name=\"TransmissionControlType\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfr\" type=\"vehAttrType\"/><xs:element name=\"TransmissionMfrCode\" type=\"vehAttrType\"/><xs:element name=\"TransmissionNumSpeeds\" type=\"vehAttrType\"/><xs:element name=\"TransmissionType\" type=\"vehAttrType\"/><xs:element name=\"ValvesPerEngine\" type=\"vehAttrType\"/><xs:element name=\"VehicleType\" type=\"vehAttrType\"/><xs:element name=\"WheelBase\" type=\"vehAttrType\"/><xs:element name=\"Years\" type=\"yearRangeType\"/><xs:complexType name=\"RegionType\"><xs:sequence><xs:element ref=\"Region\" maxOccurs=\"unbounded\"/></xs:sequence></xs:complexType><xs:complexType name=\"approvedForType\"><xs:sequence><xs:element name=\"Country\" maxOccurs=\"unbounded\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence></xs:complexType><xs:element name=\"DigitalAsset\"><xs:complexType><xs:sequence><xs:element name=\"DigitalFileInformation\" type=\"digitalFileInformationType\" minOccurs=\"1\" maxOccurs=\"unbounded\"/></xs:sequence></xs:complexType></xs:element><xs:complexType name=\"digitalFileInformationType\"><xs:sequence><xs:element name=\"FileName\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetDetailType\" type=\"assetDetailType\"/><xs:element name=\"FileType\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"assetFileType\"><xs:maxLength value=\"4\"/><xs:minLength value=\"3\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Representation\" type=\"representationType\" minOccurs=\"0\"/><xs:element name=\"FileSize\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:positiveInteger\"><xs:totalDigits value=\"10\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"Resolution\" type=\"resolutionType\" minOccurs=\"0\"/><xs:element name=\"ColorMode\" type=\"colorModeType\" minOccurs=\"0\"/><xs:element name=\"Background\" type=\"backgroundType\" minOccurs=\"0\"/><xs:element name=\"OrientationView\" type=\"orientationViewType\" minOccurs=\"0\"/><xs:element name=\"AssetDimensions\" minOccurs=\"0\"><xs:complexType><xs:sequence><xs:element name=\"AssetHeight\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"AssetWidth\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:decimal\"><xs:minExclusive value=\"0\"/><xs:totalDigits value=\"6\"/><xs:fractionDigits value=\"4\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"UOM\" type=\"dimensionUOMType\" use=\"required\"/></xs:complexType></xs:element><xs:element name=\"AssetDescription\" type=\"xs:string\" minOccurs=\"0\"/><xs:element name=\"FilePath\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:string\"><xs:minLength value=\"1\"/><xs:maxLength value=\"80\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"URI\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:anyURI\"><xs:maxLength value=\"2000\"/></xs:restriction></xs:simpleType></xs:element><xs:element name=\"FileDateModified\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"EffectiveDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"ExpirationDate\" type=\"xs:date\" minOccurs=\"0\"/><xs:element name=\"Country\" minOccurs=\"0\"><xs:simpleType><xs:restriction base=\"xs:token\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType></xs:element></xs:sequence><xs:attribute name=\"AssetName\" use=\"required\"/><xs:attribute name=\"action\" type=\"actionType\" use=\"required\"/><xs:attribute name=\"LanguageCode\" type=\"xs:string\"/></xs:complexType><xs:simpleType name=\"assetDetailType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"assetFileType\"><xs:annotation><xs:documentation></xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:minLength value=\"2\"/><xs:maxLength value=\"4\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"resolutionType\"><xs:annotation><xs:documentation></xs:documentation></xs:annotation><xs:restriction base=\"xs:string\"><xs:minLength value=\"2\"/><xs:maxLength value=\"4\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"representationType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"1\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"colorModeType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"backgroundType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"orientationViewType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"3\"/></xs:restriction></xs:simpleType><xs:simpleType name=\"dimensionUOMType\"><xs:annotation><xs:documentation/></xs:annotation><xs:restriction base=\"xs:string\"><xs:length value=\"2\"/></xs:restriction></xs:simpleType><xs:element name=\"Footer\"><xs:complexType><xs:sequence><xs:element name=\"RecordCount\" type=\"xs:string\"/></xs:sequence></xs:complexType></xs:element></xs:schema>");

            noteBlacklist.Add("RWD", false); noteBlacklist.Add("FWD", false); noteBlacklist.Add("2WD", false); noteBlacklist.Add("4WD", false); noteBlacklist.Add("AWD", false); noteBlacklist.Add("All-wheel Drive", false); noteBlacklist.Add("All Wheel Drive", false); noteBlacklist.Add("Allwheel Drive", false);
            noteBlacklist.Add("Turbo", false); noteBlacklist.Add("Aspirated", false); noteBlacklist.Add("Supercharge", false); noteBlacklist.Add("Hybrid", false); noteBlacklist.Add("Diesel", false); noteBlacklist.Add("CNG", false); noteBlacklist.Add("Flex Fuel", false); noteBlacklist.Add("Gas/Elec", false);
            noteBlacklist.Add("2 Door", false); noteBlacklist.Add("3 Door", false); noteBlacklist.Add("4 Door", false); noteBlacklist.Add("5 Door", false); noteBlacklist.Add("6 Door", false); noteBlacklist.Add("2 Dr", false); noteBlacklist.Add("Two Door", false); noteBlacklist.Add("Four Door", false); noteBlacklist.Add("3 Dr", false); noteBlacklist.Add("4 Dr", false); noteBlacklist.Add("Sedan", false); noteBlacklist.Add("Wagon", false); noteBlacklist.Add("Coup", false); noteBlacklist.Add("Hatchback", false); noteBlacklist.Add("Rear Disc", false);
            noteBlacklist.Add("Manual Tran", false); noteBlacklist.Add("Standard Tran", false); noteBlacklist.Add("Automatic Tran", false); noteBlacklist.Add("3 Speed", false); noteBlacklist.Add("4 Speed", false); noteBlacklist.Add("5 Speed", false); noteBlacklist.Add("6 Speed", false); noteBlacklist.Add("7 Speed", false); noteBlacklist.Add("8 Speed", false); noteBlacklist.Add("9 Speed", false); noteBlacklist.Add("10 Speed", false);
            noteBlacklist.Add("Rear Drum", false); noteBlacklist.Add("Dohc", false); noteBlacklist.Add("Sohc", false); noteBlacklist.Add("8 Valve", false); noteBlacklist.Add("12 Valve", false); noteBlacklist.Add("16 Valve", false); noteBlacklist.Add("24 Valve", false);
            noteBlacklist.Add("overhead cam", false); noteBlacklist.Add("overhead valve", false); noteBlacklist.Add("ohv", false); noteBlacklist.Add("4 cyl", false); noteBlacklist.Add("6 cyl", false); noteBlacklist.Add("8 cyl", false); noteBlacklist.Add("V4 ", false); noteBlacklist.Add("V6 ", false); noteBlacklist.Add("V8 ", false); noteBlacklist.Add("V10 ", false); noteBlacklist.Add("L2 ", false); noteBlacklist.Add("L3 ", false);
            noteBlacklist.Add("L4 ", false); noteBlacklist.Add("L5 ", false); noteBlacklist.Add("L6 ", false); noteBlacklist.Add("1.0L", false); noteBlacklist.Add("1.1L", false); noteBlacklist.Add("1.2L", false); noteBlacklist.Add("1.3L", false); noteBlacklist.Add("1.4L", false); noteBlacklist.Add("1.5L", false); noteBlacklist.Add("1.6L", false); noteBlacklist.Add("1.7L", false); noteBlacklist.Add("1.8L", false); noteBlacklist.Add("1.9L", false); noteBlacklist.Add("2.0L", false); noteBlacklist.Add("2.1L", false); noteBlacklist.Add("2.2L", false); noteBlacklist.Add("2.3L", false); noteBlacklist.Add("2.4L", false); noteBlacklist.Add("2.5L", false);
            noteBlacklist.Add("2.6L", false); noteBlacklist.Add("2.7L", false); noteBlacklist.Add("2.8L", false); noteBlacklist.Add("2.9L", false); noteBlacklist.Add("3.0L", false); noteBlacklist.Add("3.1L", false); noteBlacklist.Add("3.2L", false); noteBlacklist.Add("3.3L", false); noteBlacklist.Add("3.4L", false); noteBlacklist.Add("3.5L", false); noteBlacklist.Add("3.6L", false); noteBlacklist.Add("3.7L", false); noteBlacklist.Add("3.8L", false); noteBlacklist.Add("3.9L", false); noteBlacklist.Add("4.0L", false); noteBlacklist.Add("4.1L", false);
            noteBlacklist.Add("4.2L", false); noteBlacklist.Add("4.3L", false); noteBlacklist.Add("4.4L", false); noteBlacklist.Add("4.5L", false); noteBlacklist.Add("4.6L", false); noteBlacklist.Add("4.7L", false); noteBlacklist.Add("4.8L", false); noteBlacklist.Add("4.9L", false); noteBlacklist.Add("5.0L", false); noteBlacklist.Add("5.1L", false); noteBlacklist.Add("5.2L", false); noteBlacklist.Add("5.3L", false); noteBlacklist.Add("5.4L", false); noteBlacklist.Add("5.5L", false); noteBlacklist.Add("5.6L", false); noteBlacklist.Add("5.7L", false);
            noteBlacklist.Add("5.8L", false); noteBlacklist.Add("5.9L", false); noteBlacklist.Add("6.0L", false); noteBlacklist.Add("6.1L", false); noteBlacklist.Add("6.2L", false); noteBlacklist.Add("6.3L", false); noteBlacklist.Add("6.4L", false); noteBlacklist.Add("6.5L", false); noteBlacklist.Add("6.6L", false); noteBlacklist.Add("6.7L", false); noteBlacklist.Add("6.8L", false); noteBlacklist.Add("6.9L", false); noteBlacklist.Add("7.0L", false); noteBlacklist.Add("7.1L", false); noteBlacklist.Add("7.2L", false); noteBlacklist.Add("7.3L", false);
            noteBlacklist.Add("7.4L", false); noteBlacklist.Add("7.5L", false); noteBlacklist.Add("7.6L", false); noteBlacklist.Add("7.7L", false); noteBlacklist.Add("7.8L", false); noteBlacklist.Add("7.9L", false); noteBlacklist.Add("8.0L", false); noteBlacklist.Add("8.1L", false); noteBlacklist.Add("8.2L", false); noteBlacklist.Add("8.3L", false); noteBlacklist.Add("8.4L", false); noteBlacklist.Add("8.5L", false);
            noteBlacklist.Add("Cab Pickup", false); noteBlacklist.Add("model", false); noteBlacklist.Add("Convertible", false); noteBlacklist.Add("Wheel ABS", false); noteBlacklist.Add("non-ABS", false); noteBlacklist.Add("A.B.S.", false); noteBlacklist.Add("Antilock brake", false); noteBlacklist.Add("Anti-Lock Brake", false); noteBlacklist.Add("With ABS", false); noteBlacklist.Add("without ABS", false); noteBlacklist.Add("W/ABS", false); noteBlacklist.Add("W/O ABS", false); noteBlacklist.Add("Except ABS", false); noteBlacklist.Add("Canada", false); noteBlacklist.Add("Mexico", false); noteBlacklist.Add("Pickup", false); noteBlacklist.Add("Cargo Van", false); noteBlacklist.Add("Stripped Chassis", false); noteBlacklist.Add("Body Code", false);
            noteBlacklist.Add("Coil Spring Susp", false); noteBlacklist.Add("Leaf Spring", false); noteBlacklist.Add("Front Coil Susp", false); noteBlacklist.Add("Leaf Susp", false); noteBlacklist.Add("Quattro", false);
            noteBlacklist.Add("0CID", false); noteBlacklist.Add("1CID", false); noteBlacklist.Add("2CID", false); noteBlacklist.Add("3CID", false); noteBlacklist.Add("4CID", false); noteBlacklist.Add("5CID", false); noteBlacklist.Add("6CID", false); noteBlacklist.Add("7CID", false); noteBlacklist.Add("8CID", false); noteBlacklist.Add("9CID", false); noteBlacklist.Add("0 CID", false); noteBlacklist.Add("1 CID", false); noteBlacklist.Add("2 CID", false); noteBlacklist.Add("3 CID", false); noteBlacklist.Add("4 CID", false); noteBlacklist.Add("5 CID", false); noteBlacklist.Add("6 CID", false); noteBlacklist.Add("7 CID", false); noteBlacklist.Add("8 CID", false); noteBlacklist.Add("9 CID", false); noteBlacklist.Add("0CC", false); noteBlacklist.Add("1CC", false); noteBlacklist.Add("2CC", false); noteBlacklist.Add("3CC", false); noteBlacklist.Add("4CC", false); noteBlacklist.Add("5CC", false); noteBlacklist.Add("6CC", false); noteBlacklist.Add("7CC", false); noteBlacklist.Add("8CC", false); noteBlacklist.Add("9CC", false); noteBlacklist.Add("0 CC", false); noteBlacklist.Add("1 CC", false); noteBlacklist.Add("2 CC", false); noteBlacklist.Add("3 CC", false); noteBlacklist.Add("4 CC", false); noteBlacklist.Add("5 CC", false); noteBlacklist.Add("6 CC", false); noteBlacklist.Add("7 CC", false); noteBlacklist.Add("8 CC", false); noteBlacklist.Add("9 CC", false);
            noteBlacklist.Add("Horsepower", false); noteBlacklist.Add("0HP", false); noteBlacklist.Add("1HP", false); noteBlacklist.Add("2HP", false); noteBlacklist.Add("3HP", false); noteBlacklist.Add("4HP", false); noteBlacklist.Add("5HP", false); noteBlacklist.Add("6HP", false); noteBlacklist.Add("7HP", false); noteBlacklist.Add("8HP", false); noteBlacklist.Add("9HP", false); noteBlacklist.Add("0 HP", false); noteBlacklist.Add("1 HP", false); noteBlacklist.Add("2 HP", false); noteBlacklist.Add("3 HP", false); noteBlacklist.Add("4 HP", false); noteBlacklist.Add("5 HP", false); noteBlacklist.Add("6 HP", false); noteBlacklist.Add("7 HP", false); noteBlacklist.Add("8 HP", false); noteBlacklist.Add("9 HP", false); noteBlacklist.Add("0 H.P.", false); noteBlacklist.Add("1 H.P.", false); noteBlacklist.Add("2 H.P.", false); noteBlacklist.Add("3 H.P.", false); noteBlacklist.Add("4 H.P.", false); noteBlacklist.Add("5 H.P.", false); noteBlacklist.Add("6 H.P.", false); noteBlacklist.Add("7 H.P.", false); noteBlacklist.Add("8 H.P.", false); noteBlacklist.Add("9 H.P.", false); noteBlacklist.Add("Engine VIN", false); noteBlacklist.Add("Eng VIN", false); noteBlacklist.Add("Eng. VIN", false);
        }


        public void clear()
        {
            successfulImport = false;
            discardedDeletsOnImport = 0;
            analysisTime = 0;
            FooterRecordCount = 0;
            filePath = "";
            fileMD5hash = "";
            version = "";
            Company = "";
            SenderName = "";
            SenderPhone = "";
            TransferDate = "";
            BrandAAIAID = "";
            DocumentTitle = "";
            EffectiveDate = "";
            SubmissionType = "";
            VcdbVersionDate = "";
            QdbVersionDate = "";
            PcdbVersionDate = "";
            apps.Clear();
            assets.Clear();
            xmlAppNodeCount = 0;
            xmlAssetNodeCount = 0;
            QdbUtilizationScore = 0;
            partsAppCounts.Clear();
            distinctPartTypes.Clear();
            distinctAssetNames.Clear();
            distinctMfrLabels.Clear();
            distinctAssets.Clear();
            basevidOccurrences.Clear();
            partsPartTypes.Clear();
            partsPositions.Clear();
            qdbErrorsCount = 0;
            questionableNotesCount = 0;
            differentialParts.Clear();
            differentialVehicles.Clear();
            xmlValidationErrors.Clear();
            fitmentProblemGroupsBestPermutations.Clear();
            fitmentProblemGroupsAppLists.Clear();
            qtyOutlierCount = 0;
            parttypePositionErrorsCount = 0;
            vcdbConfigurationsErrorsCount = 0;
            parttypeDisagreementCount = 0;
            vcdbCodesErrorsCount = 0;
            basevehicleidsErrorsCount = 0;
            individualAnanlysisChunksList.Clear();
            outlierAnanlysisChunksList.Clear();
            fitmentAnalysisChunksList.Clear();
            fitmentAnalysisChunksGroups.Clear();
            qtyOutlierCachefilesList.Clear();
            parttypePositionErrorsCachefilesList.Clear();
            qdbErrorsCachefilesList.Clear();
            vcdbConfigurationsErrorsCachefilesList.Clear();
            fitmentLogicProblemsCachefilesList.Clear();
            parttypeDisagreementCachefilesList.Clear();
            vcdbCodesErrorsCachefilesList.Clear();
            basevehicleidsErrorsCachefilesList.Clear();
            appHashesFlaggedAsCosmetic.Clear();
            fitmentNodesFlaggedAsCosmetic.Clear();
            noteCounts.Clear();
        }


        public void clearAnalysisResults()
        {
            successfulImport = false;
            analysisTime = 0;
            xmlValidationErrors.Clear();
            fitmentProblemGroupsBestPermutations.Clear();
            fitmentProblemGroupsAppLists.Clear();

            qtyOutlierCount = 0;
            parttypePositionErrorsCount = 0;
            qdbErrorsCount = 0;
            questionableNotesCount = 0;
            vcdbConfigurationsErrorsCount = 0;
            fitmentLogicProblemsCount = 0;
            parttypeDisagreementCount = 0;
            vcdbCodesErrorsCount = 0;
            basevehicleidsErrorsCount = 0;
            individualAnanlysisChunksList.Clear();
            outlierAnanlysisChunksList.Clear();
            fitmentAnalysisChunksList.Clear();
            fitmentAnalysisChunksGroups.Clear();
            qtyOutlierCachefilesList.Clear();
            parttypePositionErrorsCachefilesList.Clear();
            qdbErrorsCachefilesList.Clear();
            vcdbConfigurationsErrorsCachefilesList.Clear();
            fitmentLogicProblemsCachefilesList.Clear();
            parttypeDisagreementCachefilesList.Clear();
            vcdbCodesErrorsCachefilesList.Clear();
            basevehicleidsErrorsCachefilesList.Clear();
        }


        public void logHistoryEvent(string path,string line)
        {
            try
            {
                analysisHistory.Add(line);
                if (logToFile)
                {
                    using (StreamWriter stream = new StreamWriter(path, true))
                    {
                        stream.WriteLine(line);
                    }
                }
            }
            catch(Exception ex)
            {// fail silently - because many parallel jobs are adding to the list, it's possible they collide
            }
        }

        // take a string of CSS-style name-value attribute pairs and cram them into a list of VCdbAttribute class instances
        public List<VCdbAttribute> parseAttributePairsString(string nameValuePairsString)
        {
            List<VCdbAttribute> myAttributeList = new List<VCdbAttribute>();
            string[] attributes = nameValuePairsString.Split(';');
            foreach (string attribute in attributes)
            {
                string[] attributePieces = attribute.Split(':');
                if (attributePieces.Count() == 2)
                {
                    VCdbAttribute VCdbAttributeTemp = new VCdbAttribute(); VCdbAttributeTemp.name = attributePieces[0]; VCdbAttributeTemp.value = Convert.ToInt32(attributePieces[1]);
                    myAttributeList.Add(VCdbAttributeTemp);
                }
            }
            return myAttributeList;
        }






        public void fitmentTreeAddFillerNodes(int width, int height)
        {
            int i, parentId, deepestLevel = 0;
            bool addedNodes = true;
            foreach (fitmentNode myNode in fitmentNodeList) { if (myNode.pathFromRoot.Count() > deepestLevel) { deepestLevel = myNode.pathFromRoot.Count(); } }
            while (addedNodes)
            {
                addedNodes = false;
                for (i = 0; i <= fitmentNodeList.Count() - 1; i++)
                {
                    if (fitmentNodeList[i].nodeId == 0) { continue; } // ignore root node
                    if (fitmentNodeList[i].childNodeIds.Count() > 0) { continue; } // ignore nodes that have children
                    if (fitmentNodeList[i].pathFromRoot.Count() >= deepestLevel) { continue; } // ignore nodes that already occupy the deepest level

                    fitmentNode newNode = new fitmentNode();
                    newNode.nodeId = fitmentNodeList.Count();
                    newNode.parentNode = fitmentNodeList[i].nodeId;
                    newNode.markedAsCosmetic = false;
                    newNode.childNodeIds = new List<int>();
                    newNode.pathFromRoot = new List<int>();
                    newNode.pathFromRoot.Add(newNode.nodeId);
                    newNode.pathFromRoot.AddRange(fitmentNodeList[i].pathFromRoot);
                    newNode.graphicalWidth = width; newNode.graphicalHeight = height;
                    newNode.fitmentElementType = "fillerfillerfiller"; newNode.fitmentElementString = "fillerfillerfiller"; newNode.fitmentElementData = "fillerfillerfiller"; newNode.filler = true;
                    fitmentNodeList[fitmentNodeList[i].nodeId].childNodeIds.Add(newNode.nodeId);
                    parentId = newNode.nodeId;
                    fitmentNodeList.Add(newNode);
                    addedNodes = true;
                    break;
                }
            }
        }


        public int fitmentTreeBranchyness(List<fitmentNode> nodes)
        {
            int score = 0;
            foreach (fitmentNode myNode in nodes)
            {
                if (myNode.nodeId == 0 || myNode.app != null || myNode.deleted) { continue; }
                if (myNode.childNodeIds.Count() > 1) { score += (myNode.childNodeIds.Count() - 1); }
            }
            return score;
        }


        // grade a node-tree for number and severity of bad branches 
        public int fitmentTreeTotalBadBranches(List<fitmentNode> nodes, Qdb qdb, bool detectIncompatibleBranches, bool respectQdbType)
        {
            int score = 0;
            if (nodes.Count() == 0) { return score; }
            foreach (fitmentNode myNode in nodes)
            {
                if (myNode.deleted || myNode.childNodeIds.Count() < 2) { continue; }
                score += (fitmentBranchCompare(myNode.nodeId, nodes, qdb,detectIncompatibleBranches, respectQdbType) * myNode.childNodeIds.Count());
            }
            return score;
        }

        //compile an applist of apps that are downstream of a badbranch
        // used for isolating the apps in a tree that actually contribute to a problem
        public List<App> badbrabchAppsFromNodelist(List<fitmentNode> nodes, Qdb qdb, bool detectIncompatibleBranches, bool respectQdbType)
        {
            List<App> appList = new List<App>();
            if (nodes.Count() == 0) { return appList; }
                        
            int score = 0;
            for(int i=0; i< nodes.Count(); i++)
            {
                if (nodes[i].deleted || nodes[i].childNodeIds.Count() < 2) { continue; }
                score = fitmentBranchCompare(nodes[i].nodeId, nodes, qdb, detectIncompatibleBranches, respectQdbType);
                if (score > 0)
                {// this (i's) node's children are bad branching. look for app nodes that are descndants of this current node
                    for (int j = 0; j < nodes.Count(); j++)
                    {
                        if (nodes[j].app == null || nodes[j].deleted) { continue; }
                        // this is an app node - follow its path to root looking for a touch of the currently identified badbranch node in the containing "myNode" loop
                        foreach (int nodeId in  nodes[j].pathFromRoot)
                        {
                            if(nodes[i].nodeId==nodeId)
                            {
                                appList.Add(nodes[j].app);
                            }
                        }
                        // the path-from-root List<int> does not include root itself. Therefore, if bad branches grow directly from root, we would not see it. so we must seperately check root 
                        if (nodes[i].nodeId==0)
                        {
                            appList.Add(nodes[j].app);
                        }
                    }
                }
            }
            return appList;
        }



        //compile an applist of apps that are downstream of a specific nodeid
        //used for determining the apps that need to be edited when a fitment element is flagged by the user as cosmetic
        public List<App> appsDescendentFromNode(List<fitmentNode> nodes, int targetNodeid)
        {
            List<App> appList = new List<App>();
            if (nodes.Count() == 0) { return appList; }

            for (int j = 0; j < nodes.Count(); j++)
            {
                if (nodes[j].app == null || nodes[j].deleted) { continue; }
                // this is an app node - follow its path to root looking for a touch of the targetnode
                foreach (int nodeId in nodes[j].pathFromRoot)
                {
                    if (nodeId == targetNodeid)
                    {
                        appList.Add(nodes[j].app);
                    }
                }
            }
            return appList;
        }


        // compile a string list of problems found in a nodeTree
        public string fitmentTreeProblemDescription(List<fitmentNode> nodes, Qdb qdb, bool detectIncompatibleBranches, bool respectQdbType)
        {
            List<string> problemsList=new List<string>(); 
            int score = 0;
            if (nodes.Count() == 0) { return ""; }
            foreach (fitmentNode myNode in nodes)
            {
                if (myNode.deleted || myNode.childNodeIds.Count() < 2) { continue; }
                score = fitmentBranchCompare(myNode.nodeId, nodes, qdb, detectIncompatibleBranches, respectQdbType);

                switch(score)
                {
                        case 0: break;
                        case 1:{if(!problemsList.Contains("questionable comparison (Vcdb/Qdb)")){problemsList.Add("questionable comparison (Vcdb/Qdb)");} break;}
                        case 2:{if(!problemsList.Contains("questionable comparison (Note vs VCdb|Qdb)")){problemsList.Add("questionable comparison (Note vs VCdb|Qdb)");} break;}
                        case 3:{if(!problemsList.Contains("CNC Overlap")){problemsList.Add("CNC Overlap");} break;}
                        case 4:{if(!problemsList.Contains("Overlap")){problemsList.Add("Overlap");} break;}
                        case 5:{if(!problemsList.Contains("questionable comparison (disparate VCdb)")){problemsList.Add("questionable comparison (disparate VCdb)");} break;}
                        default: { if (!problemsList.Contains("something bad")) { problemsList.Add("something bad"); } break; };
                }
            }
            return  string.Join(", ", problemsList);
        }

            //VCdbSysX - VCdbSysX   valid                           return 0
            //Qdb - Qdb             valid                           return 0
            //Note - Note           valid                           return 0
            //VCdb - Qdb            Poor form                       return 1
            //VCdb - Note           Poor form                       return 2
            //Qdb - Note            questionable                    return 2
            //VCdb - Part           bad (clasic "deep CNC")         return 3
            //Qdb - Part            bad (clasic "deep CNC")         return 3
            //Note - Part           bad (clasic "deep CNC")         return 3
            //Part - Part           bad (simple overlap)            return 4
            //VCdbSysX - VCdbSysY   bad (ex: EngineBase-DriveType)  return 5


        // find the next supior node that branches. this is used for finding cousin nodes in paralell branches
        public int fitmentTreeFindSuperiorNodeThatBranches(List<fitmentNode> nodes, int nodeId)
        {
            int nodeIdTemp = nodeId;
            int returnValue = -1;
            while (true)
            {
                if (nodes[nodes[nodeIdTemp].parentNode].childNodeIds.Count() > 1)
                {// ascendant node has multiple children - stop here and return
                    returnValue = nodeIdTemp; break;
                }
                else
                {// ascendant node has only one child - it is part of a straight line - keep iterating up the branch (unles we are at root)
                    if (nodeIdTemp == 0)
                    {// stop at root
                        returnValue = -1; break;
                    }
                    nodeIdTemp = nodes[nodeIdTemp].parentNode; // point at next iteration up
                }
            }
            return returnValue;
        }


        // comb compatible nodes up and combine
        public bool fitmentTreeSOMETHING(List<fitmentNode> nodes)
        {
            //test every node against every other node and see if they are compatible and their "superior" nodes are siblings (have same parent)
            // when both are true, the two nodes being compared will be swapped with their superior nodes and then a merge executed

            int saftyCounter = 0;
            bool modifiedTree = true;
            int superiorAid = 0; int superiorBid;
            bool nodesAreCompatible = false;

            while (modifiedTree)
            {
                modifiedTree = false;
                saftyCounter++; if (saftyCounter > 10000) { break; }

                foreach (fitmentNode myNodeA in nodes)
                {
                    if (myNodeA.deleted || myNodeA.nodeId == 0 || myNodeA.filler || myNodeA.app != null) { continue; }

                    foreach (fitmentNode myNodeB in nodes)
                    {
                        if (myNodeB.nodeId == myNodeA.nodeId || myNodeB.deleted || myNodeB.nodeId == 0 || myNodeB.filler || myNodeB.app != null) { continue; }

                        nodesAreCompatible = false;

                        if (myNodeA.fitmentElementType == "note" && myNodeB.fitmentElementType == "note") { nodesAreCompatible = true; }
                        if (myNodeA.fitmentElementType == "qdb" && myNodeB.fitmentElementType == "qdb") { nodesAreCompatible = true; }

                        if (myNodeA.fitmentElementType == "vcdb" && myNodeB.fitmentElementType == "vcdb")
                        {
                            string[] chunksA = myNodeA.fitmentElementData.Split(':'); string[] chunksB = myNodeB.fitmentElementData.Split(':');
                            if (chunksA.Count() == 2 && chunksB.Count() == 2 && chunksA[0] == chunksB[0]) { nodesAreCompatible = true; }
                        }

                        if (nodesAreCompatible)
                        {// found identical nodes in different branches - now see if they are within reach of each other

                            superiorAid = fitmentTreeFindSuperiorNodeThatBranches(nodes, myNodeA.nodeId); // find the top of the straight-line stack that A is in
                            superiorBid = fitmentTreeFindSuperiorNodeThatBranches(nodes, myNodeB.nodeId); // find the top of the straight-line stack that B is in

                            if (superiorAid > 0 && superiorBid > 0)
                            {
                                if (nodes[superiorAid].parentNode == nodes[superiorBid].parentNode)
                                {// both both nodes live in straignt-line branches from the same parent. they can consolidate

                                    if (myNodeA.nodeId != superiorAid)
                                    {// swap A with its supior
                                        fitmentNodesSwap(nodes, superiorAid, myNodeA.nodeId);
                                        modifiedTree = true;
                                    }

                                    if (myNodeB.nodeId != superiorBid)
                                    {// swap B with its supior
                                        fitmentNodesSwap(nodes, superiorBid, myNodeB.nodeId);
                                        modifiedTree = true;
                                    }
                                }
                            }
                        }
                        if (modifiedTree) { break; }
                    }
                    if (modifiedTree) { break; }
                }
                if (modifiedTree)
                {
                    fitmentTreeMergeRedundantChildren(nodes, nodes[superiorAid].parentNode);
                }
            }
            return modifiedTree;
        }




        // pass in a node tha has children. each child will be re-routed to the given node's parent through a new replicated version of the passed-in node
        public bool fitmentTreeSplitBranch(List<fitmentNode> nodes, int nodeId)
        {
            int i, parentNodeId, originalChildId;
            List<int> removedChildren = new List<int>();

            if (nodes[nodeId].childNodeIds.Count() <= 1 || nodes[nodeId].deleted) { return false; }

            parentNodeId = nodes[nodeId].parentNode;
            originalChildId = nodes[nodeId].childNodeIds[0];

            foreach (int childNodeId in nodes[nodeId].childNodeIds)
            {
                if (childNodeId == originalChildId) { continue; }

                fitmentNode newNode = new fitmentNode();


                newNode.fitmentElementType = nodes[nodeId].fitmentElementType;
                newNode.fitmentElementData = nodes[nodeId].fitmentElementData;
                newNode.fitmentElementString = nodes[nodeId].fitmentElementString;
                newNode.graphicalWidth = nodes[nodeId].graphicalWidth;
                newNode.graphicalHeight = nodes[nodeId].graphicalHeight;
                newNode.childNodeIds = new List<int>();
                newNode.childNodeIds.Add(childNodeId);
                newNode.pathFromRoot = new List<int>();
                newNode.nodeId = nodes.Count();
                newNode.parentNode = nodes[nodeId].parentNode;
                nodes.Add(newNode);

                //add new node to the childlist of passed-in node's parent
                nodes[parentNodeId].childNodeIds.Add(newNode.nodeId);

                //redirect current child node to point to new node as its parent
                nodes[childNodeId].parentNode = newNode.nodeId;

                // make note of child nodes to be removed from passed-in node's children list
                removedChildren.Add(childNodeId);
            }

            foreach (int childNodeId in removedChildren)
            {
                nodes[nodeId].childNodeIds.Remove(childNodeId);
            }

            // fix all the "pathToRoot" indexes
            for (i = 0; i <= nodes.Count() - 1; i++)
            {
                if (nodes[i].deleted) { continue; }
                nodes[i].pathFromRoot.Clear();
                nodes[i].pathFromRoot.Add(nodes[i].nodeId); //add "self" as thr first step 
                parentNodeId = i;
                while (true)
                {
                    if (nodes[parentNodeId].parentNode == 0) { break; }
                    nodes[i].pathFromRoot.Add(nodes[parentNodeId].parentNode);
                    parentNodeId = nodes[parentNodeId].parentNode;
                }
            }
            return true;
        }



        public bool fitmentTreeMergeRedundantChildren(List<fitmentNode> nodes, int parentNodeId)
        {
            int i;
            bool modifiedTree = false;
            bool foundPair = true;
            int adoptedNodeId = 0;
            while (foundPair)
            {
                foundPair = false;
                foreach (int childNodeId in nodes[parentNodeId].childNodeIds)
                {
                    if (nodes[childNodeId].deleted) { continue; }
                    foreach (int tempNodeId in nodes[parentNodeId].childNodeIds)
                    {
                        if (tempNodeId == childNodeId || nodes[tempNodeId].deleted) { continue; }
                        if (nodes[childNodeId].fitmentElementData == nodes[tempNodeId].fitmentElementData && nodes[childNodeId].fitmentElementType == nodes[tempNodeId].fitmentElementType && nodes[childNodeId].fitmentElementString == nodes[tempNodeId].fitmentElementString)
                        {// found a redundant node pair "childNodeId" (outer "for" loop) will be the survivor
                            nodes[childNodeId].childNodeIds.AddRange(nodes[tempNodeId].childNodeIds); // add the dying branche's child list to the survivor's childlist
                            foreach (int adoptedChildId in nodes[tempNodeId].childNodeIds) { nodes[adoptedChildId].parentNode = childNodeId; } // fix all the newly-adopted children's parent id
                            nodes[tempNodeId].deleted = true; foundPair = true; modifiedTree = true;
                            adoptedNodeId = tempNodeId;
                            break;
                        }
                    }
                    if (foundPair) { break; }
                }
                // adjust the parent (passed-in) child list to remove the dying branch
                if (foundPair) { nodes[parentNodeId].childNodeIds.Remove(adoptedNodeId); }
            }

            if (modifiedTree)
            {// fix all the "pathToRoot" indexes
                for (i = 0; i <= nodes.Count() - 1; i++)
                {
                    if (nodes[i].deleted) { continue; }
                    nodes[i].pathFromRoot.Clear();
                    nodes[i].pathFromRoot.Add(nodes[i].nodeId); //add "self" as thr first step 
                    parentNodeId = i;
                    while (true)
                    {
                        if (nodes[parentNodeId].parentNode == 0) { break; }
                        nodes[i].pathFromRoot.Add(nodes[parentNodeId].parentNode);
                        parentNodeId = nodes[parentNodeId].parentNode;
                    }
                }
            }
            return modifiedTree;
        }





        // Return a nodeid that is the best anchor-point for the given app and a list of strings containing the remaining (unconsumed)
        // fitment elements of the app. The first string in the list will be the nodeId (as a string value) to attach to. List elements 1 through n are
        // the fitment elements (like "note\tDRW" and "vcdb\tEngineBase:212" and  "qdb\t240:2")
        // The fitmentElementPrevalence variable is a dictionary of elements like vcdb-EngineBase:3,qdb-240:2,note-W/ 213mm Rear Drum:1
        // the fitment elements from an app are consumed in the order of this dictionary. this way, the elements that are the most common are 
        // laid-in nearest root for the most compact tree (fewest nodes)
        public List<string> fitmentTreeBestPath(List<fitmentNode> nodeList, App app, Dictionary<string, int> fitmentElementPrevalence)
        {
            List<string> returnStrings = new List<string>();
            Dictionary<int, int> pathScores = new Dictionary<int, int>();
            Dictionary<string, int> fitmentElementsDict = new Dictionary<string, int>();
            int prevalence = 0;
            List<string> fitmentElements = new List<string>();
            List<string> fitmentElementsTemp = new List<string>();
            string fitmentElementsDictTempKey = "";
            Dictionary<int, List<String>> unconsumedFitmentByAttachmentPoint = new Dictionary<int, List<string>>();
            fitmentNode dummyNode = new fitmentNode();
            List<string> noteElementsList = new List<string>();

            int highScore = 0; int winningNode = 0; int pathScore; bool pathMisMatch;

            foreach (VCdbAttribute myAttribute in app.VCdbAttributes)
            {
                prevalence = 0; fitmentElementPrevalence.TryGetValue("vcdb-" + myAttribute.name, out prevalence);
                fitmentElementsDict.Add("vcdb\t" + myAttribute.name + ":" + myAttribute.value.ToString(), prevalence);
            }

            
            foreach (QdbQualifier myQualifier in app.QdbQualifiers)
            {
                prevalence = 0; fitmentElementPrevalence.TryGetValue("qdb-" + myQualifier.qualifierId.ToString(), out prevalence);

                fitmentElementsDictTempKey="qdb\t" + myQualifier.qualifierId.ToString() + ":" + String.Join(",", myQualifier.qualifierParameters.ToArray());
                if (!fitmentElementsDict.ContainsKey(fitmentElementsDictTempKey))
                {
                    fitmentElementsDict.Add(fitmentElementsDictTempKey, prevalence);
                }
            }
            foreach (String noteString in app.notes)
            {
                prevalence = 0; fitmentElementPrevalence.TryGetValue("note-" + noteString, out prevalence);
                fitmentElementsDictTempKey = "note\t" + noteString;
                if (!fitmentElementsDict.ContainsKey(fitmentElementsDictTempKey))
                {
                    fitmentElementsDict.Add(fitmentElementsDictTempKey, prevalence);
                }
                noteElementsList.Add(noteString);
            }

            foreach (KeyValuePair<string, int> item in fitmentElementsDict.OrderByDescending(key => key.Value)) { fitmentElements.Add(item.Key); }

            foreach (fitmentNode node in nodeList)
            { //iterate through each node's path-to-root list and consume the input app's list of fitment elments as we go.
                if (node.deleted) { continue; }

                fitmentElementsTemp.Clear();
                fitmentElementsTemp.AddRange(fitmentElements);

                if (node.app != null) { continue; }
                pathScore = 0; pathMisMatch = false;
                foreach (int nodeId in node.pathFromRoot)
                {
                    if (fitmentElementsTemp.Contains(nodeList[nodeId].fitmentElementType + "\t" + nodeList[nodeId].fitmentElementData))
                    {
                        pathScore++;
                        fitmentElementsTemp.Remove(nodeList[nodeId].fitmentElementType + "\t" + nodeList[nodeId].fitmentElementData);
                    }
                    else
                    {
                        pathScore = -1000; pathMisMatch = true; break;
                    }
                }

                if (!pathMisMatch)
                {
                    pathScores.Add(node.nodeId, pathScore);
                    List<String> unconsumedList = new List<string>(); unconsumedList.AddRange(fitmentElementsTemp);
                    unconsumedFitmentByAttachmentPoint.Add(node.nodeId, unconsumedList);
                }
            }
            foreach (KeyValuePair<int, int> entry in pathScores)
            {
                if (entry.Value > highScore)
                {
                    highScore = entry.Value; winningNode = entry.Key;
                }
            }
            returnStrings.Add(winningNode.ToString());
            if (winningNode > 0)
            {
                returnStrings.AddRange(unconsumedFitmentByAttachmentPoint[winningNode]);
            }
            else
            {
                returnStrings.AddRange(fitmentElements);
            }
            return returnStrings;
        }


        public bool fitmentNodeBoxesGraphicallyOverlap(List<fitmentNode> nodes)
        {
            foreach (fitmentNode thisnode in nodes)
            {
                if (thisnode.nodeId == 0) { continue; }
                foreach (fitmentNode thatnode in nodes)
                {
                    if (thisnode.nodeId == thatnode.nodeId || thatnode.nodeId == 0) { continue; }
                    if (
                        (thisnode.graphicalXpos + thisnode.graphicalWidth) >= thatnode.graphicalXpos &&
                        (thisnode.graphicalXpos + thisnode.graphicalWidth) <= (thatnode.graphicalXpos + thatnode.graphicalWidth) &&
                        (thisnode.graphicalYpos + thisnode.graphicalHeight) >= thatnode.graphicalYpos &&
                        (thisnode.graphicalYpos + thisnode.graphicalHeight) <= (thatnode.graphicalYpos + thatnode.graphicalHeight)
                        )
                    { return true; }
                }
            }
            return false;
        }


        public void establishFitmentTreeRoots(bool treatAssetsAsFitment)
        {
            string hashkey = "";
            Dictionary<string, List<App>> fitmentTreeHashtable = new Dictionary<string, List<App>>();

            foreach (App app in apps)
            {//establish hashgroups (mmy/type/position/mfrlabel)
                if (app.action == "D") { continue; } // ignore "Delete" apps
                if (treatAssetsAsFitment)
                {
                    hashkey = app.basevehicleid.ToString() + "\t" + app.parttypeid.ToString() + "\t" + app.positionid + "\t" + app.mfrlabel + "\t" + app.asset + "\t" + app.assetitemorder + "\t" + app.assetitemref + "\t"+ app.brand + "\t" + app.subbrand;
                }
                else
                {
                    hashkey = app.basevehicleid.ToString() + "\t" + app.parttypeid.ToString() + "\t" + app.positionid + "\t" + app.mfrlabel + "\t" + app.brand + "\t" + app.subbrand;
                }

                if (fitmentTreeHashtable.ContainsKey(hashkey)) { App appTemp = new App(); appTemp = app; fitmentTreeHashtable[hashkey].Add(appTemp); }
                else
                {// first time seeing this hashkey
                    List<App> appListTemp = new List<App>();
                    App appTemp = new App();
                    appTemp = app;
                    appListTemp.Add(appTemp);
                    fitmentTreeHashtable[hashkey] = appListTemp;
                }
            }

            int groupNumber=1;
            // transscribe groups into "chunks" and add them to the list
            foreach(KeyValuePair<string,List<App>> entry in fitmentTreeHashtable)
            {
                if(entry.Value.Count()>1)
                {
                    analysisChunk chunk = new analysisChunk();
                    chunk.appsList = entry.Value;
                    chunk.id = groupNumber;
                    chunk.appgroupHashKey = entry.Key;
                    fitmentAnalysisChunksList.Add(chunk);
                    groupNumber++;
                }
            }
        }


        // Treat the apps that hang on a [mmy/parttype/position/mfrlable/asset] as nodes in a directory tree.
        // the goal is to be able to compare branches for compatibility.
        // Every node in the tree is represented by an instance of a class called fitmentNode.
        // fitmentNode has a pair of "int" members (nodeId and parentNodeId) to position it in the tree. A node with nodeId=0  
        // is root (there can only be one). Remember that this concept of "root" is just local root within the scope of a specific mmy/parttype/position/mfrlable/asset.
        // We would typically see several apps (more than 10 would be pretty surprising) at this level. Multiple nodes can have "0" as parent.
        // The fitmentElementType member is used to store a single fitment element (ie: "Submodel:912" or "w/ 215mm rear disc"). 
        // fitmentElementType differentiates vcdb vs note vs qdb fitment enelents - this so that branches can be compared for compatibility 
        // (ie: mixing vcdb and note or vcdb and qdb) on the same branch would be poor form but not a critical problem. 
        // The "App" member is used to store the app hanging on the end of the branch (nodes 1,2,5,7,10,12,15 in the diagram below).
        // nodes 3,4,6,8,9,11,13,14 have null in the App member. The absense of an app (member contains null) indicates that the node is not at a branch end.
        // A different (more generic) design approach would be to hang "partNumber" on the end of branches instead of "app". We chose to terminate with "app"
        // because the "app" class can carry with it all of the contextual data - such as the realtive XML node id - that the data originated from.
        //
        // Apps are placed on the tree based on the VCdb,Qdb and Note fitment elements they have. An app with no VCdb/Qdb/Note 
        // attributes would be stored in a node that has 0 as its parent (node "1" in the diagram below) and null fitmentElementData and fitmentElementType.
        // 
        // There are two CNC errors in the tree below: The root contains one (1 vs 4 & 9) and the other is at node 14 as it branches to 13 & 15. 
        // Notice the "Turbo" appears in two different place in the tree. This illustrates that "lineage" back to root must be taken into account 
        // while attching apps to the tree - in other words, we aren't simply looking for any occurance of "Turbo" while attaching an app. We ar looking for a
        // lineage path from root hat includes all the same fitment elements as the app in hand.

        //  Jeep/Cherokee/2001/Brakepad/front/Premium
        //                                          (0)
        //                                           |
        //                                           |-(1)-- MF123 
        //                                           |   
        //                                           |-(4)---FWD---------
        //                                           |                   |-(3)--Turbo------------------     
        //                                           |                   |                             |-(2)--MF345
        //                                           |                   |
        //                                           |                   |-(6)---Naturally Aspirated---
        //                                           |                                                 |-(5)--MF559
        //                                           |-(9)---4WD---------
        //                                                               |-(8)----L4 2.5L-----
        //                                                               |                    |-(7)-- MF699
        //                                                               |
        //                                                               |-(11)----L6 4.0L----
        //                                                               |                    |-(10)-- MF657
        //                                                               |
        //                                                               |-(14)----L6 5.0L----
        //                                                                                    |-(13)--Turbo---
        //                                                                                    |               |-(12)-- MF444
        //                                                                                    |
        //                                                                                    |-(15)-- MF333
        //
        //  If we needed to add another app that was qualified only by "L6 5.0L", it would attach as a new branch from root as node id 17, and the endnode (containing the app)
        // would attach to 17 as 16. Notice that the existing "L6 5.0L" node (14) would not factor in because its lineage includes "4WD" and the new app does not include "4WD".
        // By contrast, if our new app had been qualified by "L6 5.0L" and "4WD" it would attach as a single new endpoint node (16) as a new third branch from node 14.
        // If the new app had been qualified by "FWD" and "L6 5.0L" a new node called "L6 5.0L" would be added as a third branch from 4 and then the endnode containing
        // the app reference would attach it. Notice that the sequence of engine vs drivetype were deliberately opposite in previous two examples. This is to illustrate  
        // that sequence is immaterial. All fitment elements are simply "anded" together to qualify a part's connection to a vehicle. 
        // The key function (method) required for finding attachment points is "findBestPath". It identifies a path to cunsume some or all of the fitment elements of the 
        // app and returns the un-cunsumed elements as a list. It will identify the existing path (defined by one specific attachment node) for an app to attach to. 
        // it does this by testing and scoring all the possible paths away from root in the existing tree. The path that includes the hishest number of the app's fitment elemets is
        // considered the winner.
        // If the identified path only addresses some of the fitment elements of the new app being added, the remainder must be added from the identified attachment point and the 
        // app-containing endnode tacked on at the end. If a path with no un-consumed (remaining) fitment elements is identified, the app-containing endnode would attach directly
        // to it without contributing any additional nodes. This scenario would be a classic "overlap". findBestPath result may be "root" with all original fitment elements 
        // remaining - this would be the case when attaching the first app the the tree. It would also be the case in our example diagram for the attachment of MF345. 
        // Because the tree was mostly empty when MF345 was presented, bestPath would be identified as the path ending at "root" and "FWD"+"Turbo" would remain to then
        // be attached in sequence at root.
        public void findFitmentLogicProblems(analysisChunkGroup chunkGroup, VCdb vcdb, PCdb pcdb, Qdb qdb, string permutationCacheFilePath, int iterationLimit, string cacheDirectory, bool concernForDisparates, bool respectQdbType)
        {
            logHistoryEvent("", "5\tLooking for fitment logic problems");

            int i = 0; string hashkey = "";
            List<string> fitmentElements = new List<string>();
            List<fitmentNode> nodesList = new List<fitmentNode>();
            VCdbAttribute myAttribute = new VCdbAttribute();
            Dictionary<string, int> fitmentElementPrevalence = new Dictionary<string, int>();
            int badness; int lowestBadnessScore; int smallestNodeCount;
            bool addedToFitmentPermutationMineingCache = false;
            int iterationCount = 0; bool cacheHit;
            List<string> problemLinesForCacheFile = new List<string>();
            List<analysisChunk> chunks = new List<analysisChunk>();
            
            //--------------------------
            for (int chunkIndex = 0; chunkIndex < chunkGroup.chunkList.Count(); chunkIndex++)
            {
                analysisChunk chunk = chunkGroup.chunkList[chunkIndex];
                String[] hashkeyChunks = chunk.appgroupHashKey.Split('\t');
                hashkey = hashkeyChunks[0] + "," + hashkeyChunks[1] + "," + hashkeyChunks[2]; // hashkey for the group originally inluded mfrlable and assets - strip it back to mmy/parttype/position. this is granular enough for caching the fitment element precadence

                nodesList.Clear();
                badness = 0;
                fitmentElements.Clear();

                foreach (App appTemp in chunk.appsList)
                {
                    foreach (VCdbAttribute attributeTemp in appTemp.VCdbAttributes) { if (!fitmentElements.Contains("vcdb-" + attributeTemp.name)) { fitmentElements.Add("vcdb-" + attributeTemp.name); } }
                    foreach (QdbQualifier myQualifier in appTemp.QdbQualifiers) { if (!fitmentElements.Contains("qdb-" + myQualifier.qualifierId.ToString())) { fitmentElements.Add("qdb-" + myQualifier.qualifierId.ToString()); } }
                    foreach (String noteString in appTemp.notes) { if (!fitmentElements.Contains("note-" + noteString)) { fitmentElements.Add("note-" + noteString); } }
                }

                //// build a tree optimized by fitmentElementPrevalence and see if it contains any bad branches
                //fitmentElementPrevalence.Clear(); cacheHit = false;
                //if (fitmentPermutationMiningCache.ContainsKey(hashkey))
                //{// cache-hit - use the cached sequence as the guide for organizing this tree
                //    i = 0; foreach (string fitmentElementTemp in fitmentPermutationMiningCache[hashkey].Split('|'))
                //    {
                //        fitmentElementPrevalence.Add(fitmentElementTemp, i); i++;
                //    }
                //    cacheHit = true;
                //}
                //else
                //{// cache-miss - construct the fitmentElementPrevalence list by popularity contest

                    foreach (App appTemp in chunk.appsList)
                    {
                        foreach (VCdbAttribute attributeTemp in appTemp.VCdbAttributes)
                        {
                            if (fitmentElementPrevalence.ContainsKey("vcdb-" + attributeTemp.name)) { fitmentElementPrevalence["vcdb-" + attributeTemp.name]++; } else { fitmentElementPrevalence.Add("vcdb-" + attributeTemp.name, 1); }
                        }
                        foreach (QdbQualifier myQualifier in appTemp.QdbQualifiers)
                        {
                            if (fitmentElementPrevalence.ContainsKey("qdb-" + myQualifier.qualifierId.ToString())) { fitmentElementPrevalence["qdb-" + myQualifier.qualifierId.ToString()]++; } else { fitmentElementPrevalence.Add("qdb-" + myQualifier.qualifierId.ToString(), 1); }
                        }
                        foreach (String noteString in appTemp.notes)
                        {
                            if (fitmentElementPrevalence.ContainsKey("note-" + noteString)) { fitmentElementPrevalence["note-" + noteString]++; } else { fitmentElementPrevalence.Add("note-" + noteString, 1); }
                        }
                    }
                //}

                var fitmentElementPrevalenceTemp = fitmentElementPrevalence.OrderBy(x => x.Value).ToList();
                chunk.lowestBadnessPermutation.Clear(); foreach (KeyValuePair<string, int> prevalenceEntry in fitmentElementPrevalenceTemp) { chunk.lowestBadnessPermutation.Add(prevalenceEntry.Key); }

                nodesList.AddRange(buildFitmentTreeFromAppList(chunk.appsList, fitmentElementPrevalence, -1, false, false, vcdb, qdb));
                badness = fitmentTreeTotalBadBranches(nodesList, qdb, concernForDisparates, respectQdbType);

                /*
                if (badness > 0 && fitmentElements.Count() < 8)
                {// iterate through all permutations of the fitment elements in this group of apps and construct a tree optimized by each and test it for size
                    // record the record low node count and make note of the order of fitment elements so it can be reused later
                    smallestNodeCount = 10000; iterationCount = 0;
                    foreach (String[] permutation in Permutations(fitmentElements.ToArray()))
                    {
                        fitmentElementPrevalence.Clear();
                        i = 0; foreach (string fitmentElementTemp in permutation) { fitmentElementPrevalence.Add(fitmentElementTemp, i); i++; }
                        nodesList.Clear(); nodesList.AddRange(buildFitmentTreeFromAppList(chunk.appsList, fitmentElementPrevalence, smallestNodeCount, false, vcdb, qdb));
                        if (nodesList.Count == 0) { continue; }// treebuilder bailed out because its tree had grown beyond previoulsy estabished record small
                        badness = fitmentTreeTotalBadBranches(nodesList, concernForDisparates);
                        if (nodesList.Count() < smallestNodeCount) { smallestNodeCount = nodesList.Count(); chunk.lowestBadnessPermutation = permutation.ToList(); }
                        if (badness == 0)
                        {
                            lowestBadnessScore = badness; chunk.lowestBadnessPermutation = permutation.ToList();

                            // write this discovered permutation to local cachefile
                            //if (!fitmentPermutationMiningCache.ContainsKey(chunk.appgroupHashKey.Replace('\t', ',')))
                            //{
                            //    fitmentPermutationMiningCache.Add(chunk.appgroupHashKey.Replace('\t', ','), string.Join("|", permutation));
                            //    addedToFitmentPermutationMineingCache = true;
                            //}

                            break;
                        }
                        iterationCount++;
                        if (iterationCount > iterationLimit) { break; }
                    }
                    // render the final nodeslist using the lowestBadnessPermutation list
                    nodesList.Clear();
                    fitmentElementPrevalence.Clear();
                    i = 0; foreach (string fitmentElementTemp in chunk.lowestBadnessPermutation) { fitmentElementPrevalence.Add(fitmentElementTemp, i); i++; }
                    nodesList.AddRange(buildFitmentTreeFromAppList(chunk.appsList, fitmentElementPrevalence, -1, false, vcdb, qdb));
                }


                badness = fitmentTreeTotalBadBranches(nodesList, concernForDisparates);
                */

                if (badness > 0)
                {
                    chunkGroup.errorsCount++;
                    chunk.problemAppsList = badbrabchAppsFromNodelist(nodesList, qdb, concernForDisparates, respectQdbType);// extract the subset of apps that participate in a problem
                    chunk.problems.Add(badness.ToString());
                    chunk.problemsDescription=fitmentTreeProblemDescription(nodesList, qdb, concernForDisparates, respectQdbType);
                }
            }
            chunkGroup.complete = true;

         // if (addedToFitmentPermutationMineingCache){writeFitmentPermutationMiningCache(permutationCacheFilePath); addedToFitmentPermutationMineingCache = false;}

        }



        public List<int> fitmentNodeIdsAtLevel(List<fitmentNode> nodesList, int level)
        {
            List<int> returnList = new List<int>();

            if (level == 0) { returnList.Add(0); return returnList; } // level 0 (root) is a special case - it alwas only contains node[0] 
            List<int> listA = new List<int>(); listA = nodesList[0].childNodeIds;
            foreach (int nodeIdA in listA)
            {
                if (nodesList[nodeIdA].deleted) { continue; }
                if (level == 1) { returnList.Add(nodesList[nodeIdA].nodeId); }

                List<int> listB = new List<int>(); listB = nodesList[nodeIdA].childNodeIds;
                foreach (int nodeIdB in listB)
                {
                    if (nodesList[nodeIdB].deleted) { continue; }
                    if (level == 2) { returnList.Add(nodesList[nodeIdB].nodeId); }

                    List<int> listC = new List<int>(); listC = nodesList[nodeIdB].childNodeIds;
                    foreach (int nodeIdC in listC)
                    {
                        if (nodesList[nodeIdC].deleted) { continue; }
                        if (level == 3) { returnList.Add(nodesList[nodeIdC].nodeId); }

                        List<int> listD = new List<int>(); listD = nodesList[nodeIdC].childNodeIds;
                        foreach (int nodeIdD in listD)
                        {
                            if (nodesList[nodeIdD].deleted) { continue; }
                            if (level == 4) { returnList.Add(nodesList[nodeIdD].nodeId); }

                            List<int> listE = new List<int>(); listE = nodesList[nodeIdD].childNodeIds;
                            foreach (int nodeIdE in listE)
                            {
                                if (nodesList[nodeIdE].deleted) { continue; }
                                if (level == 5) { returnList.Add(nodesList[nodeIdE].nodeId); }

                                List<int> listF = new List<int>(); listF = nodesList[nodeIdE].childNodeIds;
                                foreach (int nodeIdF in listF)
                                {
                                    if (nodesList[nodeIdF].deleted) { continue; }
                                    if (level == 6) { returnList.Add(nodesList[nodeIdF].nodeId); }

                                    List<int> listG = new List<int>(); listG = nodesList[nodeIdF].childNodeIds;
                                    foreach (int nodeIdG in listG)
                                    {
                                        if (nodesList[nodeIdG].deleted) { continue; }
                                        if (level == 7) { returnList.Add(nodesList[nodeIdG].nodeId); }

                                        List<int> listH = new List<int>(); listH = nodesList[nodeIdG].childNodeIds;
                                        foreach (int nodeIdH in listH)
                                        {
                                            if (nodesList[nodeIdH].deleted) { continue; }
                                            if (level == 8) { returnList.Add(nodesList[nodeIdH].nodeId); }

                                            List<int> listI = new List<int>(); listI = nodesList[nodeIdH].childNodeIds;
                                            foreach (int nodeIdI in listI)
                                            {
                                                if (nodesList[nodeIdI].deleted) { continue; }
                                                if (level == 9) { returnList.Add(nodesList[nodeIdI].nodeId); }

                                                List<int> listJ = new List<int>(); listJ = nodesList[nodeIdI].childNodeIds;
                                                foreach (int nodeIdJ in listJ)
                                                {
                                                    if (nodesList[nodeIdJ].deleted) { continue; }
                                                    if (level == 10) { returnList.Add(nodesList[nodeIdJ].nodeId); }

                                                    List<int> listk = new List<int>(); listk = nodesList[nodeIdJ].childNodeIds;
                                                    foreach (int nodeIdK in listk)
                                                    {
                                                        if (nodesList[nodeIdK].deleted) { continue; }
                                                        if (level == 11) { returnList.Add(nodesList[nodeIdK].nodeId); }

                                                        List<int> listl = new List<int>(); listl = nodesList[nodeIdK].childNodeIds;
                                                        foreach (int nodeIdL in listl)
                                                        {
                                                            if (nodesList[nodeIdL].deleted) { continue; }
                                                            if (level == 12) { returnList.Add(nodesList[nodeIdL].nodeId); }

                                                            //xxx

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return returnList;
        }

        public Pen fitmentBranchPen(int targetNodeId, List<fitmentNode> nodesList,bool detectIncompatibleBranches, Qdb qdb, bool respectQdbType)
        {
            Pen returnValue;
            switch (fitmentBranchCompare(targetNodeId, nodesList, qdb, detectIncompatibleBranches, respectQdbType))
            {
                case 0: { returnValue = new Pen(Brushes.Black, 2); break; }
                case 1: { returnValue = new Pen(Brushes.DarkOliveGreen, 3); break; }
                case 2: { returnValue = new Pen(Brushes.Brown, 3); break; }
                case 3: { returnValue = new Pen(Brushes.Yellow, 3); break; }
                case 4: { returnValue = new Pen(Brushes.DarkOrange, 3); break; }
                case 5: { returnValue = new Pen(Brushes.Red, 3); break; }
                default: returnValue = new Pen(Brushes.Gray, 3); break;
            }
            return returnValue;
        }


        // ??? note - when adding a new app via the "best path" tester, maybe skip the best score of a lesser is the lesser avoids creating an invalid branch ???


        // target a specific node in a tree and inspect the branches that are its immediate children for validity
        //VCdbSysX - VCdbSysX   valid                           return 0
        //Qdb - Qdb             valid                           return 0
        //Note - Note           valid                           return 0
        //VCdb - Qdb            Poor form                       return 1
        //VCdb - Note           Poor form                       return 2
        //Qdb - Note            questionable                    return 2
        //VCdb - Part           bad (clasic "CNC")              return 3
        //Qdb - Part            bad (clasic "CNC")              return 3
        //Note - Part           bad (clasic "CNC")              return 3
        //Part - Part           bad (simple overlap)            return 4
        //VCdbSysX - VCdbSysY   bad (ex: EngineBase-DriveType)  return 5
        public int fitmentBranchCompare(int targetNodeId, List<fitmentNode> nodesList, Qdb qdb,bool detectIncompatibleBranches,bool repectQdbType)
        {
            int returnValue = 0;
            List<int> childNodeIds = new List<int>();
            childNodeIds = nodesList[targetNodeId].childNodeIds;
            
            if (childNodeIds.Count() > 1)
            {// this node has multiple children

                List<string> PartsInGroup = new List<string>();
                List<string> VCdbSystemsInGroup = new List<string>();
                List<string> QdbQualifiersInGroup = new List<string>();
                List<string> NotesInGroup = new List<string>();

                foreach (int childNodeId in childNodeIds)
                {// establish list of the fitment types in use

                    if (nodesList[childNodeId].fitmentElementType == "vcdb")
                    {
                        string[] elementChunks = nodesList[childNodeId].fitmentElementData.Split(':');
                        if (elementChunks.Count() > 0) { if (!VCdbSystemsInGroup.Contains(elementChunks[0])) { VCdbSystemsInGroup.Add(elementChunks[0]); } }
                    }
                    if (nodesList[childNodeId].fitmentElementType == "qdb")
                    {
                        string[] elementChunks = nodesList[childNodeId].fitmentElementData.Split(':');
                        int QdbIDtemp = Convert.ToInt32(elementChunks[0]);
                        int qualifierTypeID = 0;
                        qdb.qualifiersTypes.TryGetValue(QdbIDtemp, out qualifierTypeID);
                  
                        if(qualifierTypeID==1 || !repectQdbType)
                        {// type 1 Qdb's are "fitment" the other types are informational and can be ignored for the purpose of scoring branches
                            QdbQualifiersInGroup.Add(nodesList[childNodeId].fitmentElementData);
                        }

                    }
                    if (nodesList[childNodeId].fitmentElementType == "note")
                    {
                        NotesInGroup.Add(nodesList[childNodeId].fitmentElementString);
                    }
                    if (nodesList[childNodeId].fitmentElementType == "part")
                    {
                        //if (!PartsInGroup.Contains(nodesList[childNodeId].app.part)) { PartsInGroup.Add(nodesList[childNodeId].app.part); }
                        PartsInGroup.Add(nodesList[childNodeId].app.part); 
                    }
                }


                if ((VCdbSystemsInGroup.Count() + QdbQualifiersInGroup.Count() + NotesInGroup.Count() + PartsInGroup.Count()) > 1)
                {// target node has children with miltiple types - quantify them and set return value accordingly

                    returnValue = 6; // default to unknown (bad)

                    while (true)
                    {
                        //VCdbSysX - VCdbSysY
                        if (VCdbSystemsInGroup.Count() > 1) { returnValue = 5; break; }

                        //multiple parts (overlap)
                        if (PartsInGroup.Count() > 1) { returnValue = 4; break; }

                        // Part - (VCdb|Qdb|Note)  - Literal CNC (blank qualifid and non-blank qualified)
                        if ((VCdbSystemsInGroup.Count() + QdbQualifiersInGroup.Count() + NotesInGroup.Count()) > 0 && PartsInGroup.Count() > 0) 
                        {
                            returnValue = 3; break; 
                        }

                        // Note - Part branches
                        if (VCdbSystemsInGroup.Count() == 0 && QdbQualifiersInGroup.Count() == 0 && NotesInGroup.Count() > 0 && PartsInGroup.Count() > 0) { returnValue = 3; break; }

                        // Qdb - Part branches
                        if (VCdbSystemsInGroup.Count() == 0 && QdbQualifiersInGroup.Count() > 0 && NotesInGroup.Count() == 0 && PartsInGroup.Count() > 0)
                        { 
                            returnValue = 3; break; 
                        }

                        // VCdb - Part branches
                        if (VCdbSystemsInGroup.Count() > 0 && QdbQualifiersInGroup.Count() == 0 && NotesInGroup.Count() == 0 && PartsInGroup.Count() > 0) { returnValue = 3; break; }

                        // Qdb - Note branches
                        if (VCdbSystemsInGroup.Count() == 0 && QdbQualifiersInGroup.Count() > 0 && NotesInGroup.Count() > 0 && PartsInGroup.Count() == 0) { returnValue = 2; break; }

                        // VCdb - Note branches
                        if (VCdbSystemsInGroup.Count() > 0 && QdbQualifiersInGroup.Count() == 0 && NotesInGroup.Count() > 0 && PartsInGroup.Count() == 0) { returnValue = 2; break; }

                        // VCdb - Qdb branches
                        if (VCdbSystemsInGroup.Count() > 0 && QdbQualifiersInGroup.Count() > 0 && NotesInGroup.Count() == 0 && PartsInGroup.Count() == 0) { returnValue = 1; break; }

                        // VCdb - Qdb - Note branches
                        if (VCdbSystemsInGroup.Count() > 0 && QdbQualifiersInGroup.Count() > 0 && NotesInGroup.Count() > 0 && PartsInGroup.Count() == 0) { returnValue = 1; break; }

                        // Note - Note branches
                        if (VCdbSystemsInGroup.Count() == 0 && QdbQualifiersInGroup.Count() == 0 && NotesInGroup.Count() > 0 && PartsInGroup.Count() == 0) { returnValue = 0; break; }

                        // Qdb - Qdb branches
                        if (VCdbSystemsInGroup.Count() == 0 && QdbQualifiersInGroup.Count() > 0 && NotesInGroup.Count() == 0 && PartsInGroup.Count() == 0) { returnValue = 0; break; }

                        break;
                    }
                }
                else
                {// thi scenario covers VCdbSysX - VCdbSysX
                    returnValue = 0;
                }
            }

            if(!detectIncompatibleBranches)
            {// surpress poor-form detection
                if(returnValue==1 || returnValue==2 || returnValue==5){returnValue=0;}
            }
            return returnValue;
        }



        public int fitmentNodeCompare(fitmentNode nodeA, fitmentNode nodeB)
        {
            int returnValue = 6;
            //VCdbSysX - VCdbSysX   valid                           return 0
            //Qdb - Qdb             valid                           return 0
            //Note - Note           valid                           return 0
            //VCdb - Qdb            Poor form                       return 1
            //VCdb - Note           Poor form                       return 2
            //Qdb - Note            questionable                    return 2
            //VCdb - Part           bad (clasic "deep CNC")         return 3
            //Qdb - Part            bad (clasic "deep CNC")         return 3
            //Note - Part           bad (clasic "deep CNC")         return 3
            //Part - Part           bad (simple overlap)            return 4
            //VCdbSysX - VCdbSysY   bad (ex: EngineBase-DriveType)  return 5

            switch (nodeA.fitmentElementType)
            {
                case "vcdb":
                    switch (nodeB.fitmentElementType)
                    {
                        case "vcdb":
                            string[] chunksA = nodeA.fitmentElementData.Split(':'); string[] chunksB = nodeB.fitmentElementData.Split(':');
                            if (chunksA.Count() > 0 && chunksB.Count() > 0) { if (chunksA[0] == chunksB[0]) { returnValue = 0; } else { returnValue = 5; } }
                            break;
                        case "qdb": returnValue = 1; break;
                        case "note": returnValue = 2; break;
                        case "part": returnValue = 3; break;
                        default: break;
                    }
                    break;
                case "qdb":
                    switch (nodeB.fitmentElementType)
                    {
                        case "vcdb": returnValue = 1; break;
                        case "qdb": returnValue = 0; break;
                        case "note": returnValue = 2; break;
                        case "part": returnValue = 3; break;
                        default: break;
                    }
                    break;
                case "note":
                    switch (nodeB.fitmentElementType)
                    {
                        case "vcdb": returnValue = 2; break;
                        case "qdb": returnValue = 2; break;
                        case "note": returnValue = 0; break;
                        case "part": returnValue = 3; break;
                        default: break;
                    }
                    break;
                case "part":
                    switch (nodeB.fitmentElementType)
                    {
                        case "vcdb": returnValue = 3; break;
                        case "qdb": returnValue = 3; break;
                        case "note": returnValue = 3; break;
                        case "part": if (nodeA.fitmentElementString == nodeB.fitmentElementString) { returnValue = 0; } else { returnValue = 4; } break;
                        default: break;
                    }
                    break;
                default: break;
            }
            return returnValue;
        }



        public void fitmentNodeCopy(fitmentNode fromNode, fitmentNode toNode)
        {
            toNode.fitmentElementType = fromNode.fitmentElementType;
            toNode.fitmentElementData = fromNode.fitmentElementData;
            toNode.fitmentElementString = fromNode.fitmentElementString;
            toNode.graphicalWidth = fromNode.graphicalWidth;
            toNode.graphicalHeight = fromNode.graphicalHeight;
            toNode.deleted = fromNode.deleted;
            toNode.filler = fromNode.filler;
            toNode.parentNode = fromNode.parentNode;
            List<int> children = new List<int>();
            List<int> rootPath = new List<int>();
            toNode.childNodeIds = children;
            toNode.pathFromRoot = rootPath;
            toNode.childNodeIds.AddRange(fromNode.childNodeIds);
            toNode.pathFromRoot.AddRange(fromNode.pathFromRoot);
            toNode.app = fromNode.app;
        }





        public bool fitmentNodesSwap(List<fitmentNode> nodes, int nodeAid, int nodeBid)
        {
            int intTemp;
            bool boolTemp;
            string stringTemp;
            //List<int> intListTemp = new List<int>();


            stringTemp = nodes[nodeAid].fitmentElementType;
            nodes[nodeAid].fitmentElementType = nodes[nodeBid].fitmentElementType;
            nodes[nodeBid].fitmentElementType = stringTemp;

            stringTemp = nodes[nodeAid].fitmentElementData;
            nodes[nodeAid].fitmentElementData = nodes[nodeBid].fitmentElementData;
            nodes[nodeBid].fitmentElementData = stringTemp;

            stringTemp = nodes[nodeAid].fitmentElementString;
            nodes[nodeAid].fitmentElementString = nodes[nodeBid].fitmentElementString;
            nodes[nodeBid].fitmentElementString = stringTemp;

            intTemp = nodes[nodeAid].graphicalWidth;
            nodes[nodeAid].graphicalWidth = nodes[nodeBid].graphicalWidth;
            nodes[nodeBid].graphicalWidth = intTemp;

            intTemp = nodes[nodeAid].graphicalHeight;
            nodes[nodeAid].graphicalHeight = nodes[nodeBid].graphicalHeight;
            nodes[nodeBid].graphicalHeight = intTemp;

            boolTemp = nodes[nodeAid].deleted;
            nodes[nodeAid].deleted = nodes[nodeBid].deleted;
            nodes[nodeBid].deleted = boolTemp;

            boolTemp = nodes[nodeAid].filler;
            nodes[nodeAid].filler = nodes[nodeBid].filler;
            nodes[nodeBid].filler = boolTemp;

            return true;
        }



        public bool fitmentNodesCanSwap(List<fitmentNode> nodesList, int nodeAid, int nodeBid)
        {
            // can only swap nodes that are in a straight line together

            if (nodeAid == 0 || nodeBid == 0 || nodesList[nodeAid].app != null || nodesList[nodeBid].app != null || nodeAid == nodeBid) { return false; } // root and endnodes are off-limits for a swap
            int parentNodeId; bool keepWalking;// but you won't knock down our wall!
            int higherNodeId = 0; int lowerNodeId = 0;

            if (nodesList[nodeBid].pathFromRoot.Contains(nodeAid))
            {//nodeA is in the lineage (above) nodeB walk up the levels till we touch "A" and look for branches along the way
                higherNodeId = nodeAid;
                lowerNodeId = nodeBid;
            }
            else
            {//nodeA is NOT in the lineage (above) nodeB now see if the opposite is true
                if (nodesList[nodeAid].pathFromRoot.Contains(nodeBid))
                {//nodeB is in the lineage (above) nodeA
                    higherNodeId = nodeBid;
                    lowerNodeId = nodeAid;
                }
            }

            keepWalking = true;
            parentNodeId = nodesList[lowerNodeId].parentNode;
            while (keepWalking)
            {
                if (nodesList[parentNodeId].childNodeIds.Count() != 1)
                {// this node has siblings 
                    keepWalking = false;
                }
                if (parentNodeId == higherNodeId) { break; }// we have reached the other node


                parentNodeId = nodesList[parentNodeId].parentNode;
            }


            return keepWalking;
        }


        public static IEnumerable<T[]> Permutations<T>(T[] values, int fromInd = 0)
        {
            if (fromInd + 1 == values.Length)
                yield return values;
            else
            {
                foreach (var v in Permutations(values, fromInd + 1))
                    yield return v;

                for (var i = fromInd + 1; i < values.Length; i++)
                {
                    SwapValues(values, fromInd, i);
                    foreach (var v in Permutations(values, fromInd + 1))
                        yield return v;
                    SwapValues(values, fromInd, i);
                }
            }
        }

        private static void SwapValues<T>(T[] values, int pos1, int pos2)
        {
            if (pos1 != pos2)
            {
                T tmp = values[pos1];
                values[pos1] = values[pos2];
                values[pos2] = tmp;
            }
        }








        // for dynamically building small trees (several dozen-ish nodes) from a given list of apps. this is used for overlap detection as well as
        // rendering a tree chart uppon click of the datagrid cell containing the CNC group number.
        public List<fitmentNode> buildFitmentTreeFromAppList(List<App> applist, Dictionary<string, int> fitmentElementPrevalence,int sizeToBeat ,bool humanReadable, bool truncateLongNotes, VCdb vcdb, Qdb qdb)
        {
            int i, parentNodeId, attachmentPointNodeId, nodeIndex;
            List<string> stringListTemp = new List<string>();
            List<fitmentNode> nodesList = new List<fitmentNode>();
            VCdbAttribute myAttribute = new VCdbAttribute();

            // Establish a dictionary of fitment element group prevalence in this app group (all apps in group will be same mmy/parttype/position). 
            // This will be used to prioritize the consumption
            // of elements in the node attachment process for building a tree. Theory being that elements that appear in more apps should be laid 
            // in closer to root so that multiple down-stream branches have more likelyhood of attaching without duplicating an element in seperate branches.
            // Total VCdb count prevalence by name only ("EngineBase") - don't include the specific coded value.
            // Total note prevalence by entire note string.
            // Total Qdb prevalence by quaifierid only - not parameters.

            fitmentNode rootNode = new fitmentNode();
            rootNode.fitmentElementData = "root";
            rootNode.fitmentElementString = applist[0].niceMMYstring(vcdb);
            rootNode.pathFromRoot = new List<int>();
            rootNode.childNodeIds = new List<int>();
            nodesList.Add(rootNode);

            nodeIndex = 1;

            foreach (App appTemp in applist)
            {
                if(sizeToBeat > 0 && nodesList.Count()>sizeToBeat)
                {
                    // abandon this build because it has already grown bigger than a previoulsy established permutations
                    // return an empty set 
                    nodesList.Clear();
                    break;
                }

                stringListTemp = fitmentTreeBestPath(nodesList, appTemp, fitmentElementPrevalence);
                attachmentPointNodeId = Convert.ToInt32(stringListTemp[0]);
                stringListTemp.RemoveAt(0); //element 0 was used to convey the best attachment nodeId back from the bestPath finder function. the strings in the list are fitmentelements needing to be added to that point
                                            // stringListTemp now contains a list of fitment elements that need to have nodes created for them

                foreach (string fitmentElement in stringListTemp)
                {
                    fitmentNode branchNode = new fitmentNode();
                    branchNode.nodeId = nodeIndex;
                    string[] fitmentElementChunks = fitmentElement.Split('\t'); branchNode.fitmentElementType = fitmentElementChunks[0]; branchNode.fitmentElementData = fitmentElementChunks[1];
                    branchNode.parentNode = attachmentPointNodeId;
                    branchNode.pathFromRoot = new List<int>();
                    branchNode.childNodeIds = new List<int>();
                    nodesList.Add(branchNode);
                    attachmentPointNodeId = nodeIndex;
                    nodeIndex++;
                }

                fitmentNode endNode = new fitmentNode();
                endNode.app = appTemp;
                endNode.fitmentElementType = "part";
                endNode.fitmentElementData = appTemp.part;
                endNode.nodeId = nodeIndex;
                endNode.parentNode = attachmentPointNodeId;
                endNode.pathFromRoot = new List<int>();
                endNode.childNodeIds = new List<int>();
                nodesList.Add(endNode);
                nodeIndex++;

                //update the "pathFromRoot" nodelist inside each node. This is the list of nodes that connect the node in question back to root. an empty list indicates that the node's parent is root.
                for (i = 0; i <= nodesList.Count() - 1; i++)
                {
                    nodesList[i].pathFromRoot.Clear();
                    nodesList[i].pathFromRoot.Add(nodesList[i].nodeId); //add "self" as thr first step 
                    parentNodeId = i;
                    while (true)
                    {
                        if (nodesList[parentNodeId].parentNode == 0) { break; }
                        nodesList[i].pathFromRoot.Add(nodesList[parentNodeId].parentNode);
                        parentNodeId = nodesList[parentNodeId].parentNode;
                    }
                }
            }

            // update the childNodeIds index lists inside each node
            Dictionary<int, List<int>> childLists = new Dictionary<int, List<int>>();
            foreach (fitmentNode node in nodesList)
            {
                if (node.parentNode == node.nodeId) { continue; } // ingnore the fact that root is its own parent

                if (childLists.ContainsKey(node.parentNode))
                {
                    childLists[node.parentNode].Add(node.nodeId);
                }
                else
                {
                    List<int> childList = new List<int>();
                    childList.Add(node.nodeId);
                    childLists.Add(node.parentNode, childList);
                }
            }
            foreach (KeyValuePair<int, List<int>> childListEntry in childLists) { nodesList[childListEntry.Key].childNodeIds = childListEntry.Value; }

            if (humanReadable)
            {//translate all nodes' coded values (vcdb and qdb) to human-readable as needed
                for (i = 0; i <= nodesList.Count() - 1; i++)
                {
                    if (i == 0) { continue; } // leave root alone - his fitmentElementString has been set to MMY
                    nodesList[i].fitmentElementString = nodesList[i].fitmentElementData;
                    if (nodesList[i].fitmentElementType == "vcdb")
                    {
                        string[] attributeChuncks = nodesList[i].fitmentElementData.Split(':');
                        if (attributeChuncks.Count() == 2)
                        {
                            myAttribute.name = attributeChuncks[0]; myAttribute.value = Convert.ToInt32(attributeChuncks[1]);
                            nodesList[i].fitmentElementString = vcdb.niceAttribute(myAttribute);
                        }
                    }

                    if (nodesList[i].fitmentElementType == "qdb")
                    {//translate qdb-coded values to human readable
                        string[] qualifierChuncks = nodesList[i].fitmentElementData.Split(':');
                        if (qualifierChuncks.Count() == 2)
                        {// 
                            nodesList[i].fitmentElementString = qdb.niceQdbQualifier(Convert.ToInt32(qualifierChuncks[0]), qualifierChuncks[1].Split(',').ToList());
                        }
                    }
                }
            }

            if(truncateLongNotes)
            {
                for (i = 0; i <= nodesList.Count() - 1; i++)
                {
                    if (i == 0) { continue; } // leave root alone - his fitmentElementString has been set to MMY
                    if (nodesList[i].fitmentElementType == "note" && nodesList[i].fitmentElementString.Length>30)
                    {
                        nodesList[i].fitmentElementString = nodesList[i].fitmentElementString.Substring(0, 19) + "...";
                    }
                }
            }

            return nodesList;
        }

        //-------------------------------- find all objective, single-app type problems that are - thread-safe  -----------------------------------------
        // parttype/Position, Qdb, invalidBaseVids, invalid coded values, invalid configurations
        // these are all lump together into a single function so it can be called as a seperate (in-parrallel) tasks with sub-sections of the full appslists to chew on.
        // The rationale is to maximize multi-core environments. The apps list could be all apps in the primary aces object or a smaller subset.
        //  The found problems will be written to their respective files named by hash and "taskumber" for re-assembly by the calling process
        public void findIndividualAppErrors(analysisChunk chunk, VCdb vcdb, PCdb pcdb, Qdb qdb, bool flagDupQdb)
        {
            string cacheFilename;
            string errorString = "";

            //-------------------------------- partType/position errors --------------------------
            logHistoryEvent("", "5\tLooking for parttype/position errors");
            cacheFilename = chunk.cachefile + "_parttypePositionErrors" + chunk.id.ToString() + ".txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                foreach(App app in chunk.appsList)
                {
                    if (app.action == "D") { continue; } // ignore "Delete" apps
                    errorString = "";
                    if (pcdb.niceParttype(app.parttypeid) == app.parttypeid.ToString())
                    {// parttype id not valid (nice name returned the numeric value of the id)
                        errorString = "Invalid Parttype";
                    }

                    if (app.positionid != 0 && pcdb.nicePosition(app.positionid) == app.positionid.ToString())
                    {// parttype id not valid (nice name returned the numeric value of the id)
                        errorString += " Invalid Position";
                    }

                    if (errorString == "" && app.positionid != 0 && !pcdb.codmasterParttypePoisitions.Contains(app.parttypeid.ToString() + "_" + app.positionid.ToString()))
                    {// this combo of parttype and position was not found in the codemaster table
                        errorString = "Invalid Parttype-Position";
                    }

                    if (errorString != "")
                    {
                        chunk.parttypePositionErrorsCount++;
                        string problemData = errorString + "\t" + app.id.ToString() + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity.ToString() + "\t" + app.part + "\t" + app.niceFullFitmentString(vcdb, qdb);
                        sw.WriteLine(problemData);
                    }
                }
            }
            if (chunk.parttypePositionErrorsCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tError: " + chunk.parttypePositionErrorsCount.ToString() + " invalid parttypes or parttype/positions combinations (task "+chunk.id.ToString()+")"); }  // delete cache file if empty

            
            //----------------------------------------------- Qdb errors ------------------------------------------
            logHistoryEvent("", "5\tLooking for Qdb errors");
            cacheFilename = chunk.cachefile + "_QdbErrors" + chunk.id.ToString() + ".txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                foreach (App app in chunk.appsList)
                {
                    if (app.action == "D") { continue; } // ignore "Delete" apps
                    foreach (QdbQualifier myQdbQualifier in app.QdbQualifiers)
                    {
                        if (qdb.niceQdbQualifier(myQdbQualifier.qualifierId, myQdbQualifier.qualifierParameters) == myQdbQualifier.qualifierId.ToString())
                        {// QdbQualifier id not valid (nice name returned the numeric value of the id)
                            chunk.qdbErrorsCount++;
                            sw.WriteLine("Invalid Qdb id (" + myQdbQualifier.qualifierId.ToString() + ")" + "\t" + app.id + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity + "\t" + app.part + "\t" + app.niceAttributesString(vcdb, false) + "\t" + string.Join(";", app.notes));
                        }
                    }
                }

                if (flagDupQdb)
                {
                    List<int> tempQdbList = new List<int>();
                    foreach (App app in chunk.appsList)
                    {
                        if (app.action == "D") { continue; } // ignore "Delete" apps
                        tempQdbList.Clear();
                        foreach (QdbQualifier myQdbQualifier in app.QdbQualifiers)
                        {
                            if (tempQdbList.Contains(myQdbQualifier.qualifierId))
                            {// qualifier is on the app multiple times 
                                chunk.qdbErrorsCount++;
                                sw.WriteLine("Multiple instances of Qdb (" + myQdbQualifier.qualifierId.ToString() + ") on app" + "\t" + app.id + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity + "\t" + app.part + "\t" + app.niceFullFitmentString(vcdb, qdb) + "\t" + string.Join(";", app.notes));
                            }
                            else
                            {
                                tempQdbList.Add(myQdbQualifier.qualifierId);
                            }
                        }
                    }
                }

            }
            if (chunk.qdbErrorsCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tError: " + chunk.qdbErrorsCount.ToString() + " invalid Qdb references (task " + chunk.id.ToString() + ")"); }  // delete cache file if empty


            //----------------------------------------------- Questioable Notes ---------------------------

            logHistoryEvent("", "5\tLooking for Questionable Notes");
            cacheFilename = chunk.cachefile + "_questionableNotes" + chunk.id.ToString() + ".txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                foreach (App app in chunk.appsList)
                {
                    if (app.action == "D") { continue; } // ignore "Delete" apps
                    foreach (KeyValuePair<string, bool> searchTermEntry in noteBlacklist)
                    {
                        foreach (String note in app.notes)
                        {
                            if (note.Contains(searchTermEntry.Key))
                            {//
                                chunk.questionableNotesCount++;
                                sw.WriteLine("Questionable note (" + note + ")" + "\t" + app.id + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity + "\t" + app.part + "\t" + app.niceAttributesString(vcdb, false) + "\t" + string.Join(";", app.notes));
                            }
                        }
                    }
                }
            }
            if (chunk.questionableNotesCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tError: " + chunk.questionableNotesCount.ToString() + " questionable notes (task " + chunk.id.ToString() + ")"); }  // delete cache file if empty








            //---------------------------------------------- invalid baseVehicles --------------------------------------
            logHistoryEvent("", "5\tLooking for invalid basevehicles");
            BaseVehicle basevidTemp = new BaseVehicle();
            cacheFilename = chunk.cachefile + "_invalidBasevehicles" + chunk.id.ToString() + ".txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                foreach (App app in chunk.appsList)
                {
                    if (app.action == "D") { continue; } // ignore "Delete" apps
                    if (!vcdb.vcdbBasevhicleDict.TryGetValue(app.basevehicleid, out basevidTemp))
                    {
                        chunk.basevehicleidsErrorsCount++;
                        sw.WriteLine(app.id + "\t" + app.basevehicleid.ToString() + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity + "\t" + app.part + "\t" + app.niceFullFitmentString(vcdb, qdb));
                    }
                }
            }
            if (chunk.basevehicleidsErrorsCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tError: " + chunk.basevehicleidsErrorsCount.ToString() + " invalid basevehicles (task "+chunk.id.ToString()+")"); } // delete cache file if empty


            //---------------------------------------------- invalid vcdb-coded attributes ------------------------------
            logHistoryEvent("", "5\tLooking for invalid vcdb-coded values");
            cacheFilename = chunk.cachefile + "_invalidVCdbCodes" + chunk.id.ToString() + ".txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                foreach (App app in chunk.appsList)
                {
                    if (app.action == "D") { continue; } // ignore "Delete" apps
                    if (app.VCdbAttributes.Count > 0)
                    {
                        errorString = "";
                        foreach (VCdbAttribute myAttribute in app.VCdbAttributes)
                        {
                            if (!vcdb.validAttribute(myAttribute))
                            {
                                errorString += myAttribute.name + ":" + myAttribute.value.ToString() + ";";
                            }
                        }

                        if (errorString != "")
                        {
                            chunk.vcdbCodesErrorsCount++;
                            sw.WriteLine(app.id + "\t" +  app.reference + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity + "\t" + app.part + "\t" + app.niceAttributesString(vcdb, false) + "\t" + String.Join(";", app.notes));
                        }
                    }
                }
            }
            if (chunk.vcdbCodesErrorsCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tError: " + chunk.vcdbCodesErrorsCount.ToString() + " invalid vcdb-coded values (task " + chunk.id.ToString() + ")"); } // delete cache file if empty

            //------------------------------------------------ invalid vehicle configurations --------------------------------------------------
            
            bool appHasInvalidAttribute;
            List<VCdbAttribute> dummyAttributesList = new List<VCdbAttribute>();
            App dummyApp = new App();
            cacheFilename = chunk.cachefile + "_configurationErrors" + chunk.id.ToString() + ".txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                foreach (App app in chunk.appsList)
                {
                    if (app.action == "D") { continue; } // ignore "Delete" apps
                    if (app.VCdbAttributes.Count > 0 && vcdb.vcdbBasevhicleDict.ContainsKey(app.basevehicleid))
                    {
                        appHasInvalidAttribute = false; foreach (VCdbAttribute myAttribute in app.VCdbAttributes) { if (!vcdb.validAttribute(myAttribute)) { appHasInvalidAttribute = true; } }
                        if (!appHasInvalidAttribute)
                        {// dont include invalid attributed apps in vcdb config analysis - these are handled in the "invalid attributes" analysis

                            if (!vcdb.configIsValidMemoryBased(app))
                            {   // this apps's combination of attribute values is not found in the specified VCdb.
                                // create a new list of attributes for this app that has excluded any attribute where "U/K" is an option

                                if (allowGraceForWildcardConfigs)
                                {
                                    dummyAttributesList.Clear();
                                    foreach (VCdbAttribute attribute in app.VCdbAttributes)
                                    {
                                        if (!vcdb.attributeHasWildcardInConfig(app.basevehicleid, attribute.name))
                                        {
                                            dummyAttributesList.Add(attribute);
                                        }
                                    }

                                    dummyApp.VCdbAttributes = dummyAttributesList;
                                    dummyApp.basevehicleid = app.basevehicleid;
                                    if (!vcdb.configIsValidMemoryBased(dummyApp))
                                    {
                                        chunk.vcdbConfigurationsErrorsCount++;
                                        sw.WriteLine(app.id + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity + "\t" + app.part + "\t" + app.niceAttributesString(vcdb, false) + "\t" + app.niceQdbQualifierString(qdb) + "\t" + string.Join(";", app.notes));
                                    }
                                }
                                else
                                {// no wildcard grace
                                    chunk.vcdbConfigurationsErrorsCount++;
                                    sw.WriteLine(app.id + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity + "\t" + app.part + "\t" + app.niceAttributesString(vcdb, false) + "\t" + app.niceQdbQualifierString(qdb) + "\t" + string.Join(";", app.notes));
                                }

                            }
                        }
                    }
                }
            }
            if (chunk.vcdbConfigurationsErrorsCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tError: " + chunk.vcdbConfigurationsErrorsCount.ToString() + " invalid vcdb configurations (task " + chunk.id.ToString() + ")"); } // delete cache file if empty
            chunk.complete = true;
            
        }





        // individual apps that MAY be problems because they are not typical of their neighbors
        //  ------------------- THIS ANALYSIS SECTION IS NOT THREAD SAFE ----------------------------------
        // this has to be run for entire apps list - otherwise, "neighbors" end up sectioned off and not considered together
        // the entire aces.apps list will be conveyed to this function in the "chunk" mechainsm so that its completion can be tracked along with the 
        // other thread-safe analysis chunks
        public void findIndividualAppOutliers(analysisChunk chunk, VCdb vcdb, PCdb pcdb, Qdb qdb)
        {
            //----------------------------------------- parttypeDisagreement --------------------------
            // look for parts that are used in multiple parttypes 
            logHistoryEvent("", "5\tLooking for part-type disagreements");
            int warningsCount = 0; string cacheFilename = chunk.cachefile + "_parttypeDisagreements.txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                List<string> partstypeNamesList = new List<string>();
                foreach (KeyValuePair<string, List<int>> entry in partsPartTypes)
                {
                    partstypeNamesList.Clear();
                    if (entry.Value.Count() > 1)
                    {
                        chunk.parttypeDisagreementErrorsCount++;
                        foreach (int parttypeIdtemp in entry.Value)
                        {
                            partstypeNamesList.Add(pcdb.niceParttype(parttypeIdtemp));
                        }
                        sw.WriteLine(entry.Key + "\t" + String.Join(",", partstypeNamesList));
                    }
                }
            }
            if(chunk.parttypeDisagreementErrorsCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tWarning: " + warningsCount.ToString() + " part-type disagreements"); } // delete cache file if empty

            //----------------------------------------- qtyOutliers --------------------------
            // establish two hashtables <string,int>
            //  - appcounts by parttype/position/qty
            //  - appcounts by parttype/position
            // establish a results hashtable <string,double> that is the percentage of (parttype/position/qty) within (parttype/position)
            // roll through all apps and lookup thier quantity "outlierness" in the <string,double> table and record them on the output string list if they are below prevelence threshold
            logHistoryEvent("", "5\tLooking for quantity outliers (" + qtyOutlierThreshold.ToString() + "%, " + qtyOutlierSampleSize.ToString() + ")");
            Dictionary<string, int> parttypePositionDict = new Dictionary<string, int>();
            Dictionary<string, int> parttypePositionQtyDict = new Dictionary<string, int>();
            Dictionary<string, Double> parttypePositionQtyPrevelence = new Dictionary<string, Double>();

            Double outliernessThreshold = Convert.ToDouble(qtyOutlierThreshold / 100);
            string hashKey = "";
            string hashKeyTypePosition = "";

            foreach (App app in chunk.appsList)
            {
                if (app.action == "D") { continue; } // ignore "Delete" apps
                hashKey = app.parttypeid.ToString() + "\t" + app.positionid.ToString() + "\t" + app.quantity.ToString();
                if (parttypePositionQtyDict.ContainsKey(hashKey))
                {// seen this type/position/qty before. increment the counter
                    parttypePositionQtyDict[hashKey] += 1;
                }
                else
                {//brandnew  type/position/qty combination. establish the key and set value = 1;
                    parttypePositionQtyDict[hashKey] = 1;
                }
            }

            foreach (KeyValuePair<string, int> dictEntry in parttypePositionQtyDict)
            {
                String[] chuncks = dictEntry.Key.Split('\t');
                hashKey = chuncks[0] + "\t" + chuncks[1];
                if (parttypePositionDict.ContainsKey(hashKey)) { parttypePositionDict[hashKey] += dictEntry.Value; } else { parttypePositionDict[hashKey] = dictEntry.Value; }
            }

            foreach (KeyValuePair<string, int> dictEntry in parttypePositionQtyDict)
            {
                String[] chuncks = dictEntry.Key.Split('\t');
                hashKey = chuncks[0] + "\t" + chuncks[1] + "\t" + chuncks[2];
                hashKeyTypePosition = chuncks[0] + "\t" + chuncks[1];
                parttypePositionQtyPrevelence[hashKey] = (float)parttypePositionQtyDict[hashKey] / (float)parttypePositionDict[hashKeyTypePosition];
            }

            cacheFilename = chunk.cachefile + "_qtyOutliers.txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                bool alreadyFlaggedThisApp = false; int typicalQty;
                foreach (App app in chunk.appsList)
                {
                    if (app.action == "D") { continue; } // ignore "Delete" apps
                    alreadyFlaggedThisApp = false;
                    hashKey = app.parttypeid.ToString() + "\t" + app.positionid.ToString() + "\t" + app.quantity.ToString();
                    hashKeyTypePosition = app.parttypeid.ToString() + "\t" + app.positionid.ToString();
                    if (parttypePositionQtyPrevelence[hashKey] < outliernessThreshold && parttypePositionDict[hashKeyTypePosition] >= qtyOutlierSampleSize)
                    {
                        chunk.qtyOutlierCount++; alreadyFlaggedThisApp = true;
                        sw.WriteLine("Quantity of " + app.quantity.ToString() + " is in the minority for this part-type and position\t" + app.id.ToString() + "\t" + app.reference  + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity.ToString() + "\t" + app.part + "\t" + app.niceAttributesString(vcdb, false) + "\t" + app.niceQdbQualifierString(qdb) + "\t" + string.Join(";", app.notes));
                    }

                    //also check "typical" quantities also
                    typicalQty=typicalAppQty(app.parttypeid, app.positionid);
                    if (!alreadyFlaggedThisApp && typicalQty !=0 && app.quantity != typicalQty)
                    {
                        chunk.qtyOutlierCount++;
                        sw.WriteLine("Quantity of " + app.quantity.ToString() + " is not typical for this part-type and position\t" + app.id.ToString() + "\t" + app.reference +  "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity.ToString() + "\t" + app.part + "\t" + app.niceAttributesString(vcdb, false) + "\t" + app.niceQdbQualifierString(qdb) + "\t" + string.Join(";", app.notes));
                    }


                }
            }
            if (chunk.qtyOutlierCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tWarning: " + warningsCount.ToString() + " qty outliers"); } // delete cache file if empty

            // asset/app analysis -- look for orphan assets and apps that have no supporting asset node (even though they claimed an asset)
            logHistoryEvent("", "5\tLooking for asset problems");
            cacheFilename = chunk.cachefile + "_assetProblems.txt";
            using (StreamWriter sw = new StreamWriter(cacheFilename))
            {
                foreach (App app in chunk.appsList)
                {// look for apps that refer to a missing asset name
                    if (app.action == "D") { continue; } // ignore "Delete" apps
                    if (app.asset!="" && !distinctAssetNames.Contains(app.asset))
                    {
                        sw.WriteLine("Asset (" + app.asset + ") is not present in the file\t" + app.id.ToString() + "\t" + app.reference + "\t" + app.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(app.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(app.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(app.basevehicleid) + "\t" + pcdb.niceParttype(app.parttypeid) + "\t" + pcdb.nicePosition(app.positionid) + "\t" + app.quantity.ToString() + "\t" + app.part + "\t" + app.niceFullFitmentString(vcdb, qdb));
                        chunk.assetProblemsCount++;
                    }
                }

                // look for assets that don't have a matching app
                bool foundApp = false;
                foreach(Asset asset in assets)
                {
                    foundApp = false;
                    foreach (App app in apps)
                    {
                        if (app.action == "D") { continue; } // ignore "Delete" apps
                        if (asset.basevehicleid==0 || asset.basevehicleid != app.basevehicleid || asset.VCdbAttributes.Count != app.VCdbAttributes.Count || asset.notes.Count != app.notes.Count || asset.QdbQualifiers.Count != app.QdbQualifiers.Count) { continue; }
                        // found an app with a matching basevid and counts of fitment elements

                        bool foundVCdbAttribute = true;
                        foreach (VCdbAttribute assetAttribute in asset.VCdbAttributes)
                        {// look through all the asset's VCdb addtributes looking for matches in the found app's vcdb attributes
                            foundVCdbAttribute = false;
                            foreach (VCdbAttribute appAttribute in app.VCdbAttributes)
                            {
                                if (appAttribute.name==assetAttribute.name && appAttribute.value == assetAttribute.value){ foundVCdbAttribute = true; break; }
                            }
                            if (!foundVCdbAttribute) { break; }
                        }

                        bool foundNotes = true;
                        foreach(String assetNote in asset.notes)
                        {
                            if (!app.notes.Contains(assetNote)) {foundNotes = false; break;}
                        }

                        bool foundQdbQualifier = true;
                        foreach(QdbQualifier assetQdbQualifier in asset.QdbQualifiers)
                        {
                            foundQdbQualifier = false;
                            foreach(QdbQualifier appQdbQualifier in app.QdbQualifiers)
                            {
                                if(appQdbQualifier.qualifierText == assetQdbQualifier.qualifierText) { foundQdbQualifier = true;break; }
                            }
                            if (!foundQdbQualifier) { break; }
                        }

                        if (foundVCdbAttribute && foundNotes && foundQdbQualifier) { foundApp = true; }
                    }

                    if (!foundApp)
                    {
                        sw.WriteLine("Asset id " + asset.id.ToString() + " (" + asset.assetName + ") has no matching app\t0\t\t" + asset.basevehicleid.ToString() + "\t" + vcdb.niceMakeOfBasevid(asset.basevehicleid) + "\t" + vcdb.niceModelOfBasevid(asset.basevehicleid) + "\t" + vcdb.niceYearOfBasevid(asset.basevehicleid) + "\t\t\t\t\t" + asset.niceFullFitmentString(vcdb, qdb));
                        chunk.assetProblemsCount++;
                    }
                }
            }

            if (chunk.assetProblemsCount == 0) { File.Delete(cacheFilename); } else { logHistoryEvent("", "5\tWarning: " + chunk.assetProblemsCount.ToString() + " asset problems"); } // delete cache file if empty
            chunk.complete = true;
        }
        
        //the world that the children made, here

        // take two ACES objects (primary and reference) and put the diffs into "this" instance
        public void findDifferentials(ACES primearyaces, ACES refaces, VCdb vcdb, PCdb pcdb, Qdb qdb, IProgress<int> progress)
        {
            string hashKey = ""; int percentProgress = 0; int partsAddedCount = 0; int partsDroppedCount = 0; int vehiclesAddedCount = 0; int vehiclesDroppedCount = 0; int appsAddedCount = 0; int appsDroppedCount = 0;
            Dictionary<string, int> primaryvehicles = new Dictionary<string, int>();
            Dictionary<string, int> refvehicles = new Dictionary<string, int>();
            Dictionary<string, int> primaryApps = new Dictionary<string, int>();
            Dictionary<string, int> refApps = new Dictionary<string, int>();


            foreach (KeyValuePair<string,int> partEntry in primearyaces.partsAppCounts)
            {
                if (!refaces.partsAppCounts.ContainsKey(partEntry.Key)) { differentialParts.Add("Add\t" + partEntry.Key); partsAddedCount++; }
            }
            if (progress != null) { percentProgress = 5; progress.Report(percentProgress); }
            foreach (KeyValuePair<string,int> partEntry in refaces.partsAppCounts)
            {
                if (!primearyaces.partsAppCounts.ContainsKey(partEntry.Key)) { differentialParts.Add("Drop\t" + partEntry.Key); partsDroppedCount++; }
            }

            // differential vehicles - defined as basevid/type/position/attributes/notes/mfrlabel
            if (progress != null) { percentProgress = 10; progress.Report(percentProgress); }
            foreach (App app in primearyaces.apps)
            {
                if (app.action == "D") { continue; } // ignore "Delete" apps
                hashKey = app.basevehicleid.ToString() + "\t" + app.parttypeid.ToString() + "\t" + app.positionid.ToString() + "\t" + app.niceFullFitmentString(vcdb,qdb)+ "\t" + app.mfrlabel + "\t" + app.brand + "\t" + app.subbrand;
                if (!primaryvehicles.ContainsKey(hashKey)) { primaryvehicles.Add(hashKey, 0); }
            }
            if (progress != null) { percentProgress = 15; progress.Report(percentProgress); }
            foreach (App app in refaces.apps)
            {
                if (app.action == "D") { continue; } // ignore "Delete" apps
                hashKey = app.basevehicleid.ToString() + "\t" + app.parttypeid.ToString() + "\t" + app.positionid.ToString() + "\t" + app.niceFullFitmentString(vcdb, qdb) + "\t" + app.mfrlabel + "\t" + app.brand + "\t" + app.subbrand;
                if (!refvehicles.ContainsKey(hashKey)) { refvehicles.Add(hashKey, 0); }
            }
            if (progress != null) { percentProgress = 20; progress.Report(percentProgress); }
            foreach (KeyValuePair<string, int> entry in primaryvehicles)
            {
                if (!refvehicles.ContainsKey(entry.Key)) { differentialVehicles.Add("Add\t" + entry.Key); vehiclesAddedCount++; }
            }
            if (progress != null) { percentProgress = 25; progress.Report(percentProgress); }
            foreach (KeyValuePair<string, int> entry in refvehicles)
            {
                if (!primaryvehicles.ContainsKey(entry.Key)) { differentialVehicles.Add("Drop\t" + entry.Key); vehiclesDroppedCount++; }
            }



            // differential apps  - defined as basevid/type/position/attributes/qdbs/notes/mfrlabel/asset/part
            if (progress != null) { percentProgress = 30; progress.Report(percentProgress); }
            foreach (App app in primearyaces.apps)
            {
                if (app.action == "D") { continue; } // ignore "Delete" apps
                hashKey = app.basevehicleid.ToString() + "\t" + app.parttypeid.ToString() + "\t" + app.positionid.ToString() + "\t" + app.quantity.ToString() + "\t" + app.niceFullFitmentString(vcdb, qdb) + "\t" + app.mfrlabel + "\t" + app.part + "\t" + app.asset + "\t" + app.assetitemorder.ToString() + "\t" + app.brand + "\t" + app.subbrand;
                if (!primaryApps.ContainsKey(hashKey)) { primaryApps.Add(hashKey, 0); }
            }
            if (progress != null) { percentProgress = 40; progress.Report(percentProgress); }
            foreach (App app in refaces.apps)
            {
                if (app.action == "D") { continue; } // ignore "Delete" apps
                hashKey = app.basevehicleid.ToString() + "\t" + app.parttypeid.ToString() + "\t" + app.positionid.ToString() + "\t" + app.quantity.ToString() + "\t" + app.niceFullFitmentString(vcdb, qdb) + "\t" + app.mfrlabel + "\t" + app.part + "\t" + app.asset + "\t" + app.assetitemorder.ToString() + "\t" + app.brand + "\t" + app.subbrand;
                if (!refApps.ContainsKey(hashKey)) { refApps.Add(hashKey, 0); }
            }
            if (progress != null) { percentProgress = 70; progress.Report(percentProgress); }


            foreach (App app in primearyaces.apps)
            {
                if (app.action == "D") { continue; } // ignore "Delete" apps
                hashKey = app.basevehicleid.ToString() + "\t" + app.parttypeid.ToString() + "\t" + app.positionid.ToString() + "\t" + app.quantity.ToString() + "\t" + app.niceFullFitmentString(vcdb, qdb) + "\t" + app.mfrlabel + "\t" + app.part + "\t" + app.asset + "\t" + app.assetitemorder.ToString() + "\t" + app.brand + "\t" + app.subbrand;
                if (!refApps.ContainsKey(hashKey))
                {
                    App addApp = new App();
                    addApp.basevehicleid = app.basevehicleid;
                    addApp.action = "A";
                    addApp.reference = app.reference;
                    addApp.asset = app.asset;
                    addApp.assetitemorder = app.assetitemorder;
                    addApp.assetitemref = app.assetitemref;
                    addApp.id = app.id;
                    addApp.mfrlabel = app.mfrlabel;
                    addApp.notes = app.notes;
                    addApp.part = app.part;
                    addApp.parttypeid = app.parttypeid;
                    addApp.positionid = app.positionid;
                    addApp.QdbQualifiers = app.QdbQualifiers;
                    addApp.quantity = app.quantity;
                    addApp.VCdbAttributes = app.VCdbAttributes;
                    addApp.brand = app.brand;
                    addApp.subbrand = app.subbrand;

                    this.apps.Add(addApp);
                }
            }

            foreach (App app in refaces.apps)
            {
                if (app.action == "D") { continue; } // ignore "Delete" apps
                hashKey = app.basevehicleid.ToString() + "\t" + app.parttypeid.ToString() + "\t" + app.positionid.ToString() + "\t" + app.quantity.ToString() + "\t" + app.niceFullFitmentString(vcdb, qdb) + "\t" + app.mfrlabel + "\t" + app.part + "\t" + app.asset + "\t" + app.assetitemorder.ToString() + "\t" + app.brand + "\t" + app.subbrand; 
                if (!primaryApps.ContainsKey(hashKey))
                {
                    App dropApp = new App();
                    dropApp.basevehicleid = app.basevehicleid;
                    dropApp.action = "D";
                    dropApp.reference = app.reference;
                    dropApp.asset = app.asset;
                    dropApp.assetitemorder = app.assetitemorder;
                    dropApp.assetitemref = app.assetitemref;
                    dropApp.id = app.id;
                    dropApp.mfrlabel = app.mfrlabel;
                    dropApp.notes = app.notes;
                    dropApp.part = app.part;
                    dropApp.parttypeid = app.parttypeid;
                    dropApp.positionid = app.positionid;
                    dropApp.QdbQualifiers = app.QdbQualifiers;
                    dropApp.quantity = app.quantity;
                    dropApp.VCdbAttributes = app.VCdbAttributes;
                    dropApp.brand = app.brand;
                    dropApp.subbrand = app.subbrand;

                    this.apps.Add(dropApp);
                }
            }

            if (progress != null) { percentProgress = 100; progress.Report(percentProgress); }

            differentialsSummary = "Parts: +" + partsAddedCount.ToString() + ", -" + partsDroppedCount.ToString() + "   Vehicles: +" + vehiclesAddedCount.ToString() + ", -" + vehiclesDroppedCount.ToString();
        }

        public int typicalAppQty(int parttypeid, int positionid)
        {
            string key = parttypeid.ToString() + "_" + positionid.ToString();
            switch(key)
            {

                //pad sets
                case "1684_22": return 1;
                case "1684_103": return 1;
                case "1684_104": return 1;
                case "1684_30": return 1;
                case "1684_105": return 1;
                case "1684_106": return 1;

                //rotors
                case "1896_22": return 2;
                case "1896_103": return 1;
                case "1896_104": return 1;
                case "1896_30": return 2;
                case "1896_105": return 1;
                case "1896_106": return 1;

                //electronic wear sensors
//                case "1920_22": return 2;
                case "1920_103": return 1;
                case "1920_104": return 1;
//                case "1920_30": return 2;
                case "1920_105": return 1;
                case "1920_106": return 1;

                //brake drums
                case "1744_22": return 2;
                case "1744_103": return 1;
                case "1744_104": return 1;
                case "1744_30": return 2;
                case "1744_105": return 1;
                case "1744_106": return 1;

/*
2 | Left |
12 | Right |
22 | Front |
30 | Rear |
87 | Front Left Upper    |
88 | Front Left Lower    |
89 | Front Right Upper   |
90 | Front Right Lower   |
91 | Rear Left Upper     |
92 | Rear Left Lower     |
93 | Rear Right Lower    |
94 | Rear Right Upper    |
103 | Front Left |
104 | Front Right |
105 | Rear Left |
106 | Rear Right |
3974 | Front Right Forward |
4136 | Rear Left Forward   |
4137 | Rear Left Rearward  |
4138 | Rear Right Forward  |
4139 | Rear Right Rearward |
*/




default: return 0;
            }
        }



        public int importInterchange(string interchangeFile)
        {
            using (var reader = new StreamReader(interchangeFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fields = line.Split('\t');
                    if (fields.Count() == 2 && fields[0].Trim().Length > 0 && fields[1].Trim().Length > 0)
                    {//text must be exactly 2 columns seperated by tab
                        interchange[fields[0].Trim()] = fields[1].Trim();
                    }
                }
            }
            return interchange.Count;
        }

        public int importAssetNameInterchange(string interchangeFile)
        {
            using (var reader = new StreamReader(interchangeFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fields = line.Split('\t');
                    if (fields.Count() == 2 && fields[0].Trim().Length > 0 && fields[1].Trim().Length > 0)
                    {//text must be exactly 2 columns seperated by tab
                        assetNameInterchange[fields[0].Trim()] = fields[1].Trim();
                    }
                }
            }
            return assetNameInterchange.Count;
        }

        //xxx

        public void importFitmentPermutationMiningCache(string cacheFile)
        {
            fitmentPermutationMiningCache.Clear();

            if (!File.Exists(cacheFile))
            {// need to create cachefile
                try
                { // write empty file and return
                    using (StreamWriter sw = new StreamWriter(cacheFile)){}
                }
                catch (Exception ex){}
                return;
            }
            
            using (var reader = new StreamReader(cacheFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fields = line.Split('\t');
                    if (fields.Count() == 2 && fields[0].Trim().Length > 0 && fields[1].Trim().Length > 0)
                    {
                        if (!fitmentPermutationMiningCache.ContainsKey(fields[0]))
                        {
                            fitmentPermutationMiningCache.Add(fields[0], fields[1]);
                        }
                    }
                }
            }
        }

        public void writeFitmentPermutationMiningCache(string cacheFile)
        {
            if (fitmentPermutationMiningCache.Count() > 0)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(cacheFile))
                    {
                        foreach (KeyValuePair<string, string> cacheEntry in fitmentPermutationMiningCache)
                        {
                            sw.WriteLine(cacheEntry.Key + "\t" + cacheEntry.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        static IEnumerable<XElement> StreamAppElement(Stream inputSteam,string name)
        {
            using (XmlReader reader = XmlReader.Create(inputSteam))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == name)
                            {
                                XElement el = XElement.ReadFrom(reader) as XElement;
                                if (el != null)
                                    yield return el;
                            }
                            break;
                    }
                }
            }
        }


        public List<string> listValidACESfiles(string path,List<string> excludedFiles)
        {
            List<string> returnVal = new List<string>();

            try
            {// check directory for xml files and read their first few lines to see if it smells like and ACES file

                foreach (string filename in Directory.GetFiles(path, "*.xml"))
                {
                    if (excludedFiles.Contains(filename)) { continue; } // ignore files already processed

                    string filesample = "";

                    byte[] test = new byte[5000];
                    using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
                    {
                        reader.Read(test, 0, 5000).ToString();
                        filesample = System.Text.Encoding.UTF8.GetString(test);
                        if (filesample.Contains("<ACES") && filesample.Contains("<Header") && filesample.Contains("<App"))
                        {
                            returnVal.Add(filename);
                            logHistoryEvent("", "10\tAutomation - listValidACESfiles() found file: "+ filename);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logHistoryEvent("", "0\tAutomation - Exception during listValidACESfiles:"+ex.Message);

            }





            return returnVal;
        
        }




        public int importXML(string _filePath, string _schemaString, bool respectValidateNoTag, bool importDeletes, Dictionary<string,string> noteTranslation, Dictionary<string,QdbQualifier> noteQdbTransformDictionary, VCdb vcdb, IProgress<int> progress)
        {
            // if schema string is "", select XSD according to what ACES version is claimed by the XML

            filePath = _filePath;
            xmlValidationErrors.Clear();
            successfulImport = false; string schemaString = ""; //bool found; //int i;
            XDocument xmlDoc = null;
            XmlSchemaSet schemas = new XmlSchemaSet();
            string[] VCdbAttributeNames = new string[] { "Aspiration", "BedLength", "BedType", "BodyNumDoors", "BodyType", "BrakeABS", "BrakeSystem", "CylinderHeadType", "DriveType", "EngineBase", "EngineDesignation", "EngineMfr", "EngineVIN", "EngineVersion", "EngineBlock","FrontBrakeType", "FrontSpringType", "FuelDeliverySubType", "FuelDeliveryType", "FuelSystemControlType", "FuelSystemDesign", "FuelType", "IgnitionSystemType", "MfrBodyCode", "PowerOutput", "RearBrakeType", "RearSpringType", "Region", "SteeringSystem", "SteeringType", "SubModel", "TransElecControlled", "TransmissionBase", "TransmissionControlType", "TransmissionMfr", "TransmissionMfrCode", "TransmissionNumSpeeds", "TransmissionType", "ValvesPerEngine", "VehicleType", "WheelBase" };
            string noteTemp;
            bool splitNotesBySemicolon = true;
            List<int> basevidsInRange = new List<int>();
            int NoteUsageCount = 0;
            int QdbUsageCount = 0;

            if (_schemaString == "")
            {// no schema string was passed in - extract ACES version from XML


                using (Stream s = File.OpenRead(filePath))
                {

                    try
                    {
                        //xmlDoc = XDocument.Load(filePath);
                        xmlDoc = XDocument.Load(s);
                        version = (string)xmlDoc.Element("ACES").Attribute("version");
                        if (ACESschemas.TryGetValue(version, out schemaString))
                        {// ACES version claimed by XML file was found in the dictionary of schemas
                            logHistoryEvent("", "1\tKnown schema version:" + version);
                            schemas.Add("", XmlReader.Create(new StringReader(schemaString)));
                        }
                        else
                        {// ACES version claimed by XML file was NOT found in the dictionary of schemas
                            xmlValidationErrors.Add("Your XML file contains an unsupported version (" + version + ") of ACES");
                            logHistoryEvent("", "0\tunsupported version (" + version + ")");
                            return 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        xmlValidationErrors.Add(ex.Message);
                        logHistoryEvent("", "0\t" + ex.Message);
                        return 0;
                    }
                }


            }
            else
            {
                schemas.Add("", XmlReader.Create(new StringReader(_schemaString)));
            }


            FileStream xmlFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);



            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += (o, args) => {xmlValidationErrors.Add(args.Message);};
                settings.Schemas.Add(schemas);

                //XmlReader reader = XmlReader.Create(new StreamReader(filePath), settings);
                XmlReader reader = XmlReader.Create(xmlFileStream, settings);


                while (reader.Read()) ;
            }
            catch (Exception ex){xmlValidationErrors.Add(ex.Message);}


            //foreach(XElement FooterElement in StreamAppElement(filePath, "Footer"))
            xmlFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            foreach (XElement FooterElement in StreamAppElement(xmlFileStream, "Footer"))
            {
                FooterRecordCount = Convert.ToInt32((string)FooterElement.Element("RecordCount"));
            }



            //foreach (XElement HeaderElement in StreamAppElement(filePath, "Header"))
            xmlFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            foreach (XElement HeaderElement in StreamAppElement(xmlFileStream, "Header"))
            {
                Company = (string)HeaderElement.Element("Company");
                SenderName = (string)HeaderElement.Element("SenderName");
                SenderPhone = (string)HeaderElement.Element("SenderPhone");
                TransferDate = (string)HeaderElement.Element("TransferDate");
                EffectiveDate = (string)HeaderElement.Element("EffectiveDate");
                BrandAAIAID = (string)HeaderElement.Element("BrandAAIAID");
                SubmissionType = (string)HeaderElement.Element("SubmissionType");
                VcdbVersionDate = (string)HeaderElement.Element("VcdbVersionDate");
                PcdbVersionDate = (string)HeaderElement.Element("PcdbVersionDate");
                QdbVersionDate = (string)HeaderElement.Element("QdbVersionDate");
                DocumentTitle = (string)HeaderElement.Element("DocumentTitle");
            }

            //-- Stand-alone Assets (not the ones in the App) --------------------------------------------------------------------------------------------
         //            foreach (XElement assetElement in StreamAppElement(filePath, "Asset"))
            xmlFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            foreach (XElement assetElement in StreamAppElement(xmlFileStream, "Asset"))
           {
                Asset assetTemp = new Asset();
                assetTemp.action = (string)assetElement.Attribute("action").Value;
                if (assetTemp.action == "D" && !importDeletes) { discardedDeletsOnImport++; continue; }// skip deleted assets
                assetTemp.assetName = (string)assetElement.Element("AssetName");
                assetTemp.id = Convert.ToInt32(assetElement.Attribute("id").Value);
                if (!distinctAssetNames.Contains(assetTemp.assetName)) { distinctAssetNames.Add(assetTemp.assetName); }
                xmlAssetNodeCount++;

                basevidsInRange.Clear();
                if (assetElement.Element("BaseVehicle") != null)
                {// standard base-vid style app
                    basevidsInRange.Add(Convert.ToInt32(assetElement.Element("BaseVehicle").Attribute("id").Value));
                }
                else
                {// this is a year-range style app (not a specific basevid)
                    if (assetElement.Element("Years") != null)
                    {// year-range style app
                        if ((string)assetElement.Element("Make").Attribute("id").Value != null && (string)assetElement.Element("Model").Attribute("id").Value != null && (string)assetElement.Element("Years").Attribute("from").Value != null && (string)assetElement.Element("Years").Attribute("to").Value != null)
                        {
                            basevidsInRange = vcdb.basevidsFromYearRange(Convert.ToInt32((string)assetElement.Element("Make").Attribute("id").Value), Convert.ToInt32((string)assetElement.Element("Model").Attribute("id").Value), Convert.ToInt32((string)assetElement.Element("Years").Attribute("from").Value), Convert.ToInt32((string)assetElement.Element("Years").Attribute("to").Value));
                        }
                    }
                    else
                    {// equipment-style app (ACES 4.0+)
                    }
                }

                foreach (XElement noteElement in assetElement.Descendants("Note"))
                {
                    noteTemp = (string)noteElement;
                    if (noteTemp.Contains(";") && splitNotesBySemicolon)
                    {
                        foreach (string noteTempChunk in noteTemp.Split(';'))
                        {
                            if (!assetTemp.notes.Contains(noteTempChunk.Trim()))
                            {
                                assetTemp.notes.Add(noteTempChunk.Trim());
                            }
                        }
                    }
                    else
                    {
                        if (!assetTemp.notes.Contains(noteTemp.Trim()))
                        {
                            assetTemp.notes.Add(noteTemp.Trim());
                        }
                    }
                    
                    string translationNoteOutput = "";
                    List<String> noteListTemp = new List<string>();
                    bool changedSomeNotes = false;
                    foreach (string translationNoteInput in assetTemp.notes)
                    {
                        if (noteTranslation.ContainsKey(translationNoteInput))
                        {// this note is called-out in the left column of our translation file
                            translationNoteOutput = noteTranslation[translationNoteInput];
                            // see if the output is a Qdb transform
                            if (translationNoteOutput.Substring(0, 4) == "Qdb:")
                            {// transform this note to a Qdb.

                            }
                            else
                            {// plain-old translate of note - take second column as the resulting new note text
                                changedSomeNotes = true;
                                if (translationNoteOutput != "")
                                {// right column of translation file is non-blank. use the translated value
                                    noteListTemp.Add(translationNoteOutput);
                                }
                            }
                        }
                        else
                        {// this note is not present in the translations list - add it to the temp output list
                            noteListTemp.Add(translationNoteInput);
                        }
                    }
                    if (changedSomeNotes) { assetTemp.notes = noteListTemp; }
                }

                foreach (string VCdbAttributeName in VCdbAttributeNames)
                {// roll through the entire of list of possible VCdb attribute names looking for nodes like <SubModel id="13">
                    if (assetElement.Element(VCdbAttributeName) != null)
                    {
                        VCdbAttribute VCdbAttributeTemp = new VCdbAttribute();
                        VCdbAttributeTemp.name = VCdbAttributeName;
                        VCdbAttributeTemp.value = Convert.ToInt32(assetElement.Element(VCdbAttributeName).Attribute("id").Value);
                        assetTemp.VCdbAttributes.Add(VCdbAttributeTemp);
                    }
                }

                assetTemp.VCdbAttributes.Sort();

                // Qdb for this asset
                foreach (XElement qualElement in assetElement.Elements("Qual"))
                {
                    QdbQualifier QdbQualifierTemp = new QdbQualifier();
                    List<string> myParametersList = new List<string>();
                    QdbQualifierTemp.qualifierId = Convert.ToInt32(qualElement.Attribute("id").Value);
                    foreach (XElement paramElement in qualElement.Elements("param"))
                    {
                        if (paramElement.Attribute("uom") != null)
                        {// no unit-of-measure attribute present
                            myParametersList.Add((string)paramElement.Attribute("value").Value + " " + (string)paramElement.Attribute("uom").Value);
                        }
                        else
                        {
                            myParametersList.Add((string)paramElement.Attribute("value").Value);
                        }
                    }
                    QdbQualifierTemp.qualifierParameters = myParametersList;
                    assetTemp.QdbQualifiers.Add(QdbQualifierTemp);
                }

                // see if any of the note strings in the app have Qdb transforms defined
                List<String> newNoteListTemp = new List<string>();
                foreach (string QdbTransformInput in assetTemp.notes)
                {
                    if (noteQdbTransformDictionary.ContainsKey(QdbTransformInput))
                    {//there exists a note->QdbTransform for this note text. Add the QdbQualifier (which includes its list of parms) to this app
                        assetTemp.QdbQualifiers.Add(noteQdbTransformDictionary[QdbTransformInput]);
                    }
                    else
                    {// no transform exists this note will get add to the app as-is
                        newNoteListTemp.Add(QdbTransformInput);
                    }
                }
                assetTemp.notes = newNoteListTemp;


                foreach (int basevid in basevidsInRange)
                {
                    Asset assetToAdd = new Asset();
                    assetToAdd.basevehicleid = basevid;
                    assetToAdd.action = assetTemp.action;
                    assetToAdd.assetName = assetTemp.assetName;
                    assetToAdd.id = assetTemp.id;
                    assetToAdd.notes = assetTemp.notes;
                    assetToAdd.QdbQualifiers = assetTemp.QdbQualifiers;
                    assetToAdd.VCdbAttributes = assetTemp.VCdbAttributes;
                    assets.Add(assetToAdd);
                    NoteUsageCount += assetToAdd.notes.Count;
                    QdbUsageCount += assetToAdd.QdbQualifiers.Count;
                }

            }


 //--End of Stand-alone assets -------------------------------------------------------------------------


            int percentProgress = 0;

            //          foreach (XElement appElement in StreamAppElement(filePath, "App"))
            xmlFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            foreach (XElement appElement in StreamAppElement(xmlFileStream, "App"))
            {
                App appTemp = new App();
                appTemp.action = (string)appElement.Attribute("action").Value;
                if (appTemp.action == "D" && !importDeletes) { discardedDeletsOnImport++; continue;}

                appTemp.validate = true; if (respectValidateNoTag && (string)appElement.Attribute("action").Value == "no") { appTemp.validate = false; }

                if (appElement.Attribute("ref") != null)
                {
                    appTemp.reference = (string)appElement.Attribute("ref").Value;
                }

                appTemp.part = (string)appElement.Element("Part");
                if (ignoreNAitems && (appTemp.part == "NA" || appTemp.part == "NR" || appTemp.part == "N/A" || appTemp.part == "N/R" || appTemp.part == "")) { continue; } // skip place-holder part numbers like "NA"

                appTemp.brand = ""; appTemp.subbrand = "";
                XElement partElement = appElement.Element("Part");
                if (partElement.Attribute("BrandAAIAID") != null){appTemp.brand = (string)partElement.Attribute("BrandAAIAID");}
                if (partElement.Attribute("SubBrandAAIAID") != null) { appTemp.subbrand = (string)partElement.Attribute("SubBrandAAIAID"); }

                appTemp.id = Convert.ToInt32(appElement.Attribute("id").Value);
                xmlAppNodeCount++;

                basevidsInRange.Clear();

                if (appElement.Element("BaseVehicle") != null)
                {// standard, traditional base-vid style app (majority of the files in the wild)
                    basevidsInRange.Add(Convert.ToInt32(appElement.Element("BaseVehicle").Attribute("id").Value));
                }
                else
                {// this is a year-range style app (not a specific basevid)
                    if (appElement.Element("Years") != null)
                    {// year-range style app. It may contain only makeid, but could also include modelid

                        int MakeIDtemp = 0; if (appElement.Element("Make") != null) { MakeIDtemp = Convert.ToInt32((string)appElement.Element("Make").Attribute("id").Value); }
                        int ModelIDtemp = 0; if (appElement.Element("Model") != null) { MakeIDtemp = Convert.ToInt32((string)appElement.Element("Model").Attribute("id").Value); }
                        if ((string)appElement.Element("Years").Attribute("from").Value != null && (string)appElement.Element("Years").Attribute("to").Value != null)
                        {
                            basevidsInRange = vcdb.basevidsFromYearRange(MakeIDtemp, ModelIDtemp, Convert.ToInt32((string)appElement.Element("Years").Attribute("from").Value), Convert.ToInt32((string)appElement.Element("Years").Attribute("to").Value));
                        }
                    }
                    else
                    {// equipment-style app (ACES 4.0+)
                    }
                }

                appTemp.positionid = 0; if ((string)appElement.Element("Position") != null && (string)appElement.Element("Position").Attribute("id").Value!="") { appTemp.positionid = Convert.ToInt32(appElement.Element("Position").Attribute("id").Value); }
                appTemp.parttypeid = 0; if ((string)appElement.Element("PartType") != null && (string)appElement.Element("PartType").Attribute("id").Value != "") { appTemp.parttypeid = Convert.ToInt32(appElement.Element("PartType").Attribute("id").Value); }
                appTemp.mfrlabel = (string)appElement.Element("MfrLabel");
                appTemp.quantity = Convert.ToInt32((string)appElement.Element("Qty"));

                if(interchange.Count()>0)
                {// translate part by interchange (if any anterchange recs exist)
                    if (interchange.ContainsKey(appTemp.part))
                    {// found part from xml input in the interchange. Replace with "output" part
                        appTemp.part = interchange[appTemp.part];
                    }
                    else
                    {// interchange does not contain this part in the "input" column - ignore app (skip its importation)
                        continue;
                    }
                }


                if (appTemp.mfrlabel != null && !distinctMfrLabels.Contains(appTemp.mfrlabel)) { distinctMfrLabels.Add(appTemp.mfrlabel); }

                if (!distinctPartTypes.Contains(appTemp.parttypeid)) { distinctPartTypes.Add(appTemp.parttypeid); }


                
                if (!partsAppCounts.ContainsKey(appTemp.part))
                {
                    partsAppCounts.Add(appTemp.part,1);
                }
                else
                {// this part has been seen before - increment the associated count
                    partsAppCounts[appTemp.part]++;
                }
                


                if(partsPartTypes.ContainsKey(appTemp.part))
                {// this part has been seen before - if we'v never seen it type id'd this way, add this new type id
                    if(!partsPartTypes[appTemp.part].Contains(appTemp.parttypeid)){partsPartTypes[appTemp.part].Add(appTemp.parttypeid);}
                }
                else
                {//add new part key and list (with single element)
                    List<int> parttypeIdListTemp = new List<int>();
                    parttypeIdListTemp.Add(appTemp.parttypeid);
                    partsPartTypes.Add(appTemp.part, parttypeIdListTemp);
                }

                if (partsPositions.ContainsKey(appTemp.part))
                {// this part has been seen before - if we'v never seen it position id'd this way, add this new position id
                    if (!partsPositions[appTemp.part].Contains(appTemp.positionid)){partsPositions[appTemp.part].Add(appTemp.positionid);}
                }
                else
                {//add new part key and positionid list (with single element)
                    List<int> positionIdListTemp = new List<int>();
                    positionIdListTemp.Add(appTemp.positionid);
                    partsPositions.Add(appTemp.part, positionIdListTemp);
                }


                string assetNameTemp = (string)appElement.Element("AssetName");
                appTemp.asset = ""; appTemp.assetitemref = ""; appTemp.assetitemorder = 0;

                if(assetNameTemp != null)
                {
                    if (assetNameInterchange.ContainsKey(assetNameTemp))
                    {//  AssetName translation record exists , and it is not "" - translated it 
                        if (assetNameInterchange[assetNameTemp] != "")
                        {
                            appTemp.asset = assetNameInterchange[assetNameTemp];
                            if ((string)appElement.Element("AssetItemRef") != null) { appTemp.assetitemref = (string)appElement.Element("AssetItemRef"); }
                            if ((string)appElement.Element("AssetItemOrder") != null) { appTemp.assetitemorder = Convert.ToInt32((string)appElement.Element("AssetItemOrder")); }
                        }
                    }
                    else
                    {//no AssetName translation record exists - pull it in as-is
                        appTemp.asset = assetNameTemp;
                        if ((string)appElement.Element("AssetItemRef") != null) { appTemp.assetitemref = (string)appElement.Element("AssetItemRef"); }
                        if ((string)appElement.Element("AssetItemOrder") != null)
                        {
                            appTemp.assetitemorder = Convert.ToInt32((string)appElement.Element("AssetItemOrder"));
                        }
                    }
                    if (!distinctAssets.Contains(appTemp.asset) && appTemp.asset!="") { distinctAssets.Add(appTemp.asset); }
                }

                foreach (XElement noteElement in appElement.Descendants("Note"))
                {
                    noteTemp = (string)noteElement;
                    if(noteTemp.Contains(";") && splitNotesBySemicolon)
                    {
                        foreach (string noteTempChunk in noteTemp.Split(';'))
                        {
                            if (!appTemp.notes.Contains(noteTempChunk.Trim()) && noteTempChunk.Trim() != "")
                            {
                                appTemp.notes.Add(noteTempChunk.Trim());
                            }
                        }
                    }
                    else
                    {
                        if (!appTemp.notes.Contains(noteTemp.Trim()) && noteTemp.Trim()!="")
                        {
                            appTemp.notes.Add(noteTemp.Trim());
                        }
                    }
                }

                //add this app's notes to the master noteCounts dictionary
                foreach (string appTempNote in appTemp.notes)
                {
                    if (noteCounts.ContainsKey(appTempNote)){noteCounts[appTempNote]++;}else{noteCounts.Add(appTempNote, 1);}
                }





                string translationNoteOutput = "";
                List<String> noteListTemp = new List<string>();
                bool changedSomeNotes = false;
                foreach(string translationNoteInput in appTemp.notes)
                {
                    if (noteTranslation.ContainsKey(translationNoteInput))
                    {// this note is called-out in the left column of our translation file
                        translationNoteOutput = noteTranslation[translationNoteInput];
                        changedSomeNotes = true;
                        if (translationNoteOutput != "")
                        {// right column of translation file is non-blank. use the translated value
                            noteListTemp.Add(translationNoteOutput);
                        }
                    }
                    else
                    {// this note is not present in the translations list - add it to the temp output list
                        noteListTemp.Add(translationNoteInput);
                    }
                }
                if (changedSomeNotes) { appTemp.notes = noteListTemp; }


                foreach (string VCdbAttributeName in VCdbAttributeNames)
                {// roll through the entire of list of possible VCdb attribute names looking for nodes like <SubModel id="13">
                    if (appElement.Element(VCdbAttributeName) != null)
                    {
                        VCdbAttribute VCdbAttributeTemp = new VCdbAttribute();
                        VCdbAttributeTemp.name = VCdbAttributeName;
                        VCdbAttributeTemp.value = Convert.ToInt32(appElement.Element(VCdbAttributeName).Attribute("id").Value);
                        appTemp.VCdbAttributes.Add(VCdbAttributeTemp);
                        if (vcdbUsageStatsDict.ContainsKey(VCdbAttributeTemp.name)) { vcdbUsageStatsDict[VCdbAttributeTemp.name]++; } else { vcdbUsageStatsDict.Add(VCdbAttributeTemp.name, 1); } //record usage stats for VCdb tags for later export. This dict is not cleared upon new ACES file load so that all files analyzed within a session are totaled
                    }
                }

                appTemp.VCdbAttributes.Sort();

                // Qdb
                foreach (XElement qualElement in appElement.Elements("Qual"))
                {
                    QdbQualifier QdbQualifierTemp = new QdbQualifier();
                    List<string> myParametersList = new List<string>();
                    QdbQualifierTemp.qualifierId = Convert.ToInt32(qualElement.Attribute("id").Value);
                    foreach (XElement paramElement in qualElement.Elements("param"))
                    {
                        if (paramElement.Attribute("uom")!=null)
                        {// no unit-of-measure attribute present
                            myParametersList.Add((string)paramElement.Attribute("value").Value + " " + (string)paramElement.Attribute("uom").Value);
                        }
                        else
                        {
                            myParametersList.Add((string)paramElement.Attribute("value").Value);
                        }
                    }
                    QdbQualifierTemp.qualifierParameters = myParametersList;

                    bool foundExistingQdbQualifier = false;
                    foreach (QdbQualifier existinQdbQualifier in appTemp.QdbQualifiers)
                    {// skip the import of multiple qualifiers with the same ID
                        if (QdbQualifierTemp.qualifierId == existinQdbQualifier.qualifierId) { 
                           foundExistingQdbQualifier = true; break; 
                        }
                    }

                    if (!foundExistingQdbQualifier)
                    {
                        appTemp.QdbQualifiers.Add(QdbQualifierTemp);
                    }
                    else
                    { // this Qdb id is already used in this app - technically ok, but poor form
                      // maybe convert it to a note?
                        appTemp.QdbQualifiers.Add(QdbQualifierTemp);
                    }



                    // update stats on qdb usage
                    if (!qdbidOccurrences.ContainsKey(QdbQualifierTemp.qualifierId))
                    {// first time seeing this qdbid
                        qdbidOccurrences.Add(QdbQualifierTemp.qualifierId, 1);
                    }
                    else
                    {// seen this qdbid before
                        qdbidOccurrences[QdbQualifierTemp.qualifierId]++;
                    }
                }

                // see if any of the note strings in the app have Qdb transforms defined
                List<String> newNoteListTemp = new List<string>();
                foreach (string QdbTransformInput in appTemp.notes)
                {
                    if (noteQdbTransformDictionary.ContainsKey(QdbTransformInput))
                    {//there exists a note->QdbTransform for this note text. Add the QdbQualifier (which includes its list of parms) to this app
                        appTemp.QdbQualifiers.Add(noteQdbTransformDictionary[QdbTransformInput]);
                    }
                    else
                    {// no transform exists this note will get add to the app as-is
                        newNoteListTemp.Add(QdbTransformInput);
                    }
                }
                appTemp.notes = newNoteListTemp;








                foreach (int basevid in basevidsInRange)
                {

                    if (!basevidOccurrences.ContainsKey(basevid))
                    {// first time seeing this basevid
                        basevidOccurrences.Add(basevid, 1);
                    }
                    else
                    {// seen this basevid before
                        basevidOccurrences[basevid]++;
                    }

                    App appToAdd = new App();
                    appToAdd.basevehicleid = basevid;
                    appToAdd.action = appTemp.action;
                    appToAdd.reference = appTemp.reference;
                    appToAdd.asset = appTemp.asset;
                    appToAdd.assetitemorder = appTemp.assetitemorder;
                    appToAdd.assetitemref = appTemp.assetitemref;
                    appToAdd.id = appTemp.id;
                    appToAdd.mfrlabel = appTemp.mfrlabel;
                    appToAdd.notes = appTemp.notes;
                    appToAdd.part = appTemp.part;
                    appToAdd.parttypeid = appTemp.parttypeid;
                    appToAdd.positionid = appTemp.positionid;
                    appToAdd.QdbQualifiers = appTemp.QdbQualifiers;
                    appToAdd.quantity = appTemp.quantity;
                    appToAdd.VCdbAttributes = appTemp.VCdbAttributes;
                    appToAdd.brand = appTemp.brand;
                    appToAdd.subbrand = appTemp.subbrand;
                    appToAdd.hash = appToAdd.appHash();
                    apps.Add(appToAdd);
                    vcdbUsageStatsTotalApps++;
                    NoteUsageCount += appTemp.notes.Count;
                    QdbUsageCount += appTemp.QdbQualifiers.Count;
                }

                if (progress != null)
                {// only report progress on whole percentage steps (100 total reports). reporting on every iteration is too process intensive
                    percentProgress = Convert.ToInt32(((double)apps.Count / (double)FooterRecordCount) * 100);
                    if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); }
                }
            }

            QdbUtilizationScore = 0;
            if ((NoteUsageCount + QdbUsageCount) > 0)
            {// only compute a score if divide-by-zero will not happen
                QdbUtilizationScore = Convert.ToDecimal(QdbUsageCount * 100) / Convert.ToDecimal(NoteUsageCount + QdbUsageCount);
            }

            logHistoryEvent("", "0\tImported "+apps.Count().ToString() + " apps");
            if(xmlAssetNodeCount>0) {logHistoryEvent("", "0\tImported " + assets.Count().ToString() + " aassets"); }
            if (xmlValidationErrors.Count==0 && apps.Count > 0)
            {
                vcdbUsageStatsFileList.Add(_filePath);
                successfulImport = true;
            }
            else
            {
                logHistoryEvent("", "0\tImport failed (sortof). XSD errors "+ xmlValidationErrors.Count.ToString());
                logHistoryEvent("", "0\tXSD errors: " + string.Join("; ", xmlValidationErrors));
            }
            return apps.Count;
        }

        public QdbQualifier QdbQualifierFromTransfromString(string input)
        {
            // transform this note to a Qdb. string will look like one of these:
            // Qdb:20420:,                  -- no parms
            // Qdb:21720:Sport,             -- "name" parm
            // Qdb:7095:8500,lb             -- "weight" numeric parm with unit of measure
            // Qdb:977:9000,lb;9200,lb      -- multiple "weight" numeric parms with units of measure. semi-colon between parms
            // Qdb:21092:S482213|Y422059,    -- "id list" strings (pipe-delimited)
            // Qdb:21092:S482213|Y422059,;8500,lb;sport,;    -- "id list" strings (pipe-delimited)
            QdbQualifier QdbQualifier = new QdbQualifier();
            QdbQualifier.qualifierParameters = new List<string>();

            if (input.Substring(0,4)=="Qdb:")
            {// Qdb:21092:S482213|Y422059,;8500,lb;sport,;
                input = input.Substring(4); // hack off the fisrt 4 chars ("Qdb:")
            }
            else
            {// no "Qdb:" was not the start of the string - return an un-populated Qdb object
                return QdbQualifier;
            }

            if (input.IndexOf(":")>0)
            {// 21092:S482213|Y422059,;8500,lb;sport,;
                QdbQualifier.qualifierId = Convert.ToInt32(input.Substring(0, input.IndexOf(":")));
            }
            else
            {// no ":" was found - return an un-populated Qdb object
                return QdbQualifier;
            }

            input = input.Substring(input.IndexOf(":")+1); // hack off the first ":" and chars to its left - this is the QdbId and a delimiting ":"

            string[] parms = input.Split(';');

            foreach (string parm in parms)
            {// each paramater split out by ";"
                if (parm.Trim() != "")
                {
                    string[] valueAndUom = parm.Split(',');
                    if (valueAndUom.Count() == 2 && valueAndUom[0].Trim() != "")
                    {
                        if (valueAndUom[1].Trim() == "")
                        {
                            QdbQualifier.qualifierParameters.Add(escapeXMLspecialChars(valueAndUom[0]));
                        }
                        else
                        {
                            QdbQualifier.qualifierParameters.Add(escapeXMLspecialChars(valueAndUom[0]) + " " + valueAndUom[1]);
                        }
                    }
                }
            }
            return QdbQualifier;
        }

        private string escapeXMLspecialChars(string inputString)
        {
            string outputString = inputString;
            if (!string.IsNullOrEmpty(outputString))
            {
                outputString = outputString.Replace("&", "&amp;");
                outputString = outputString.Replace("<", "&lt;");
                outputString = outputString.Replace(">", "&gt;");
                outputString = outputString.Replace("'", "&apos;");
                outputString = outputString.Replace("\"", "&quot;");
            }
            return outputString;
        }



        public string exportRelatedParts(string _filePath, PCdb pcdb, string leftType, string rightType, bool usePosition, bool useVCdbAttributes, bool useNotes, IProgress<int> progress)
        {
            // establish a hashtable - "vehicle" is the key and partlist is the value.
            // definition of "vehilce" is variable - basevehicleid+[position]+[vcdb-coded-qualifiers]+[notes]
            // a "vehilce" key is strung together with tabs like one of these examples: 
            //   2270\t\t
            //   2270\t22\t\t
            //   2270\t\tPerformance Package
            //   2270\t22\t
            //   2270\t22\tEngineBase:22;Submodel:1;Performance Package
            Dictionary<string, List<String>> vehilceParts = new Dictionary<string, List<String>>();
            string vehicleKey = "";
            List<String> partsListtemp;
            List<String> partsPairings = new List<string>();
            String returnString = "no part relationships found";
            int percentProgress = 0;
            int i=0;


            foreach (App app in apps)
            {// establish the reference (lefthand) list
                if (app.action == "D") { continue; } // ignore "Delete" apps
                vehicleKey = app.basevehicleid.ToString();
                if(usePosition){vehicleKey += "\t" + app.positionid.ToString();}
                if(useVCdbAttributes){vehicleKey += "\t" + app.namevalpairString(false);}
                if(useNotes){vehicleKey += "\t" + string.Join(";", app.notes);}

                if (pcdb.niceParttype(app.parttypeid) == leftType)
                {
                    if (vehilceParts.ContainsKey(vehicleKey))
                    {
                        vehilceParts[vehicleKey].Add(app.part);
                    }
                    else
                    {
                        vehilceParts.Add(vehicleKey, partsListtemp = new List<string>());
                    }
                }

                if (progress != null)
                {// only report progress on whole percentage steps (100 total reports). reporting on every iteration is too process intensive
                    percentProgress = Convert.ToInt32(((double)i / (double)apps.Count()) * 5);
                    if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); }
                }

                i++;
            }


            i = 0;

            foreach (App app in apps)
            {// look for matching keys in the reference list
                if (app.action == "D") { continue; } // ignore "Delete" apps
                vehicleKey = app.basevehicleid.ToString();
                if(usePosition){vehicleKey += "\t" + app.positionid.ToString();}
                if(useVCdbAttributes){vehicleKey += "\t" + app.namevalpairString(false);}
                if(useNotes){vehicleKey += "\t" + string.Join(";", app.notes);}

                if (pcdb.niceParttype(app.parttypeid) == rightType)
                {
                    if (vehilceParts.ContainsKey(vehicleKey))
                    {
                        foreach (String tempLeftPart in vehilceParts[vehicleKey])
                        {
                            if (!partsPairings.Contains(tempLeftPart + "\t" + app.part))
                            {
                                partsPairings.Add(tempLeftPart + "\t" + app.part);
                            }
                        }
                    }
                }
                if (progress != null)
                {// only report progress on whole percentage steps (100 total reports). reporting on every iteration is too process intensive
                    percentProgress = 5 + Convert.ToInt32(((double)i / (double)apps.Count()) * 95);
                    if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); }
                }
                i++;
            }


            if (partsPairings.Count() > 0)
            {
                partsPairings.Sort();
                try
                {
                    using (StreamWriter sw = new StreamWriter(_filePath))
                    {
                        foreach (String pairing in partsPairings)
                        {
                            sw.WriteLine(pairing);
                        }
                    }
                    returnString = partsPairings.Count().ToString() + " parings of parts written to: " + _filePath;
                }
                catch (Exception ex)
                {
                    returnString=ex.ToString();
                }
             
            }

            return returnString;

        }



        public string exportFlatApps(string _filePath, VCdb vcdb, PCdb pcdb, Qdb qdb, string delimiter, string format, IProgress<int> progress)
        {
            BaseVehicle basevehicle = new BaseVehicle();
            int percentProgress = 0;
            int i = 0;
            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath))
                {

                    switch (format)
                    {
                        case "Exploded VCdb tag columns":

                            Dictionary<string, int> tagDict = new Dictionary<string, int>(){
                            {"SubModel",0},{"MfrBodyCode",1},{"BodyNumDoors",2},{"BodyType",3},{"DriveType",4},{"EngineBase",5},{"EngineDesignation",6},
                            {"EngineVIN",7},{"EngineVersion",8},{"EngineMfr",9},{"PowerOutput",10},{"ValvesPerEngine",11},{"FuelDeliveryType",12},
                            {"FuelDeliverySubType",13},{"FuelSystemControlType",14},{"FuelSystemDesign",15},{"Aspiration",16},{"CylinderHeadType",17},
                            {"FuelType",18},{"IgnitionSystemType",19},{"TransmissionMfrCode",20},{"TransmissionBase",21},{"TransmissionType",22},
                            {"TransmissionControlType",23},{"TransmissionNumSpeeds",24},{"TransElecControlled",25},{"TransmissionMfr",26},
                            {"BedLength",27},{"BedType",28},{"WheelBase",29},{"BrakeSystem",30},{"FrontBrakeType",31},{"RearBrakeType",32},
                            {"BrakeABS",33},{"FrontSpringType",34},{"RearSpringType",35},{"SteeringSystem",36},{"SteeringType",37},{"Region",38}};

                            string[] VCdbAttributeFields = new string[39];
                            string VCdbTagHeaders = ""; foreach (KeyValuePair<string, int> entry in tagDict) { VCdbTagHeaders += "\t" + entry.Key; }
                            sw.WriteLine("Application id" + delimiter + "Reference" + delimiter + "Base Vehicle id" + delimiter + "Make" + delimiter + "Model" + delimiter + "Year" + delimiter + "Part" + delimiter + "Part Type" + delimiter + "Position" + delimiter + "Quantity" + delimiter + "Qdb-coded Qualifiers" + delimiter + "Notes" + delimiter + "Mfr Label" + delimiter + "Asset" + VCdbTagHeaders);

                            foreach (App app in apps)
                            {
                                if (app.action == "D") { continue; } // don't export deleted apps
                                for (int j = 0; j <= 37; j++) { VCdbAttributeFields[j] = ""; }

                                foreach (VCdbAttribute myAttribute in app.VCdbAttributes)
                                {
                                    if (tagDict.ContainsKey(myAttribute.name))
                                    {
                                        VCdbAttributeFields[tagDict[myAttribute.name]] = myAttribute.value.ToString();
                                    }
                                }

                                sw.WriteLine(app.id.ToString() + delimiter + app.reference + delimiter + app.basevehicleid.ToString() + delimiter + vcdb.niceMakeOfBasevid(app.basevehicleid) + delimiter + vcdb.niceModelOfBasevid(app.basevehicleid) + delimiter + vcdb.niceYearOfBasevid(app.basevehicleid) + delimiter + app.part + delimiter + pcdb.niceParttype(app.parttypeid) + delimiter + pcdb.nicePosition(app.positionid) + delimiter + app.quantity.ToString() + delimiter + app.niceQdbQualifierString(qdb) + delimiter + string.Join(";", app.notes) + delimiter + app.mfrlabel + delimiter + app.asset + delimiter + string.Join(delimiter, VCdbAttributeFields.ToList()));
                                if (progress != null) { percentProgress = Convert.ToInt32(((double)i / (double)apps.Count()) * 100); if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); } }
                                i++;
                            }
                            break;

                        case "Default":
                            sw.WriteLine("Application id" + delimiter + "Reference" + delimiter + "Base Vehicle id" + delimiter + "Make" + delimiter + "Model" + delimiter + "Year" + delimiter + "Part" + delimiter + "Part Type" + delimiter + "Position" + delimiter + "Quantity" + delimiter + "VCdb-coded Attributes" + delimiter + "Qdb-coded Qualifiers" + delimiter + "Notes" + delimiter + "Mfr Label" + delimiter + "Asset" + delimiter + "Asset Item Order");
                            foreach (App app in apps)
                            {
                                if (app.action == "D") { continue; } // don't export deleted apps
                                sw.WriteLine(app.id.ToString() + delimiter + app.reference + delimiter + app.basevehicleid.ToString() + delimiter + vcdb.niceMakeOfBasevid(app.basevehicleid) + delimiter + vcdb.niceModelOfBasevid(app.basevehicleid) + delimiter + vcdb.niceYearOfBasevid(app.basevehicleid) + delimiter + app.part + delimiter + pcdb.niceParttype(app.parttypeid) + delimiter + pcdb.nicePosition(app.positionid) + delimiter + app.quantity.ToString() + delimiter + app.niceAttributesString(vcdb, false) + delimiter + app.niceQdbQualifierString(qdb) + delimiter + string.Join(";", app.notes) + delimiter + app.mfrlabel + delimiter + app.asset + delimiter + app.assetitemorder);
                                if (progress != null) { percentProgress = Convert.ToInt32(((double)i / (double)apps.Count()) * 100); if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); } }
                                i++;
                            }
                            break;

                        case "Include Deletes":
                            sw.WriteLine("Action" + delimiter + "Application id" + delimiter + "Reference" + delimiter + "Base Vehicle id" + delimiter + "Make" + delimiter + "Model" + delimiter + "Year" + delimiter + "Part" + delimiter + "Part Type" + delimiter + "Position" + delimiter + "Quantity" + delimiter + "VCdb-coded Attributes" + delimiter + "Qdb-coded Qualifiers" + delimiter + "Notes" + delimiter + "Mfr Label" + delimiter + "Asset" + delimiter + "Asset Item Order");
                            foreach (App app in apps)
                            {
                                sw.WriteLine(app.action.ToString() + delimiter + app.id.ToString() + delimiter + app.reference + delimiter + app.basevehicleid.ToString() + delimiter + vcdb.niceMakeOfBasevid(app.basevehicleid) + delimiter + vcdb.niceModelOfBasevid(app.basevehicleid) + delimiter + vcdb.niceYearOfBasevid(app.basevehicleid) + delimiter + app.part + delimiter + pcdb.niceParttype(app.parttypeid) + delimiter + pcdb.nicePosition(app.positionid) + delimiter + app.quantity.ToString() + delimiter + app.niceAttributesString(vcdb, false) + delimiter + app.niceQdbQualifierString(qdb) + delimiter + string.Join(";", app.notes) + delimiter + app.mfrlabel + delimiter + app.asset + delimiter + app.assetitemorder);
                                if (progress != null) { percentProgress = Convert.ToInt32(((double)i / (double)apps.Count()) * 100); if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); } }
                                i++;
                            }
                            break;


                        case "Coded-Values":
                            sw.WriteLine("Application id" + delimiter + "Reference" + delimiter + "BaseVehicleid" + delimiter + "Part" + delimiter + "PartTypeid" + delimiter + "Positionid" + delimiter + "Quantity" + delimiter + "VCdb-coded Attributes" + delimiter + "Qdb-coded Qualifiers" + delimiter + "Notes" + delimiter + "Mfr Label" + delimiter + "Asset" + delimiter + "Asset Item Order");
                            foreach (App app in apps)
                            {
                                if (app.action == "D") { continue; } // don't export deleted apps
                                sw.WriteLine(app.id.ToString() + delimiter + app.reference + delimiter + app.basevehicleid.ToString() + delimiter + app.part + delimiter + app.parttypeid.ToString() + delimiter + app.positionid.ToString() + delimiter + app.quantity.ToString() + delimiter + app.namevalpairString(false) + delimiter + app.rawQdbDataString() + delimiter + string.Join(";", app.notes) + delimiter + app.mfrlabel + delimiter + app.asset + delimiter + app.assetitemorder);
                                if (progress != null){percentProgress = Convert.ToInt32(((double)i / (double)apps.Count()) * 100); if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); }}
                                i++;
                            }
                            break;

                        default: break;
                    }   
                }
                return  apps.Count().ToString() + " flat applications exported to " + _filePath;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        public string exportAppGuide(string _filePath, VCdb vcdb, PCdb pcdb, Qdb qdb, string delimiter, string format, IProgress<int> progress)
        {
            BaseVehicle basevehicle = new BaseVehicle();
            int percentProgress = 0;
            int i = 0;
            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath))
                {

                    switch (format)
                    {
                        // 







                        case "single column":
                            sw.WriteLine("Application id" + delimiter + "Reference" + delimiter + "Base Vehicle id" + delimiter + "Make" + delimiter + "Model" + delimiter + "Year" + delimiter + "Part" + delimiter + "Part Type" + delimiter + "Position" + delimiter + "Quantity" + delimiter + "VCdb-coded Attributes" + delimiter + "Qdb-coded Qualifiers" + delimiter + "Notes" + delimiter + "Mfr Label" + delimiter + "Asset" + delimiter + "Asset Item Order");
                            foreach (App app in apps)
                            {
                                if (app.action == "D") { continue; } // don't export deleted apps
                                sw.WriteLine(app.id.ToString() + delimiter + app.reference + delimiter + app.basevehicleid.ToString() + delimiter + vcdb.niceMakeOfBasevid(app.basevehicleid) + delimiter + vcdb.niceModelOfBasevid(app.basevehicleid) + delimiter + vcdb.niceYearOfBasevid(app.basevehicleid) + delimiter + app.part + delimiter + pcdb.niceParttype(app.parttypeid) + delimiter + pcdb.nicePosition(app.positionid) + delimiter + app.quantity.ToString() + delimiter + app.niceAttributesString(vcdb, false) + delimiter + app.niceQdbQualifierString(qdb) + delimiter + string.Join(";", app.notes) + delimiter + app.mfrlabel + delimiter + app.asset + delimiter + app.assetitemorder);
                                if (progress != null) { percentProgress = Convert.ToInt32(((double)i / (double)apps.Count()) * 100); if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); } }
                                i++;
                            }
                            break;


                        default: break;
                    }
                }
                return apps.Count().ToString() + " application guide exported to " + _filePath;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string exportBuyersGuide(string _filePath, VCdb vcdb, IProgress<int> progress)
        {
            int i; bool deletedElement; string lineString = ""; int percentProgress = 0;
            List<buyersguideApplication> bg = new List<buyersguideApplication>();
            foreach (App app in apps)
            {
                buyersguideApplication bgAppTemp = new buyersguideApplication();
                bgAppTemp.part = app.part;
                bgAppTemp.MakeName = vcdb.niceMakeOfBasevid(app.basevehicleid);
                bgAppTemp.ModelName = vcdb.niceModelOfBasevid(app.basevehicleid);
                bgAppTemp.startYear= Convert.ToInt32(vcdb.niceYearOfBasevid(app.basevehicleid));
                bgAppTemp.endYear = bgAppTemp.startYear;
                bg.Add(bgAppTemp);
            }
            bg.Sort();


            deletedElement = true;
            while (deletedElement)
            {
                deletedElement = false;
                for (i = 0; i <= bg.Count() - 2; i++)
                {
                    if (bg[i].part == bg[i + 1].part &&  bg[i].MakeName == bg[i + 1].MakeName && bg[i].ModelName == bg[i + 1].ModelName &&   (bg[i + 1].startYear - bg[i].endYear)<=1)
                    {
                        bg[i].endYear = bg[i + 1].startYear;
                        deletedElement = true;
                        break;
                    }
                }
                if (deletedElement)
                {
                    bg.RemoveAt(i + 1);
                    if (progress != null)
                    {// only report progress on whole percentage steps (100 total reports). reporting on every iteration is too process intensive
                        percentProgress = Convert.ToInt32(((double)i / (double)bg.Count()) * 100);
                        if ((double)percentProgress % (double)1 == 0) { progress.Report(percentProgress); }
                    }

                }
            }


            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath))
                {
                    lineString = bg[0].part + "\t";
                    for (i=0; i<=bg.Count()-1; i++)
                    {
                        if (i == (bg.Count() - 1) || bg[i].part != bg[i + 1].part)
                        {// new part (or final element)

                            if (bg[i].startYear == bg[i].endYear)
                            {
                                lineString += bg[i].MakeName + " " + bg[i].ModelName + " (" + bg[i].startYear.ToString() + ")";
                            }
                            else
                            {
                                lineString += bg[i].MakeName + " " + bg[i].ModelName + " (" + bg[i].startYear.ToString() + "-" + bg[i].endYear.ToString() + ")";
                            }
                            sw.WriteLine(lineString);
                            if (i < (bg.Count() - 1)) { lineString = bg[i + 1].part + "\t"; }
                        }
                        else
                        {// same part as last element

                            if (bg[i].startYear == bg[i].endYear)
                            {
                                lineString += bg[i].MakeName + " " + bg[i].ModelName + " (" + bg[i].startYear.ToString() + "), ";
                            }
                            else
                            {
                                lineString += bg[i].MakeName + " " + bg[i].ModelName + " (" + bg[i].startYear.ToString() + "-" + bg[i].endYear.ToString() + "), ";
                            }
                        }
                    }
                }
                return "Buyer's guide exported to " + _filePath;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        
        public string exportHolesReport(VCdb vcdb,  string _filePath)
        {// holes based on what the VCdb contains that is not in the aces file
            // the comparison is by basevehilce (make/model/year) 
            BaseVehicle basevehicle = new BaseVehicle();
            List<BaseVehicle> missingBasevids = new List<BaseVehicle>();
            int i = 0;
            int hitcount = 0;

            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath))
                {
                    sw.WriteLine("BaseVid\tMake\tModel\tYear\tVehicle Type");

                    foreach (KeyValuePair<int, BaseVehicle> entry in vcdb.vcdbBasevhicleDict)
                    {
                        i++;
                        if (!basevidOccurrences.ContainsKey(entry.Key))
                        {
                            //if ( Convert.ToInt32(entry.Value.YearId) >= 2016 && entry.Value.VehicleTypeName=="Car")
                            //{

                            sw.WriteLine(entry.Key.ToString() + "\t" + entry.Value.MakeName + "\t" + entry.Value.ModelName + "\t" + entry.Value.YearName + "\t" + entry.Value.VehicleTypeName);
                            hitcount++;
                            //}
                        }
                    }
                }
                return hitcount.ToString() + " missing base vehicles exported to " + _filePath;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        
        public string exportVCdbUsageReport(VCdb vcdb, string _filePath)
        {
            var sortedVCbDict = vcdbUsageStatsDict.ToList(); sortedVCbDict.Sort((pair2, pair1) => pair1.Value.CompareTo(pair2.Value));
            VCdbAttribute VCdbAttributeTemp = new VCdbAttribute();

            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath))
                {
                    string lineString = "\t\t"+ vcdbUsageStatsTotalApps.ToString();
                    sw.WriteLine(lineString);
                    lineString = "Attribute Name\tOccurrence Count\t% of Records";
                    sw.WriteLine(lineString);
                    foreach (KeyValuePair<string, int> entry in sortedVCbDict)
                    {
                        lineString = entry.Key + "\t" + entry.Value.ToString() + "\t" + (Convert.ToDouble(entry.Value*100)/Convert.ToDouble(vcdbUsageStatsTotalApps)).ToString("F2");
                        sw.WriteLine(lineString);
                    }
                    sw.WriteLine("\r\n\r\n\r\n");
                    foreach(string fileName in vcdbUsageStatsFileList){sw.WriteLine(fileName);}
                }
                return "VCdb usage stats exported to " + _filePath;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string exportAssetsList(string _filePath)
        {
            Dictionary<string, string> distinctAssetNamesFromApps = new Dictionary<string, string>();

            try
            {
                foreach(App apptemp in apps)
                {
                    if (!distinctAssetNamesFromApps.ContainsKey(apptemp.asset)) { distinctAssetNamesFromApps.Add(apptemp.asset, ""); }
                }

                using (StreamWriter sw = new StreamWriter(_filePath))
                {
                    sw.WriteLine("Asset\tSource");

                    foreach (KeyValuePair<string,string> entry in distinctAssetNamesFromApps)
                    {
                        sw.WriteLine(entry.Key + "\tApps Section");
                    }

                    foreach (String assetName in distinctAssetNames)
                    {
                        sw.WriteLine(assetName+"\tStand-Alone Assets Section");
                    }
                }
                return distinctAssetNamesFromApps.Count.ToString()+" fitment assets, "+ distinctAssetNames.Count.ToString()+" stand-alone assets list exported to " + _filePath;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        // if cipherFilepath is populated, or anonymize is set, the partnumbers will be enciphered
        // the purpose of "anonymize" is for uploading xml datasets made purely of VCdb-configuration errors to AutoCare's "assessmentErrorReporting" API.
        public string exportXMLApps(string _filePath, string _SubmissionType, string cipherFilePath,bool anonymize)
        {// dump an xml file from this ACES instance

            Random rnd = new Random();
            Dictionary<String, String> cipherDict = new Dictionary<string, string>();
            string exportVersion = "3.2";
            string brandAAIAIDtag = ""; if (BrandAAIAID != null && !anonymize) { brandAAIAIDtag = "\t\t<BrandAAIAID>" + BrandAAIAID + "</BrandAAIAID>\r\n"; }
            //string companyTag = "";
            
            int relativeAppId = 1;
            string attributesXMLtags = ""; //\t\t<EngineBase id=\"221\"/>\r\n
            string mfrlabelXMLtags = "";// \t\t<MfrLabel>"+app.mfrlabel+"</MfrLabel>\r\n
            string positionXMLtags = ""; // \t\t<Position id=\"22\"/>\r\n
            string notesXMLtags = "";// \t\t<Note>"+app.notes+"</Note>\r\n
            string qdbXMLtags = "";
            string assetnameXMLtags = "";// \t\t<AssetName>"+app.asset+"</AssetName>\r\n
            string assetitemorderXMLtags = "";

            List<string> stringList = new List<string>();

            string escapedPart;
            string randomPart="";
            
            var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789"; // avoid visually ambiguous characters (O0I)
            char[] trimlist = {' ', ';'};

            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n<ACES version=\"" + exportVersion + "\">\r\n\t<Header>\r\n\t\t<Company>" + SecurityElement.Escape(Company) + "</Company>\r\n\t\t<SenderName>" + SecurityElement.Escape(SenderName) + "</SenderName>\r\n\t\t<SenderPhone>" + SenderPhone + "</SenderPhone>\r\n\t\t<TransferDate>" + TransferDate + "</TransferDate>\r\n" + brandAAIAIDtag + "\t\t<DocumentTitle>" + SecurityElement.Escape(DocumentTitle) + "</DocumentTitle>\r\n\t\t<EffectiveDate>" + EffectiveDate + "</EffectiveDate>\r\n\t\t<SubmissionType>" + _SubmissionType + "</SubmissionType>\r\n\t\t<VcdbVersionDate>" + VcdbVersionDate + "</VcdbVersionDate>\r\n\t\t<QdbVersionDate>" + QdbVersionDate + "</QdbVersionDate>\r\n\t\t<PcdbVersionDate>" + PcdbVersionDate + "</PcdbVersionDate>\r\n\t</Header>");
                    foreach (App app in apps)
                    {
                        escapedPart = SecurityElement.Escape(app.part);

                        if (cipherFilePath != "")
                        {
                            if (cipherDict.ContainsKey(escapedPart))
                            {
                                escapedPart = cipherDict[escapedPart];
                            }
                            else
                            {
                                while (cipherDict.ContainsValue(randomPart) || randomPart == "")
                                {
                                    var stringChars = new char[4];
                                    for (int i = 0; i < stringChars.Length; i++)
                                    {
                                        stringChars[i] = chars[rnd.Next(chars.Length)];
                                    }
                                    randomPart = new String(stringChars);
                                }
                                cipherDict[escapedPart] = randomPart;
                                escapedPart = randomPart;
                            }
                        }

                        attributesXMLtags = "";
                        if (app.VCdbAttributes.Count > 0)
                        {
                            app.VCdbAttributes.Sort(); // this uses the class' custom Icomparable method to sort attributes into the bizare order that the XSD mandates.
                            foreach (VCdbAttribute myAttribute in app.VCdbAttributes)
                            {
                                attributesXMLtags += "\t\t<" + myAttribute.name + " id=\"" + myAttribute.value.ToString() + "\"/>\r\n";
                            }
                        }


                        qdbXMLtags = "";
                        List<string> paramList = new List<string>();

                        if (app.QdbQualifiers.Count() > 0)
                        {
                            foreach (QdbQualifier qdb in app.QdbQualifiers)
                            {
                                // build params list 
                                // <param value="Super"/>

                                paramList.Clear();
                                foreach (string param in qdb.qualifierParameters)
                                {
                                    paramList.Add("<param value=\"" + SecurityElement.Escape(param) + "\"/>");
                                }

                                if (paramList.Count > 0)
                                {
                                    qdbXMLtags += "\t\t<Qual id=\"" + qdb.qualifierId.ToString() + "\">" + String.Join("", paramList.ToArray()) + "<text></text></Qual>\r\n";
                                }
                                else 
                                {
                                    qdbXMLtags += "\t\t<Qual id=\"" + qdb.qualifierId.ToString() + "\"><text></text></Qual>\r\n";
                                }
                            }

                        }
                        //ddd return String.Join(";", stringList.ToArray());

                        notesXMLtags = "";
                        if (app.notes.Count() > 0)
                        {
                            foreach (string noteString in app.notes)
                            {
                                notesXMLtags += "\t\t<Note>" + SecurityElement.Escape(noteString.Trim(trimlist)) + "</Note>\r\n";
                            }
                        }






                        mfrlabelXMLtags = ""; if (app.mfrlabel != null && app.mfrlabel != "") { mfrlabelXMLtags = "\t\t<MfrLabel>" + SecurityElement.Escape(app.mfrlabel) + "</MfrLabel>\r\n"; }
                        positionXMLtags = ""; if (app.positionid > 0) { positionXMLtags = "\t\t<Position id=\"" + app.positionid.ToString() + "\"/>\r\n"; }
                        assetnameXMLtags = ""; if (app.asset != null && app.asset != "") { assetnameXMLtags = "\t\t<AssetName>" + SecurityElement.Escape(app.asset) + "</AssetName>\r\n"; }
                        assetitemorderXMLtags = ""; if (app.assetitemorder > 0) { assetitemorderXMLtags = "\t\t<AssetItemOrder>" + SecurityElement.Escape(app.assetitemorder.ToString()) + "</AssetItemOrder>\r\n"; }
                        sw.WriteLine("\t<App action=\"" + app.action.ToString() + "\" id=\"" + relativeAppId.ToString() + "\">\r\n\t\t<BaseVehicle id=\"" + app.basevehicleid + "\"/>\r\n" + attributesXMLtags + qdbXMLtags + notesXMLtags + "\t\t<Qty>" + app.quantity.ToString() + "</Qty>\r\n\t\t<PartType id=\"" + app.parttypeid.ToString() + "\"/>\r\n" + mfrlabelXMLtags + positionXMLtags + "\t\t<Part>" + escapedPart + "</Part>\r\n"+ assetnameXMLtags + assetitemorderXMLtags + "\t</App>");
                        relativeAppId++;
                    }
                    // write assets sections
                    

                    sw.WriteLine("\t<Footer>\r\n\t\t<RecordCount>" + apps.Count.ToString() + "</RecordCount>\r\n\t</Footer>\r\n</ACES>");
                }

                if (cipherFilePath != "")
                {
                    using (StreamWriter sw = new StreamWriter(cipherFilePath))
                    {
                        foreach (KeyValuePair<string, string> entry in cipherDict)
                        {
                            sw.WriteLine(entry.Value + "\t" + entry.Key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return apps.Count.ToString() + " apps exported as ACES xml file: " + _filePath;
        }









        // store stats about the analysis results in the registry using the md5hash of the ACES file as the key
        // elements are stored as CSS-style name:value; pairs
        // the point is to be able to know if a file has already passed validation for doing 2-file diff calculation. We want to know that the "reference" file is legit (from a previous 
        // validation session. It also paves the way for showing and/or reporting history of validations at some point in the the future
        public void recordAnalysisResults(string vcdbVersion, string pcdbVersion)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            key.SetValue(fileMD5hash, "fileName:" + Path.GetFileName(filePath) + ";ACESversion:" + version + ";validatedAgainstVCdb:" + vcdbVersion + ";validatedAgainstPCdb:" + pcdbVersion + ";errorCount:" + (parttypePositionErrorsCount+vcdbCodesErrorsCount+vcdbConfigurationsErrorsCount+basevehicleidsErrorsCount+qdbErrorsCount+questionableNotesCount+vcdbConfigurationsErrorsCount).ToString() + ";warningCount:" + (parttypeDisagreementCount+qtyOutlierCount).ToString() + ";applicationCount:" + apps.Count.ToString() + ";partCount:" + partsAppCounts.Count.ToString() + ";");
        }


        // lookup loaded aces file (by hash) in registry to see if it has previously passed validation against the currently-loaded VCdb/PCdb version
        public int fileHasBeenAnalyzed(string vcdbVersion, string pcdbVersion)
        {
            //0=no record of file
            //1=file failed validation 
            //2=file passed validations

            int returnValue = 0;

            string value = ""; string validatedAgainstVCdb = ""; string validatedAgainstPCdb = ""; int errorCount = -1;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if (key.GetValue(fileMD5hash) != null)
            {//file has was found - we'v seen this ACES file in the past
                returnValue = 1;
                value =key.GetValue(fileMD5hash).ToString();
                string[] pairs = value.Split(';');
                foreach(string pair in pairs)
                {
                    string[] pieces = pair.Split(':');
                    if(pieces.Count() == 2)
                    {
                        if(pieces[0]== "validatedAgainstVCdb"){ validatedAgainstVCdb = pieces[1];}
                        if(pieces[0] == "validatedAgainstPCdb") { validatedAgainstPCdb = pieces[1]; }
                        if (pieces[0] == "errorCount") { errorCount = Convert.ToInt32(pieces[1]);}
                    }
                }
                if(errorCount==0 && vcdbVersion==validatedAgainstVCdb && pcdbVersion == validatedAgainstPCdb){ returnValue = 2; }
            }
            return returnValue;
        }

    }

    // the contents of the VCdb are loaded into hash tables ("Dictionaries") for the sake of speed. (Local Access file or remote database)
    public class VCdb
    {
        public bool useRemoteDB;
        public bool importVCdbConfigData;
        public OleDbConnection connectionOLEDB = new OleDbConnection();  // one single connection to the local OLEDB MS Access engine
        public string MySQLdatabaseName = "";
        public string MySQLusername = "";
        public string MySQLpassword = "";
        public string MySQLconnectionString = "";
        public List<string> vcdbVersionsOnServerList = new List<string>();
        public List<MySqlConnection> connectionMySQLlist = new List<MySqlConnection>();  // potentially multiple (in-parallel) connections to a remote MySQL database (one per analysis "chunk")
        public bool importSuccess = false;
        public string importExceptionMessage = "";
        public string filePath = "";
        public string version = "";
        public int importProgress = 0;
        public bool importIsRunning = false;

        public Dictionary<int, BaseVehicle> vcdbBasevhicleDict = new Dictionary<int, BaseVehicle>();
        public Dictionary<string,int> vcdbReverseBasevhicleDict = new Dictionary<string, int>();
        public Dictionary<int, String> enginebaseDict = new Dictionary<int, string>();
        public Dictionary<int, String> engineblockDict = new Dictionary<int, string>();
        public Dictionary<int, String> submodelDict = new Dictionary<int, string>();
        public Dictionary<int, String> drivetypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> aspirationDict = new Dictionary<int, string>();
        public Dictionary<int, String> fueltypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> braketypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> brakeabsDict = new Dictionary<int, string>();
        public Dictionary<int, String> mfrbodycodeDict = new Dictionary<int, string>();
        public Dictionary<int, String> bodynumdoorsDict = new Dictionary<int, string>();
        public Dictionary<int, String> bodytypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> enginedesignationDict = new Dictionary<int, string>();
        public Dictionary<int, String> enginevinDict = new Dictionary<int, string>();
        public Dictionary<int, String> engineversionDict = new Dictionary<int, string>();
        public Dictionary<int, String> mfrDict = new Dictionary<int, string>();
        public Dictionary<int, String> fueldeliverytypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> fueldeliverysubtypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> fuelsystemcontroltypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> fuelsystemdesignDict = new Dictionary<int, string>();
        public Dictionary<int, String> cylinderheadtypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> ignitionsystemtypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> transmissionmfrcodeDict = new Dictionary<int, string>();
        public Dictionary<int, String> transmissionbaseDict = new Dictionary<int, string>();
        public Dictionary<int, String> transmissiontypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> transmissioncontroltypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> transmissionnumspeedsDict = new Dictionary<int, string>();
        public Dictionary<int, String> transmissioeleccontrolledDict = new Dictionary<int, string>();
        public Dictionary<int, String> bedlengthDict = new Dictionary<int, string>();
        public Dictionary<int, String> bedtypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> wheelbaseDict = new Dictionary<int, string>();
        public Dictionary<int, String> brakesystemDict = new Dictionary<int, string>();
        public Dictionary<int, String> regionDict = new Dictionary<int, string>();
        public Dictionary<int, String> springtypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> steeringsystemDict = new Dictionary<int, string>();
        public Dictionary<int, String> steeringtypeDict = new Dictionary<int, string>();
        public Dictionary<int, String> valvesDict = new Dictionary<int, string>();
        public Dictionary<int, String> poweroutputDict = new Dictionary<int, string>();

        public Dictionary<int, List<KeyValuePair<string, string>>> deletedEngineBaseDict = new Dictionary<int, List<KeyValuePair<string, string>>>();


        public string connectLocalOLEDB(string path)
        {
            string result = "";
            filePath = path;
            try
            {
                if (connectionOLEDB.State != System.Data.ConnectionState.Closed)
                {
                    connectionOLEDB.Close();
                }
                connectionOLEDB.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Mode=Read";
                connectionOLEDB.Open();
            }
            catch (Exception ex) { result = ex.Message; }
            return result;
        }

        // instance a new connection and add it to the list of connections
        public string addNewMySQLconnection()
        {
            string result = "";
            try
            {
                MySqlConnection newConnection = new MySqlConnection(MySQLconnectionString);
                newConnection.Open();
                connectionMySQLlist.Add(newConnection);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public void disconnect()
        {
            if (useRemoteDB)
            {// disconnect all mysql connections
                try
                {
                    for (int i = 0; i < connectionMySQLlist.Count(); i++)
                    {
                        connectionMySQLlist[i].Close();
                        connectionMySQLlist[i].Dispose();
                    }
                    connectionMySQLlist.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {// disconnect from the local database file
                filePath = "";
                if (connectionOLEDB.State != System.Data.ConnectionState.Closed)
                {
                    connectionOLEDB.Close();
                }
            }
        }



        public void clear()
        {
            useRemoteDB = false;
            MySQLdatabaseName = "";
            MySQLusername = "";
            MySQLpassword = "";
            MySQLconnectionString = "";
            importSuccess = false;
            importExceptionMessage = "";
            filePath = "";
            version = "";
            vcdbBasevhicleDict.Clear();
            vcdbReverseBasevhicleDict.Clear();
            enginebaseDict.Clear();
            engineblockDict.Clear();
            submodelDict.Clear();
            drivetypeDict.Clear();
            aspirationDict.Clear();
            fueltypeDict.Clear();
            braketypeDict.Clear();
            brakeabsDict.Clear();
            mfrbodycodeDict.Clear();
            bodynumdoorsDict.Clear();
            bodytypeDict.Clear();
            enginedesignationDict.Clear();
            enginevinDict.Clear();
            engineversionDict.Clear();
            mfrDict.Clear();
            fueldeliverytypeDict.Clear();
            fueldeliverysubtypeDict.Clear();
            fuelsystemcontroltypeDict.Clear();
            fuelsystemdesignDict.Clear();
            cylinderheadtypeDict.Clear();
            ignitionsystemtypeDict.Clear();
            transmissionmfrcodeDict.Clear();
            transmissionbaseDict.Clear();
            transmissiontypeDict.Clear();
            transmissioncontroltypeDict.Clear();
            transmissionnumspeedsDict.Clear();
            transmissioeleccontrolledDict.Clear();
            bedlengthDict.Clear();
            bedtypeDict.Clear();
            wheelbaseDict.Clear();
            brakesystemDict.Clear();
            regionDict.Clear();
            springtypeDict.Clear();
            steeringsystemDict.Clear();
            steeringtypeDict.Clear();
            valvesDict.Clear();
            poweroutputDict.Clear();
            disconnect();
        }


        public string niceAttribute(VCdbAttribute attribute)
        {

            string niceValue = "";
            string returnValue = "";
            bool gotValue = false;

            switch (attribute.name)
            {
                case "EngineBase": gotValue = enginebaseDict.TryGetValue(attribute.value, out niceValue); break;
                case "EngineBlock": gotValue = engineblockDict.TryGetValue(attribute.value, out niceValue); break;
                case "SubModel": gotValue = submodelDict.TryGetValue(attribute.value, out niceValue); break;
                case "DriveType": gotValue = drivetypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "Aspiration": gotValue = aspirationDict.TryGetValue(attribute.value, out niceValue); break;
                case "FuelType": gotValue = fueltypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "FrontBrakeType": gotValue = braketypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "RearBrakeType": gotValue = braketypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "BrakeABS": gotValue = brakeabsDict.TryGetValue(attribute.value, out niceValue); break;
                case "MfrBodyCode": gotValue = mfrbodycodeDict.TryGetValue(attribute.value, out niceValue); break;
                case "BodyNumDoors": gotValue = bodynumdoorsDict.TryGetValue(attribute.value, out niceValue); break;
                case "BodyType": gotValue = bodytypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "EngineDesignation": gotValue = enginedesignationDict.TryGetValue(attribute.value, out niceValue); break;
                case "EngineVIN": gotValue = enginevinDict.TryGetValue(attribute.value, out niceValue); break;
                case "EngineVersion": gotValue = engineversionDict.TryGetValue(attribute.value, out niceValue); break;
                //case "EngineMfr": gotValue = enginebase.TryGetValue(attribute.value, out niceValue); break;   // changed in 1.0.1.12 - Edgenet's devteam spotted this error
                case "EngineMfr": gotValue = mfrDict.TryGetValue(attribute.value, out niceValue); break;
                case "FuelDeliveryType": gotValue = fueldeliverytypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "FuelDeliverySubType": gotValue = fueldeliverysubtypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "FuelSystemControlType": gotValue = fuelsystemcontroltypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "FuelSystemDesign": gotValue = fuelsystemdesignDict.TryGetValue(attribute.value, out niceValue); break;
                case "CylinderHeadType": gotValue = cylinderheadtypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "IgnitionSystemType": gotValue = ignitionsystemtypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "TransmissionMfrCode": gotValue = transmissionmfrcodeDict.TryGetValue(attribute.value, out niceValue); break;
                case "TransmissionBase": gotValue = transmissionbaseDict.TryGetValue(attribute.value, out niceValue); break;
                case "TransmissionType": gotValue = transmissiontypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "TransmissionControlType": gotValue = transmissioncontroltypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "TransElecControlled": gotValue = transmissioeleccontrolledDict.TryGetValue(attribute.value, out niceValue); break;
                case "TransmissionNumSpeeds": gotValue = transmissionnumspeedsDict.TryGetValue(attribute.value, out niceValue); break;
                case "TransmissionMfr": gotValue = mfrDict.TryGetValue(attribute.value, out niceValue); break;
                case "BedLength": gotValue = bedlengthDict.TryGetValue(attribute.value, out niceValue); break;
                case "BedType": gotValue = bedtypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "WheelBase": gotValue = wheelbaseDict.TryGetValue(attribute.value, out niceValue); break;
                case "BrakeSystem": gotValue = brakesystemDict.TryGetValue(attribute.value, out niceValue); break;
                case "Region": gotValue = regionDict.TryGetValue(attribute.value, out niceValue); break;
                case "FrontSpringType": gotValue = springtypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "RearSpringType": gotValue = springtypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "SteeringSystem": gotValue = steeringsystemDict.TryGetValue(attribute.value, out niceValue); break;
                case "SteeringType": gotValue = steeringtypeDict.TryGetValue(attribute.value, out niceValue); break;
                case "ValvesPerEngine": gotValue = valvesDict.TryGetValue(attribute.value, out niceValue); break;
                case "PowerOutput": gotValue = poweroutputDict.TryGetValue(attribute.value, out niceValue); break;
                default: gotValue = false; break;
            }

            if (gotValue)
            {
                switch (attribute.name)
                {
                    case "MfrBodyCode": returnValue = "Body code " + niceValue; break;
                    case "BodyNumDoors": returnValue = niceValue + " Door"; break;
                    case "EngineVIN": returnValue = "VIN:" + niceValue; break;
                    case "TransmissionMfrCode": returnValue = niceValue + " Transmission"; break;
                    case "TransmissionControlType": returnValue = niceValue + " Transmission"; break;
                    case "TransElecControlled": returnValue = " Electrically Controlled Transmission:" + niceValue; break;
                    case "TransmissionNumSpeeds": returnValue = niceValue + " Speed Transmission"; break;
                    case "TransmissionMfr": returnValue = niceValue + " Transmission"; break;
                    case "BedLength": returnValue = niceValue + " Inch Bed"; break;
                    case "BedType": returnValue = niceValue + " Bed"; break;
                    case "WheelBase": returnValue = niceValue + " Inch Wheelbase"; break;
                    case "BrakeSystem": returnValue = niceValue + " Brakes"; break;
                    case "FrontBrakeType": returnValue = "Front " + niceValue; break;
                    case "RearBrakeType": returnValue = "Rear " + niceValue; break;
                    case "FrontSpringType": returnValue = "Front " + niceValue + " Suspenssion"; break;
                    case "RearSpringType": returnValue = "Rear " + niceValue + " Suspenssion"; break;
                    case "SteeringSystem": returnValue = niceValue + " Steering"; break;
                    case "SteeringType": returnValue = niceValue + " Steering"; break;
                    case "ValvesPerEngine": returnValue = niceValue + " Valve"; break;
                    case "PowerOutput": returnValue = niceValue + " Horsepower"; break;
                    default: returnValue = niceValue; break;
                }
            }
            else
            {
                returnValue = "invalid (" + attribute.name + "=" + attribute.value + ")";

                if (attribute.name== "EngineBase" && deletedEngineBaseDict.ContainsKey(attribute.value))
                {// special case for handling unknown enginebase - look in the changelog-derived list of deleted ID's

                    string deletedEngineBlockType = ""; string deletedEngineCylinders = ""; string deletedEngineLiter = "";
                    foreach (KeyValuePair<string,string> myPair in  deletedEngineBaseDict[attribute.value])
                    {
                        if (myPair.Key == "BlockType") { deletedEngineBlockType = myPair.Value; }
                        if (myPair.Key == "Cylinders") { deletedEngineCylinders = myPair.Value; }
                        if (myPair.Key == "Liter") { deletedEngineLiter = myPair.Value; }
                    }

                    returnValue = "invalid (" + attribute.name + "=" + attribute.value + " ["+ deletedEngineBlockType.Trim() + deletedEngineCylinders.Trim() + " " + deletedEngineLiter.Trim() + "L])";


                }


            }
            return returnValue;
        }



        public bool validAttribute(VCdbAttribute attribute)
        {
            string niceValue = "";
            switch (attribute.name)
            {
                case "EngineBase": return enginebaseDict.TryGetValue(attribute.value, out niceValue);
                case "EngineBlock": return engineblockDict.TryGetValue(attribute.value, out niceValue);
                case "SubModel": return submodelDict.TryGetValue(attribute.value, out niceValue);
                case "DriveType": return drivetypeDict.TryGetValue(attribute.value, out niceValue);
                case "Aspiration": return aspirationDict.TryGetValue(attribute.value, out niceValue);
                case "FuelType": return fueltypeDict.TryGetValue(attribute.value, out niceValue);
                case "FrontBrakeType": return braketypeDict.TryGetValue(attribute.value, out niceValue);
                case "RearBrakeType": return braketypeDict.TryGetValue(attribute.value, out niceValue);
                case "BrakeABS": return brakeabsDict.TryGetValue(attribute.value, out niceValue);
                case "MfrBodyCode": return mfrbodycodeDict.TryGetValue(attribute.value, out niceValue);
                case "BodyNumDoors": return bodynumdoorsDict.TryGetValue(attribute.value, out niceValue);
                case "BodyType": return bodytypeDict.TryGetValue(attribute.value, out niceValue);
                case "EngineDesignation": return enginedesignationDict.TryGetValue(attribute.value, out niceValue);
                case "EngineVIN": return enginevinDict.TryGetValue(attribute.value, out niceValue);
                case "EngineVersion": return engineversionDict.TryGetValue(attribute.value, out niceValue);
                //case "EngineMfr": return enginebase.TryGetValue(attribute.value, out niceValue); // changed in 1.0.1.12 - Edgenet's devteam spotted this error
                case "EngineMfr": return mfrDict.TryGetValue(attribute.value, out niceValue);
                case "FuelDeliveryType": return fueldeliverytypeDict.TryGetValue(attribute.value, out niceValue);
                case "FuelDeliverySubType": return fueldeliverysubtypeDict.TryGetValue(attribute.value, out niceValue);
                case "FuelSystemControlType": return fuelsystemcontroltypeDict.TryGetValue(attribute.value, out niceValue);
                case "FuelSystemDesign": return fuelsystemdesignDict.TryGetValue(attribute.value, out niceValue);
                case "CylinderHeadType": return cylinderheadtypeDict.TryGetValue(attribute.value, out niceValue);
                case "IgnitionSystemType": return ignitionsystemtypeDict.TryGetValue(attribute.value, out niceValue);
                case "TransmissionMfrCode": return transmissionmfrcodeDict.TryGetValue(attribute.value, out niceValue);
                case "TransmissionBase": return transmissionbaseDict.TryGetValue(attribute.value, out niceValue);
                case "TransmissionType": return transmissiontypeDict.TryGetValue(attribute.value, out niceValue);
                case "TransmissionControlType": return transmissioncontroltypeDict.TryGetValue(attribute.value, out niceValue);
                case "TransElecControlled": return transmissioeleccontrolledDict.TryGetValue(attribute.value, out niceValue);
                case "TransmissionNumSpeeds": return transmissionnumspeedsDict.TryGetValue(attribute.value, out niceValue);
                case "TransmissionMfr": return mfrDict.TryGetValue(attribute.value, out niceValue);
                case "BedLength": return bedlengthDict.TryGetValue(attribute.value, out niceValue);
                case "BedType": return bedtypeDict.TryGetValue(attribute.value, out niceValue);
                case "WheelBase": return wheelbaseDict.TryGetValue(attribute.value, out niceValue);
                case "BrakeSystem": return brakesystemDict.TryGetValue(attribute.value, out niceValue);
                case "Region": return regionDict.TryGetValue(attribute.value, out niceValue);
                case "FrontSpringType": return springtypeDict.TryGetValue(attribute.value, out niceValue);
                case "RearSpringType": return springtypeDict.TryGetValue(attribute.value, out niceValue);
                case "SteeringSystem": return steeringsystemDict.TryGetValue(attribute.value, out niceValue);
                case "SteeringType": return steeringtypeDict.TryGetValue(attribute.value, out niceValue);
                case "ValvesPerEngine": return valvesDict.TryGetValue(attribute.value, out niceValue);
                case "PowerOutput": return poweroutputDict.TryGetValue(attribute.value, out niceValue);
                default: return false;
            }
        }


        public string niceMakeOfBasevid(int baseVid)
        {
            BaseVehicle basevidTemp = new BaseVehicle(); //basevidTemp.MakeName = "";
            if (vcdbBasevhicleDict.TryGetValue(baseVid, out basevidTemp))
            {
                return basevidTemp.MakeName;
            }
            else
            {
                return "not found";
            }
        }

        public string niceModelOfBasevid(int baseVid)
        {
            BaseVehicle basevidTemp = new BaseVehicle(); //basevidTemp.MakeName = "";
            if (vcdbBasevhicleDict.TryGetValue(baseVid, out basevidTemp))
            {
                return basevidTemp.ModelName;
            }
            else
            {
                return "not found";
            }
        }

        public string niceYearOfBasevid(int baseVid)
        {
            BaseVehicle basevidTemp = new BaseVehicle(); //basevidTemp.MakeName = "";
            if (vcdbBasevhicleDict.TryGetValue(baseVid, out basevidTemp))
            {
                return basevidTemp.YearName;
            }
            else
            {
                return "0";
            }
        }

        // return a list of basevids that exist inside a make/model/year-range (for exploding range-style apps out to individual basevid apps)
        public List<int> basevidsFromYearRange(int makeid, int modelid, int startYear, int endYear)
        {
            List<int> basevidList = new List<int>();

            if (modelid == 0)
            {// model was not specified - technically ok, but what the hell?!
                
                foreach (KeyValuePair<int, BaseVehicle> baseVehicleEntry in vcdbBasevhicleDict)
                {
                    if (baseVehicleEntry.Value.MakeId == makeid && baseVehicleEntry.Value.Year >= startYear && baseVehicleEntry.Value.Year <= endYear)
                    {
                        basevidList.Add(baseVehicleEntry.Key);
                    }
                }
            }
            else
            {// modelid was specified (non-0)
                string mmyKeyTemp = "";
                for (int yearID = startYear; yearID <= endYear; yearID++)
                {
                    mmyKeyTemp = makeid.ToString() + "_" + modelid.ToString() + "_" + yearID.ToString();
                    if (vcdbReverseBasevhicleDict.ContainsKey(mmyKeyTemp))
                    {
                        basevidList.Add(vcdbReverseBasevhicleDict[mmyKeyTemp]);
                    }
                    else
                    {// this MMY is not valid from given VCDB
                        basevidList.Add(0);
                    }
                }
            }
            return basevidList;
        }




        // return the list of VehicleID's that an app's specific attribute validates against.
        public List<int> findVehcileIDsForAppVCdbAttribute(App app, int attributeIndex)
        {
            List<int> result = new List<int>();
            if (app.VCdbAttributes.Count() == 0) { return result; }
            if(!vcdbBasevhicleDict.ContainsKey(app.basevehicleid)) { return result; } // invalid basevids will not be present in the dict
            int i = attributeIndex;

            switch (app.VCdbAttributes[i].name)
            {
                case "EngineBase":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.EngineBaseID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key); } }
                    }
                    break;

                //2022-10-08 Enginebase2 work ccc
                case "EngineBlock":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.EngineBlockID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key); } }
                    }
                    break;

                case "SubModel":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        if (vehicleEntry.Value.SubmodelID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key); }
                    }
                    break;

                case "DriveType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (int driveTypeId in vehicleEntry.Value.DriveTypeIDlist) { if (driveTypeId == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key); } }
                    }
                    break;

                case "Aspiration":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.AspirationID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "FuelType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.FuelTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "FrontBrakeType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbBrakeConfig brakeConfig in vehicleEntry.Value.BrakeConfigList) { if (brakeConfig.FrontBrakeTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "RearBrakeType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbBrakeConfig brakeConfig in vehicleEntry.Value.BrakeConfigList) { if (brakeConfig.RearBrakeTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "BrakeABS":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbBrakeConfig brakeConfig in vehicleEntry.Value.BrakeConfigList) { if (brakeConfig.BrakeABSID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "MfrBodyCode":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (int MfrBodyCodeId in vehicleEntry.Value.MfrBodyCodeIDlist) { if (MfrBodyCodeId == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "BodyNumDoors":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbBodyStyleConfig bodyStyleConfig in vehicleEntry.Value.BodyStyleConfigList) { if (bodyStyleConfig.BodyNumDoorsID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "BodyType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbBodyStyleConfig bodyStyleConfig in vehicleEntry.Value.BodyStyleConfigList) { if (bodyStyleConfig.BodyTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "EngineDesignation":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.EngineDesignationID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "EngineVIN":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.EngineVINID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "EngineVersion":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.EngineVersionID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "EngineMfr":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.EngineMfrID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "FuelDeliveryType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.FuelDeliveryTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "FuelDeliverySubType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.FuelDeliverySubTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "FuelSystemControlType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.FuelSystemControlTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "FuelSystemDesign":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.FuelSystemDesignID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "CylinderHeadType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.CylinderHeadTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "IgnitionSystemType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.IgnitionSystemTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "TransmissionMfrCode":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbTransmission transnmission in vehicleEntry.Value.TransmissionList) { if (transnmission.TransmissionMfrCodeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "TransmissionBase":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbTransmission transnmission in vehicleEntry.Value.TransmissionList) { if (transnmission.TransmissionBaseID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "TransmissionType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbTransmission transnmission in vehicleEntry.Value.TransmissionList) { if (transnmission.TransmissionTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "TransmissionControlType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbTransmission transnmission in vehicleEntry.Value.TransmissionList) { if (transnmission.TransmissionControlTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "TransElecControlled":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbTransmission transnmission in vehicleEntry.Value.TransmissionList) { if (transnmission.TransmissionElecControlledID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key); } }
                    }
                    break;

                case "TransmissionNumSpeeds":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbTransmission transnmission in vehicleEntry.Value.TransmissionList) { if (transnmission.TransmissionNumSpeedsID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "TransmissionMfr":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbTransmission transnmission in vehicleEntry.Value.TransmissionList) { if (transnmission.TransmissionMfrID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "BedLength":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbBedConfig bedConfig in vehicleEntry.Value.BedConfigList) { if (bedConfig.BedLengthID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "BedType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbBedConfig bedConfig in vehicleEntry.Value.BedConfigList) { if (bedConfig.BedTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "WheelBase":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (int wheelBaseId in vehicleEntry.Value.WheelBaseIDlist) { if (wheelBaseId == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "BrakeSystem":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbBrakeConfig brakeConfig in vehicleEntry.Value.BrakeConfigList) { if (brakeConfig.BrakeSystemID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "Region":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        if (vehicleEntry.Value.RegionID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  }
                    }
                    break;

                case "FrontSpringType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbSpringTypeConfig springTypeConfig in vehicleEntry.Value.SpringTypeConfigList) { if (springTypeConfig.FrontSpringTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "RearSpringType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbSpringTypeConfig springTypeConfig in vehicleEntry.Value.SpringTypeConfigList) { if (springTypeConfig.RearSpringTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "SteeringSystem":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbSteeringConfig steeringConfig in vehicleEntry.Value.SteeringConfigList) { if (steeringConfig.SteeringSystemID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "SteeringType":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbSteeringConfig steeringConfig in vehicleEntry.Value.SteeringConfigList) { if (steeringConfig.SteeringTypeID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "ValvesPerEngine":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.ValvesID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key);  } }
                    }
                    break;

                case "PowerOutput":
                    foreach (KeyValuePair<int, vcdbVehilce> vehicleEntry in vcdbBasevhicleDict[app.basevehicleid].vcdbVehicleDict)
                    {
                        foreach (vcdbEngineConfig engineConfig in vehicleEntry.Value.EngineConfigList) { if (engineConfig.PowerOutputID == app.VCdbAttributes[i].value) { result.Add(vehicleEntry.Key); } }
                    }
                    break;


                default: break;
            }

            return result;
        }


        // validate an individual app against VCdb configuration (vehicle,vehilcto..) that was imported into the purpose-built data structure for holding the iported VCdb config tables
        // this data could have come from either local access file or remote MySQL service
        // use first vcdb attribute in app to establish a reference VehicleID.
        // no vcdbVehicle in VehicleID-keyed dict contains ths attribute nave/value, this config is invalid
        // if the remainder of attributes all find hits in the same vehicel, config is valid
        public bool configIsValidMemoryBased(App app)
        {
            if (app.VCdbAttributes.Count == 0) { return true; }// apps with no attributes are inherently valid from a configuration standpoint

            List<int> vehicleIDlistinitial = new List<int>();
            List<int> vehicleIDlist = new List<int>();
            bool foundCommonConfig; bool configIsValid = true;

            if (app.VCdbAttributes.Count() == 1)
            {// single attribute

                vehicleIDlist = findVehcileIDsForAppVCdbAttribute(app, 0);
                if (vehicleIDlist.Count() == 0)
                {// this attribute is not found in any Vehicle config for this basevid
                    configIsValid = false;
                }
            }
            else
            {// multiple attributes

                vehicleIDlistinitial = findVehcileIDsForAppVCdbAttribute(app, 0);
                for (int i = 1; i < app.VCdbAttributes.Count(); i++)
                {
                    vehicleIDlist = findVehcileIDsForAppVCdbAttribute(app, i);

                    foundCommonConfig = false;
                    foreach (int vehicleID in vehicleIDlist)
                    {
                        if (vehicleIDlistinitial.Contains(vehicleID))
                        {
                            foundCommonConfig = true;
                            break;
                        }
                    }
                    if (!foundCommonConfig) { configIsValid = false; break; }
                }
            }
            return configIsValid;
        }

        // validate an individual app against VCdb configuration (vehicle,vehilcto..) by dynamically building the SQL calls to be sent to either Access engine (OLEDB) or remote MySQL database
        public bool configIsValidSQLbased(App app, int remoteVCdbConnectionIndex)
        {
            bool returnValue = false;
            string sqlString = "";
            if (app.VCdbAttributes.Count == 0) { return true; }// apps with no attributes are inherently valid from a configuration standpoint
            sqlString = configValidationSQLForApp(app);
            try
            {
                if (useRemoteDB)
                {
                    MySqlCommand command = new MySqlCommand(sqlString, connectionMySQLlist[remoteVCdbConnectionIndex]);
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read()) { returnValue = true; } // got at least one record back from config query - all we care about is this fact - not what the rec contains
                    reader.Close();
                }
                else
                {// local M$ Access file datasource mode
                    OleDbCommand command = new OleDbCommand(sqlString, connectionOLEDB);
                    OleDbDataReader reader = command.ExecuteReader();
                    if (reader.Read()) { returnValue = true; } // got at least one record back from config query - all we care about is this fact - not what the rec contains
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(sqlString);
            }
            return returnValue;
        }

        // Build the "from" and "where" sql tables and join clauses for vcdb validation query based on the attributes in the reference app.
        // The purpose is to tease out a list of attribute names for knowing which tables to validate against. You could simply validate every app against a monolithic "all-in-one" 
        // join of the entire vcdb - this is process-intensive (very slow). If we only include the tables in the join that the app referes-to, the query is faster and more memory effecient.
        public string configValidationSQLForApp(App app)
        {
            if (app.VCdbAttributes.Count == 0) { return ""; }

            string fromClause = "Vehicle,";
            string whereClause = "";
            List<int> vcdbSystems = new List<int>();
            int i;

            foreach (VCdbAttribute myAttribute in app.VCdbAttributes)
            {
                i = systemGroupOfAttribute(myAttribute);
                if (!vcdbSystems.Contains(i)) { vcdbSystems.Add(i); }
            }

            foreach (int vcdbSystem in vcdbSystems)
            {
                switch (vcdbSystem)
                {
                    case 0: break; // vehilce is the only table required for determining region or submodel. fromClause is initialized to "vheicle," already
                    case 1: fromClause += "VehicleToDriveType,"; whereClause += "Vehicle.VehicleID=VehicleToDriveType.VehicleID and "; break;
                    case 2: fromClause += "VehicleToBrakeConfig,BrakeConfig,"; whereClause += "Vehicle.VehicleID=VehicleToBrakeConfig.Vehicleid and VehicleToBrakeConfig.BrakeConfigID=BrakeConfig.BrakeConfigID and "; break;
                    case 3: fromClause += "VehicleToEngineConfig,EngineConfig,Valves,EngineBase,FuelDeliveryConfig,"; whereClause += "Vehicle.VehicleID = VehicleToEngineConfig.VehicleID and VehicleToEngineConfig.EngineConfigID = EngineConfig.EngineConfigID and EngineConfig.EngineBaseID=EngineBase.EngineBaseID and EngineConfig.ValvesID=Valves.ValvesID and EngineConfig.FuelDeliveryConfigID=FuelDeliveryConfig.FuelDeliveryConfigID and "; break;
                    case 4: fromClause += "VehicleToBodyStyleConfig,BodyStyleConfig,"; whereClause += "Vehicle.VehicleID=VehicleToBodyStyleConfig.VehicleID and VehicleToBodyStyleConfig.BodyStyleConfigID = BodyStyleConfig.BodyStyleConfigID and "; break;
                    case 5: fromClause += "VehicleToMfrBodyCode,"; whereClause += "Vehicle.VehicleID=VehicleToMfrBodyCode.VehicleID and "; break;
                    case 6: fromClause += "VehicleToTransmission,Transmission,TransmissionBase,"; whereClause += "Vehicle.VehicleID=VehicleToTransmission.VehicleID and VehicleToTransmission.Transmissionid=Transmission.TransmissionID and Transmission.TransmissionbaseID=TransmissionBase.TransmissionBaseID and "; break;
                    case 7: fromClause += "VehicleToWheelbase,"; whereClause += "Vehicle.VehicleID=VehicleToWheelbase.VehicleID and "; break;
                    case 8: fromClause += "VehicleToSteeringConfig,SteeringConfig,"; whereClause += "Vehicle.VehicleID=VehicleToSteeringConfig.VehicleID and VehicleToSteeringConfig.SteeringConfigID=SteeringConfig.SteeringConfigID and "; break;
                    case 9: fromClause += "VehicleToBedConfig,BedConfig,"; whereClause += "Vehicle.VehicleID=VehicleToBedConfig.VehicleID and VehicleToBedConfig.BedConfigID=BedConfig.BedconfigID and "; break;
                    case 10: fromClause += "VehicleToSpringTypeConfig,SpringTypeConfig,"; whereClause += "Vehicle.VehicleID=VehicleToSpringTypeConfig.VehicleID and VehicleToSpringTypeConfig.SpringTypeConfigID=SpringTypeConfig.SpringTypeConfigID and "; break;
                    case 11: fromClause += "BaseVehicle,Model,"; whereClause += "BaseVehicle.ModelID=Model.ModelID and "; break;
                    case 12: break;
                    default: break;
                }
            }

            if (fromClause.Length > 0) { fromClause = fromClause.Substring(0, fromClause.Length - 1); }

            foreach (VCdbAttribute myAttribute in app.VCdbAttributes)
            {
                whereClause += attributeWhereClause(myAttribute);
            }

            whereClause += "Vehicle.BasevehicleID = " + app.basevehicleid.ToString();

            return "select Vehicle.VehicleID from " + fromClause + " where " + whereClause + ";";
        }


        public string attributeWhereClause(VCdbAttribute myAttribute)
        {
            switch (myAttribute.name)
            {
                case "EngineBase": return "EngineBase.EngineBaseID = " + myAttribute.value + " and ";
                case "SubModel": return "SubmodelID = " + myAttribute.value + " and ";
                case "DriveType": return "DriveTypeID = " + myAttribute.value + " and ";
                case "Aspiration": return "AspirationID = " + myAttribute.value + " and ";
                case "FuelType": return "FuelTypeID = " + myAttribute.value + " and ";
                case "FrontBrakeType": return "frontbraketypeid = " + myAttribute.value + " and ";
                case "RearBrakeType": return "rearbraketypeid = " + myAttribute.value + " and ";
                case "BrakeABS": return "brakeabsid = " + myAttribute.value + " and ";
                case "MfrBodyCode": return "mfrbodycodeid = " + myAttribute.value + " and ";
                case "BodyNumDoors": return "bodynumdoorsid = " + myAttribute.value + " and ";
                case "BodyType": return "bodytypeid = " + myAttribute.value + " and ";
                case "EngineDesignation": return "EngineDesignationID = " + myAttribute.value + " and ";
                case "EngineVIN": return "EngineVINID = " + myAttribute.value + " and ";
                case "EngineVersion": return "EngineVersionID = " + myAttribute.value + " and ";
                case "EngineMfr": return "EngineMfrID = " + myAttribute.value + " and ";
                case "FuelDeliveryType": return "FuelDeliveryTypeID = " + myAttribute.value + " and ";
                case "FuelDeliverySubType": return "FuelDeliverySubTypeID = " + myAttribute.value + " and ";
                case "FuelSystemControlType": return "FuelSystemControlTypeID = " + myAttribute.value + " and ";
                case "FuelSystemDesign": return "FuelSystemDesignID = " + myAttribute.value + " and ";
                case "CylinderHeadType": return "CylinderHeadTypeID = " + myAttribute.value + " and ";
                case "IgnitionSystemType": return "IgnitionSystemTypeID = " + myAttribute.value + " and ";
                case "TransmissionMfrCode": return "TransmissionMfrCodeID = " + myAttribute.value + " and ";
                case "TransmissionBase": return "TransmissionBase.TransmissionBaseID = " + myAttribute.value + " and ";
                case "TransmissionType": return "TransmissionTypeID = " + myAttribute.value + " and ";
                case "TransmissionControlType": return "TransmissionControlTypeID = " + myAttribute.value + " and ";
                case "TransElecControlled": return "TransElecControlledID = " + myAttribute.value + " and ";
                case "TransmissionNumSpeeds": return "TransmissionNumSpeedsID = " + myAttribute.value + " and ";
                case "TransmissionMfr": return "TransmissionMfrID = " + myAttribute.value + " and ";
                case "BedLength": return "BedLengthID = " + myAttribute.value + " and ";
                case "BedType": return "BedTypeID = " + myAttribute.value + " and ";
                case "WheelBase": return "WheelBaseID = " + myAttribute.value + " and ";
                case "BrakeSystem": return "BrakeSystemID = " + myAttribute.value + " and ";
                case "Region": return "RegionID = " + myAttribute.value + " and ";
                case "FrontSpringType": return "FrontSpringTypeID = " + myAttribute.value + " and ";
                case "RearSpringType": return "RearSpringTypeID = " + myAttribute.value + " and ";
                case "SteeringSystem": return "SteeringSystemID = " + myAttribute.value + " and ";
                case "SteeringType": return "SteeringTypeID = " + myAttribute.value + " and ";
                case "ValvesPerEngine": return "EngineConfig.ValvesID = " + myAttribute.value + " and ";
                case "PowerOutput": return "EngineConfig.PowerOutputID = " + myAttribute.value + " and ";
                default: return "";
            }
        }


        // determine which system group an attribute is in. this is for determining what tables to join in a validation query for the sake of effeciency
        public int systemGroupOfAttribute(VCdbAttribute myAttribute)
        {
            if (myAttribute.name == "Region") { return 0; }
            if (myAttribute.name == "SubModel") { return 0; }

            if (myAttribute.name == "DriveType") { return 1; }

            if (myAttribute.name == "BrakeABS") { return 2; }
            if (myAttribute.name == "BrakeSystem") { return 2; }
            if (myAttribute.name == "FrontBrakeType") { return 2; }
            if (myAttribute.name == "RearBrakeType") { return 2; }

            if (myAttribute.name == "EngineBase") { return 3; }
            if (myAttribute.name == "EngineVIN") { return 3; }
            if (myAttribute.name == "EngineVersion") { return 3; }
            if (myAttribute.name == "EngineMfr") { return 3; }
            if (myAttribute.name == "EngineDesignation") { return 3; }
            if (myAttribute.name == "FuelDeliverySubType") { return 3; }
            if (myAttribute.name == "FuelDeliveryType") { return 3; }
            if (myAttribute.name == "FuelSystemControlType") { return 3; }
            if (myAttribute.name == "FuelSystemDesign") { return 3; }
            if (myAttribute.name == "Aspiration") { return 3; }
            if (myAttribute.name == "IgnitionSystemType") { return 3; }
            if (myAttribute.name == "ValvesPerEngine") { return 3; }
            if (myAttribute.name == "CylinderHeadType") { return 3; }
            if (myAttribute.name == "FuelType") { return 3; }
            if (myAttribute.name == "PowerOutput") { return 3; } // is this deprecated in VCdb recently?

            if (myAttribute.name == "BodyNumDoors") { return 4; }
            if (myAttribute.name == "BodyType") { return 4; }

            if (myAttribute.name == "MfrBodyCode") { return 5; }

            if (myAttribute.name == "TransElecControlled") { return 6; } // is this depricated in VCdb recently?
            if (myAttribute.name == "TransmissionBase") { return 6; }
            if (myAttribute.name == "TransmissionControlType") { return 6; }
            if (myAttribute.name == "TransmissionMfr") { return 6; }
            if (myAttribute.name == "TransmissionMfrCode") { return 6; }
            if (myAttribute.name == "TransmissionNumSpeeds") { return 6; }
            if (myAttribute.name == "TransmissionType") { return 6; }

            if (myAttribute.name == "WheelBase") { return 7; }

            if (myAttribute.name == "SteeringSystem") { return 8; }
            if (myAttribute.name == "SteeringType") { return 8; }

            if (myAttribute.name == "BedLength") { return 9; }
            if (myAttribute.name == "BedType") { return 9; }

            if (myAttribute.name == "FrontSpringType") { return 10; }
            if (myAttribute.name == "RearSpringType") { return 10; }

            if (myAttribute.name == "VehicleType") { return 11; }
            return 12;
        }

        public bool attributeHasWildcardInConfig(int baseVehicleid, string attributeName)
        {
            App app = new App();
            VCdbAttribute myVCdbAttribute = new VCdbAttribute();
            List<int> wildcardIDs = new List<int>();

            switch (attributeName)
            {
                case "DriveType": wildcardIDs.Add(4); break; 
                case "BrakeABS": wildcardIDs.Add(4); wildcardIDs.Add(9); break;
                case "BrakeSystem": wildcardIDs.Add(4); break;
                case "FrontBrakeType": wildcardIDs.Add(4); break;
                case "RearBrakeType": wildcardIDs.Add(4); break;
                case "EngineVersion": wildcardIDs.Add(50); wildcardIDs.Add(2); wildcardIDs.Add(3); break;
                case "EngineMfr": wildcardIDs.Add(4); wildcardIDs.Add(2); break; //2=N/A, 3=N/R
                case "EngineDesignation": wildcardIDs.Add(1); break; //1="-" which we will call a wildcard
                case "EngineVIN": wildcardIDs.Add(1); break;//1="-" which we will call a wildcard
                case "FuelDeliverySubType": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A, 3=N/R
                case "FuelSystemControlType": wildcardIDs.Add(4); wildcardIDs.Add(2);break;//2=N/A, 3=N/R
                case "FuelSystemDesign": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A, 3=N/R
                case "IgnitionSystemType": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A, 3=N/R
                case "ValvesPerEngine": wildcardIDs.Add(16); wildcardIDs.Add(25); break;//17=N/R, 25=N/A
                case "PowerOutput": wildcardIDs.Add(1); wildcardIDs.Add(2); wildcardIDs.Add(2); break;//1=U/K, 2=N/A, 3=N/R
                case "CylinderHeadType": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A, 3=N/R
                case "FuelType": wildcardIDs.Add(18); wildcardIDs.Add(20); break; //20=N/A
                case "BodyNumDoors": wildcardIDs.Add(4); break;
                case "BodyType": wildcardIDs.Add(40); break;
                case "MfrBodyCode": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A, 3=N/R
                case "TransmissionControlType": wildcardIDs.Add(4);break;//3=N/R
                case "TransElecControlled": wildcardIDs.Add(3); wildcardIDs.Add(4); wildcardIDs.Add(5); break;//3=N/A, 4=N/R, 5=U/K
                case "TransmissionMfr": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A, 3=N/R
                case "TransmissionMfrCode": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A, 3=N/R
                case "TransmissionNumSpeeds": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A, 3=N/R
                case "TransmissionType": wildcardIDs.Add(4); wildcardIDs.Add(7); break;//7=N/A, 3=N/R
                case "WheelBase": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A
                case "SteeringSystem": wildcardIDs.Add(4); wildcardIDs.Add(2);break;//2=N/A
                case "SteeringType": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A
                case "BedLength": wildcardIDs.Add(2); wildcardIDs.Add(3); wildcardIDs.Add(42); break;//2=N/A, 3=N/R, 42=U/K
                case "BedType": wildcardIDs.Add(2); wildcardIDs.Add(3); wildcardIDs.Add(14); break;//2=N/A, 3=N/R, 14=U/K
                case "FrontSpringType": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A
                case "RearSpringType": wildcardIDs.Add(4); wildcardIDs.Add(2); break;//2=N/A
                default: break;
            }
            if (wildcardIDs.Count()==0) { return false; }

            myVCdbAttribute.name = attributeName;
            app.basevehicleid = baseVehicleid;
            app.VCdbAttributes.Add(myVCdbAttribute);

            foreach (int idValue in wildcardIDs)
            {// this attribute name (ie: EngineVersion) has "N/A", "N/R", "U/K", "-" in the configuration 
                myVCdbAttribute.value = idValue;
                if (configIsValidMemoryBased(app)) { return true;}
            }
            return false;
        }


        public string importOLEDBdata()
        {
            importSuccess = false;
           
            try
            {
                int i;
                OleDbCommand command = new OleDbCommand("SELECT BaseVehicle.BaseVehicleId,Make.MakeName,Model.ModelName,BaseVehicle.YearId,VehicleType.VehicleTypeName,Make.MakeId,Model.ModelId,VehicleType.VehicleTypeId FROM BaseVehicle,Make,Model,VehicleType where BaseVehicle.MakeId=Make.MakeId and BaseVehicle.ModelId=Model.ModelId and Model.VehicleTypeId=VehicleType.VehicleTypeId order by MakeName,ModelName,YearId;");
                command.Connection = connectionOLEDB;
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    i = Convert.ToInt32(reader.GetValue(0).ToString());
                    BaseVehicle basevidTemp = new BaseVehicle();
                    basevidTemp.MakeName = reader.GetValue(1).ToString();
                    basevidTemp.ModelName = reader.GetValue(2).ToString();
                    basevidTemp.YearName = reader.GetValue(3).ToString();
                    basevidTemp.Year = Convert.ToInt32(reader.GetValue(3).ToString());
                    basevidTemp.VehicleTypeName = reader.GetValue(4).ToString();
                    basevidTemp.MakeId = Convert.ToInt32(reader.GetValue(5).ToString());
                    basevidTemp.ModelId = Convert.ToInt32(reader.GetValue(6).ToString());
                    basevidTemp.VehicleTypeId = Convert.ToInt32(reader.GetValue(7).ToString());
                    vcdbBasevhicleDict.Add(i, basevidTemp);
                }
                reader.Close();
                importProgress = 5;

                // populuate basevehicle lookup dict [makeid_modelid_yearid]=>basevidid  
                string mmyKeyTemp = "";
                command.CommandText = "select Make.MakeID, Model.ModelID, YearID, BaseVehicleID from Make,Model,BaseVehicle where Make.MakeID=BaseVehicle.MakeID and Model.ModelID =BaseVehicle.ModelID;"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    mmyKeyTemp = reader.GetValue(0).ToString() + "_" + reader.GetValue(1).ToString() + "_" + reader.GetValue(2).ToString();
                    if (!vcdbReverseBasevhicleDict.ContainsKey(mmyKeyTemp))
                    {
                        vcdbReverseBasevhicleDict.Add(mmyKeyTemp, Convert.ToInt32(reader.GetValue(3).ToString()));
                    }
                }
                reader.Close();





                // instance and populuate all the "Vehicle" table stuff (VehicleID,BaseVehicleID,SubmodelID,RegionID,PublicationStageID) and put it in the basevid-keyed dict
                int basevehicleidTemp, vehicleidTemp;
                command.CommandText = "select VehicleID,BaseVehicleID,SubmodelID,RegionID,PublicationStageID from Vehicle order by BaseVehicleID,VehicleID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());

                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp))
                    {
                        if (!vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                        {// this vcdbBasevhicleDict entry does not contain a vcdbVehicleDict entry for VehicleID in this query result record
                            vcdbVehilce vcdbVehicleTemp = new vcdbVehilce();
                            vcdbVehicleTemp.SubmodelID = Convert.ToInt32(reader.GetValue(2).ToString());
                            vcdbVehicleTemp.RegionID = Convert.ToInt32(reader.GetValue(3).ToString());
                            vcdbVehicleTemp.PublicationStageID = Convert.ToInt32(reader.GetValue(4).ToString());
                            vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.Add(vehicleidTemp, vcdbVehicleTemp);
                        }
                    }
                }
                reader.Close();


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, DriveType.DriveTypeID from Vehicle, VehicleToDriveType, DriveType where Vehicle.VehicleID= VehicleToDriveType.VehicleID and VehicleToDriveType.DriveTypeID = DriveType.DriveTypeID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].DriveTypeIDlist.Add(Convert.ToInt32(reader.GetValue(2).ToString()));
                    }
                }
                reader.Close();


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, WheelBase.WheelBaseID from Vehicle, VehicleToWheelbase, WheelBase where Vehicle.VehicleID= VehicleToWheelbase.VehicleID and VehicleToWheelbase.WheelbaseID = WheelBase.WheelBaseID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());

                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].WheelBaseIDlist.Add(Convert.ToInt32(reader.GetValue(2).ToString()));
                    }
                }
                reader.Close();
                importProgress = 20;

                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, MfrBodyCode.MfrBodyCodeID from Vehicle, VehicleToMfrBodyCode, MfrBodyCode where Vehicle.VehicleID= VehicleToMfrBodyCode.VehicleID and VehicleToMfrBodyCode.MfrBodyCodeID =MfrBodyCode.MfrBodyCodeID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());

                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].MfrBodyCodeIDlist.Add(Convert.ToInt32(reader.GetValue(2).ToString()));
                    }
                }
                reader.Close();

                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, BedLengthID, BedTypeID from Vehicle, VehicleToBedConfig, BedConfig where Vehicle.VehicleID = VehicleToBedConfig.VehicleID and VehicleToBedConfig.BedConfigID = BedConfig.BedConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());

                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBedConfig vcdbBedConfigTemp = new vcdbBedConfig();
                        vcdbBedConfigTemp.BedLengthID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbBedConfigTemp.BedTypeID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].BedConfigList.Add(vcdbBedConfigTemp);
                    }
                }
                reader.Close();


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID,  BodyTypeID, BodyNumDoorsID from Vehicle, VehicleToBodyStyleConfig, BodyStyleConfig where Vehicle.VehicleID = VehicleToBodyStyleConfig.VehicleID and VehicleToBodyStyleConfig.BodyStyleConfigID = BodyStyleConfig.BodyStyleConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBodyStyleConfig vcdbBodyStyleConfigTemp = new vcdbBodyStyleConfig();
                        vcdbBodyStyleConfigTemp.BodyTypeID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbBodyStyleConfigTemp.BodyNumDoorsID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].BodyStyleConfigList.Add(vcdbBodyStyleConfigTemp);
                    }
                }
                reader.Close();


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, FrontBrakeTypeID, RearBrakeTypeID, BrakeSystemID, BrakeABSID from Vehicle, VehicleToBrakeConfig, BrakeConfig where Vehicle.VehicleID = VehicleToBrakeConfig.VehicleID and VehicleToBrakeConfig.BrakeConfigID = BrakeConfig.BrakeConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBrakeConfig vcdbBrakeConfigTemp = new vcdbBrakeConfig();
                        vcdbBrakeConfigTemp.FrontBrakeTypeID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbBrakeConfigTemp.RearBrakeTypeID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBrakeConfigTemp.BrakeSystemID = Convert.ToInt32(reader.GetValue(4).ToString());
                        vcdbBrakeConfigTemp.BrakeABSID = Convert.ToInt32(reader.GetValue(5).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].BrakeConfigList.Add(vcdbBrakeConfigTemp);
                    }
                }
                reader.Close();
                importProgress = 40;


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, FrontSpringTypeID, RearSpringTypeID from Vehicle, VehicleToSpringTypeConfig, SpringTypeConfig where Vehicle.VehicleID = VehicleToSpringTypeConfig.VehicleID and VehicleToSpringTypeConfig.SpringTypeConfigID = SpringTypeConfig.SpringTypeConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbSpringTypeConfig vcdbSpringTypeConfigTemp = new vcdbSpringTypeConfig();
                        vcdbSpringTypeConfigTemp.FrontSpringTypeID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbSpringTypeConfigTemp.RearSpringTypeID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].SpringTypeConfigList.Add(vcdbSpringTypeConfigTemp);
                    }
                }
                reader.Close();


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, SteeringTypeID, SteeringSystemID from Vehicle, VehicleToSteeringConfig, SteeringConfig where Vehicle.VehicleID = VehicleToSteeringConfig.VehicleID and VehicleToSteeringConfig.SteeringConfigID = SteeringConfig.SteeringConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbSteeringConfig vcdbSteeringConfigTemp = new vcdbSteeringConfig();
                        vcdbSteeringConfigTemp.SteeringTypeID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbSteeringConfigTemp.SteeringSystemID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].SteeringConfigList.Add(vcdbSteeringConfigTemp);
                    }
                }
                reader.Close();

                int LPS20221004 = 0;
                // if you wanted to limit the vcdb to type2 (pass car and light truck) vehicles:
                //command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, EngineConfig.EngineBaseID, EngineDesignationID, EngineVINID, ValvesID, AspirationID, CylinderHeadTypeID, FuelTypeID, IgnitionSystemTypeID, EngineMfrID, EngineVersionID, PowerOutputID, FuelDeliveryTypeID, FuelDeliverySubTypeID, FuelSystemControlTypeID, FuelSystemDesignID from BaseVehicle, Model, VehicleType, Vehicle, VehicleToEngineConfig, EngineConfig, EngineBase, FuelDeliveryConfig where Vehicle.BaseVehicleID=BaseVehicle.BaseVehicleID and BaseVehicle.ModelID=Model.ModelID and Model.VehicleTypeID=VehicleType.VehicleTypeID and Vehicle.VehicleID = VehicleToEngineConfig.VehicleID and VehicleToEngineConfig.EngineConfigID = EngineConfig.EngineConfigID and EngineConfig.EngineBaseID = EngineBase.EngineBaseID and EngineConfig.FuelDeliveryConfigID=FuelDeliveryConfig.FuelDeliveryConfigID and VehicleType.VehicleTypeGroupID=2";



                // classic (before engineBlock was a thing) command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, EngineConfig.EngineBaseID, EngineDesignationID, EngineVINID, ValvesID, AspirationID, CylinderHeadTypeID, FuelTypeID, IgnitionSystemTypeID, EngineMfrID, EngineVersionID, PowerOutputID, FuelDeliveryTypeID, FuelDeliverySubTypeID, FuelSystemControlTypeID, FuelSystemDesignID from Vehicle, VehicleToEngineConfig, EngineConfig, EngineBase, FuelDeliveryConfig where Vehicle.VehicleID = VehicleToEngineConfig.VehicleID and VehicleToEngineConfig.EngineConfigID = EngineConfig.EngineConfigID and EngineConfig.EngineBaseID = EngineBase.EngineBaseID and EngineConfig.FuelDeliveryConfigID=FuelDeliveryConfig.FuelDeliveryConfigID"; reader = command.ExecuteReader();
                command.CommandText = "SELECT Vehicle.BaseVehicleID, Vehicle.VehicleID, EngineConfig2.EngineBaseID, EngineConfig2.EngineDesignationID, EngineConfig2.EngineVINID, EngineConfig2.ValvesID, EngineConfig2.AspirationID, EngineConfig2.CylinderHeadTypeID, EngineConfig2.FuelTypeID, EngineConfig2.IgnitionSystemTypeID, EngineConfig2.EngineMfrID, EngineConfig2.EngineVersionID, EngineConfig2.PowerOutputID, FuelDeliveryConfig.FuelDeliveryTypeID, FuelDeliveryConfig.FuelDeliverySubTypeID, FuelDeliveryConfig.FuelSystemControlTypeID, FuelDeliveryConfig.FuelSystemDesignID, EngineBlock.EngineBlockID FROM Vehicle INNER JOIN (FuelDeliveryConfig INNER JOIN ((EngineBlock INNER JOIN (EngineBase2 INNER JOIN (EngineBase INNER JOIN EngineConfig2 ON EngineBase.EngineBaseID = EngineConfig2.EngineBaseID) ON EngineBase2.EngineBaseID = EngineConfig2.EngineBaseID) ON EngineBlock.EngineBlockID = EngineBase2.EngineBlockID) INNER JOIN VehicleToEngineConfig ON EngineConfig2.EngineConfigID = VehicleToEngineConfig.EngineConfigID) ON FuelDeliveryConfig.FuelDeliveryConfigID = EngineConfig2.FuelDeliveryConfigID) ON Vehicle.VehicleID = VehicleToEngineConfig.VehicleID WHERE (((Vehicle.VehicleID)=[VehicleToEngineConfig].[VehicleID]) AND ((VehicleToEngineConfig.EngineConfigID)=[EngineConfig2].[EngineConfigID]) AND ((EngineConfig2.EngineBaseID)=[EngineBase].[EngineBaseID]) AND ((EngineConfig2.FuelDeliveryConfigID)=[FuelDeliveryConfig].[FuelDeliveryConfigID]));"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    LPS20221004++;
                       basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbEngineConfig vcdbEngineConfigTemp = new vcdbEngineConfig();
                        vcdbEngineConfigTemp.EngineBaseID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbEngineConfigTemp.EngineDesignationID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbEngineConfigTemp.EngineVINID = Convert.ToInt32(reader.GetValue(4).ToString());
                        vcdbEngineConfigTemp.ValvesID = Convert.ToInt32(reader.GetValue(5).ToString());
                        vcdbEngineConfigTemp.AspirationID = Convert.ToInt32(reader.GetValue(6).ToString());
                        vcdbEngineConfigTemp.CylinderHeadTypeID = Convert.ToInt32(reader.GetValue(7).ToString());
                        vcdbEngineConfigTemp.FuelTypeID = Convert.ToInt32(reader.GetValue(8).ToString());
                        vcdbEngineConfigTemp.IgnitionSystemTypeID = Convert.ToInt32(reader.GetValue(9).ToString());
                        vcdbEngineConfigTemp.EngineMfrID = Convert.ToInt32(reader.GetValue(10).ToString());
                        vcdbEngineConfigTemp.EngineVersionID = Convert.ToInt32(reader.GetValue(11).ToString());
                        vcdbEngineConfigTemp.PowerOutputID = Convert.ToInt32(reader.GetValue(12).ToString());
                        vcdbEngineConfigTemp.FuelDeliveryTypeID = Convert.ToInt32(reader.GetValue(13).ToString());
                        vcdbEngineConfigTemp.FuelDeliverySubTypeID = Convert.ToInt32(reader.GetValue(14).ToString());
                        vcdbEngineConfigTemp.FuelSystemControlTypeID = Convert.ToInt32(reader.GetValue(15).ToString());
                        vcdbEngineConfigTemp.FuelSystemDesignID = Convert.ToInt32(reader.GetValue(16).ToString());
                        vcdbEngineConfigTemp.EngineBlockID = Convert.ToInt32(reader.GetValue(17).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].EngineConfigList.Add(vcdbEngineConfigTemp);
                    }
                }
                reader.Close();
                importProgress = 70;


                // if you wanted to limit the vcdb to type2 (pass car and light truck) vehicles:
                //command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, Transmission.TransmissionBaseID, TransmissionTypeID, TransmissionNumSpeedsID, TransmissionControlTypeID, TransmissionMfrCodeID, TransmissionElecControlledID, TransmissionMfrID from BaseVehicle, Model, VehicleType, Vehicle, VehicleToTransmission, Transmission, TransmissionBase where Vehicle.BaseVehicleID=BaseVehicle.BaseVehicleID and BaseVehicle.ModelID=Model.ModelID and Model.VehicleTypeID=VehicleType.VehicleTypeID and Vehicle.VehicleID = VehicleToTransmission.VehicleID and VehicleToTransmission.TransmissionID = Transmission.TransmissionID and Transmission.TransmissionBaseID = TransmissionBase.TransmissionBaseID and VehicleType.VehicleTypeGroupID=2";
                command.CommandText = "select BaseVehicleID, Vehicle.VehicleID, Transmission.TransmissionBaseID, TransmissionTypeID, TransmissionNumSpeedsID, TransmissionControlTypeID, TransmissionMfrCodeID, TransmissionElecControlledID, TransmissionMfrID from Vehicle, VehicleToTransmission, Transmission, TransmissionBase where Vehicle.VehicleID = VehicleToTransmission.VehicleID and VehicleToTransmission.TransmissionID = Transmission.TransmissionID and Transmission.TransmissionBaseID = TransmissionBase.TransmissionBaseID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbTransmission vcdbTransmissionTemp = new vcdbTransmission();
                        vcdbTransmissionTemp.TransmissionBaseID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbTransmissionTemp.TransmissionTypeID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbTransmissionTemp.TransmissionNumSpeedsID = Convert.ToInt32(reader.GetValue(4).ToString());
                        vcdbTransmissionTemp.TransmissionControlTypeID = Convert.ToInt32(reader.GetValue(5).ToString());
                        vcdbTransmissionTemp.TransmissionMfrCodeID = Convert.ToInt32(reader.GetValue(6).ToString());
                        vcdbTransmissionTemp.TransmissionElecControlledID = Convert.ToInt32(reader.GetValue(7).ToString());
                        vcdbTransmissionTemp.TransmissionMfrID = Convert.ToInt32(reader.GetValue(8).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].TransmissionList.Add(vcdbTransmissionTemp);
                    }
                }
                reader.Close();
                importProgress = 85;

                command.CommandText = "SELECT versiondate from version;"; reader = command.ExecuteReader();
                while (reader.Read()) { version = reader.GetValue(0).ToString(); }
                reader.Close(); 

                DateTime dt = new DateTime();
                if (DateTime.TryParseExact(version, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) { version = dt.ToString("yyyy-MM-dd"); }
                reader.Close(); 

                command.CommandText = "SELECT enginebaseid,liter,cc,cid,cylinders,blocktype from enginebase;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); enginebaseDict.Add(i, reader.GetValue(5).ToString().Trim() + reader.GetValue(4).ToString().Trim() + " " + reader.GetValue(1).ToString().Trim() + "L (EngineBase)"); }
                reader.Close();

                // 2022-10-04 enginebase2 work ccc
                command.CommandText = "SELECT engineblockid,liter,cc,cid,cylinders,blocktype from engineblock;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); engineblockDict.Add(i, reader.GetValue(5).ToString().Trim() + reader.GetValue(4).ToString().Trim() + " " + reader.GetValue(1).ToString().Trim() + "L (EngineBlock)"); }
                reader.Close();

                command.CommandText = "SELECT submodelid,submodelname from submodel"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); submodelDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT drivetypeid,drivetypename from drivetype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); drivetypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 


                command.CommandText = "SELECT aspirationid,aspirationname from aspiration;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); aspirationDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT fueltypeid,fueltypename from fueltype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fueltypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT braketypeid,braketypename from braketype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); braketypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT brakeabsid,brakeabsname from brakeabs;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); brakeabsDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT mfrbodycodeid,mfrbodycodename from mfrbodycode;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); mfrbodycodeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT bodynumdoorsid,bodynumdoors from bodynumdoors;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); bodynumdoorsDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT bodytypeid,bodytypename from bodytype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); bodytypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT enginedesignationid,enginedesignationname from enginedesignation;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); enginedesignationDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT enginevinid,enginevinname from enginevin;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); enginevinDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT engineversionid,engineversion from engineversion;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); engineversionDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT mfrid,mfrname from mfr;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); mfrDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT fueldeliverytypeid,fueldeliverytypename from fueldeliverytype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fueldeliverytypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT fueldeliverysubtypeid,fueldeliverysubtypename from fueldeliverysubtype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fueldeliverysubtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT fuelsystemcontroltypeid,fuelsystemcontroltypename from fuelsystemcontroltype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fuelsystemcontroltypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT fuelsystemdesignid,fuelsystemdesignname from fuelsystemdesign;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fuelsystemdesignDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT cylinderheadtypeid,cylinderheadtypename from cylinderheadtype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); cylinderheadtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT ignitionsystemtypeid,ignitionsystemtypename from ignitionsystemtype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); ignitionsystemtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT transmissionmfrcodeid,transmissionmfrcode from transmissionmfrcode;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissionmfrcodeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT transmissionbase.transmissionbaseid,transmissioncontroltypename, transmissiontypename, transmissionnumspeeds from transmissionbase, transmissiontype, transmissionnumspeeds, transmissioncontroltype WHERE transmissionbase.transmissiontypeid = transmissiontype.transmissiontypeid AND transmissionbase.transmissionnumspeedsid = transmissionnumspeeds.transmissionnumspeedsid AND transmissionbase.transmissioncontroltypeid = transmissioncontroltype.transmissioncontroltypeid;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissionbaseDict.Add(i, reader.GetValue(1).ToString().Trim() + " " + reader.GetValue(2).ToString().Trim() + " Speed " + reader.GetValue(3).ToString().Trim()); }
                reader.Close(); 

                command.CommandText = "SELECT transmissiontypeid,transmissiontypename from transmissiontype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissiontypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT transmissioncontroltypeid,transmissioncontroltypename from transmissioncontroltype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissioncontroltypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();

                command.CommandText = "select ElecControlledID,ElecControlled from  ElecControlled;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissioeleccontrolledDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();

                command.CommandText = "SELECT transmissionnumspeedsid,transmissionnumspeeds from transmissionnumspeeds;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissionnumspeedsDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); 

                command.CommandText = "SELECT bedlengthid,bedlength from bedlength;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); bedlengthDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 90;

                command.CommandText = "SELECT bedtypeid,bedtypename from bedtype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); bedtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 91;

                command.CommandText = "SELECT wheelbaseid,wheelbase from wheelbase;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); wheelbaseDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 92;

                command.CommandText = "SELECT brakesystemid,brakesystemname from brakesystem;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); brakesystemDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 93;

                command.CommandText = "SELECT regionid,regionname from region;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); regionDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 94;

                command.CommandText = "SELECT springtypeid,springtypename from springtype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); springtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 95;


                command.CommandText = "SELECT steeringsystemid,steeringsystemname from steeringsystem;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); steeringsystemDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 96;


                command.CommandText = "SELECT steeringtypeid,steeringtypename from steeringtype;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); steeringtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 97;


                command.CommandText = "SELECT valvesid,valvesperengine from valves;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); valvesDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 98;


                command.CommandText = "select PowerOutputID,HorsePower from PowerOutput;"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); poweroutputDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close(); importProgress = 100;

                importSuccess = true;

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                importExceptionMessage = ex.Message;
                importSuccess = false;
                return ex.Message;
            }
            return "";
        }


        public string importMySQLdata()
        {
            importSuccess = false;
            try
            {
                int i;

                MySqlCommand command = new MySqlCommand("SELECT BaseVehicle.BaseVehicleId,Make.MakeName,Model.ModelName,BaseVehicle.YearId,VehicleType.VehicleTypeName FROM BaseVehicle,Make,Model,VehicleType where BaseVehicle.MakeId=Make.MakeId and BaseVehicle.ModelId=Model.ModelId and Model.VehicleTypeId=VehicleType.VehicleTypeId order by MakeName,ModelName,YearId;", connectionMySQLlist.First());
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    i = Convert.ToInt32(reader.GetValue(0).ToString());
                    BaseVehicle basevidTemp = new BaseVehicle();
                    basevidTemp.MakeName = reader.GetValue(1).ToString();
                    basevidTemp.ModelName = reader.GetValue(2).ToString();
                    basevidTemp.YearName = reader.GetValue(3).ToString();
                    basevidTemp.VehicleTypeName = reader.GetValue(4).ToString();
                    vcdbBasevhicleDict.Add(i, basevidTemp);
                }
                reader.Close();
                importProgress = 3;

                // populuate basevehicle reverse lookup dict [makeid_modelid_yearid]=>basevidid  
                string mmyKeyTemp = "";
                command.CommandText = "select Make.MakeID, Model.ModelID, YearID, BaseVehicleID from Make,Model,BaseVehicle where Make.MakeID=BaseVehicle.MakeID and Model.ModelID =BaseVehicle.ModelID;"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    mmyKeyTemp = reader.GetValue(0).ToString() + "_" + reader.GetValue(1).ToString() + "_" + reader.GetValue(2).ToString();
                    if (!vcdbReverseBasevhicleDict.ContainsKey(mmyKeyTemp))
                    {
                        vcdbReverseBasevhicleDict.Add(mmyKeyTemp, Convert.ToInt32(reader.GetValue(3).ToString()));
                    }
                }
                reader.Close();
                importProgress = 6;

                // instance and populeate all the "Vehicle" table stuff (VehicleID,BaseVehicleID,SubmodelID,RegionID,PublicationStageID) and put it in the basevid-keyed dict
                int basevehicleidTemp, vehicleidTemp;
                command.CommandText = "select VehicleID,BaseVehicleID,SubmodelID,RegionID,PublicationStageID from Vehicle order by BaseVehicleID,VehicleID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());

                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp))
                    {
                        if (!vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                        {// this vcdbBasevhicleDict entry does not contain a vcdbVehicleDict entry for VehicleID in this query result record
                            vcdbVehilce vcdbVehicleTemp = new vcdbVehilce();
                            vcdbVehicleTemp.SubmodelID = Convert.ToInt32(reader.GetValue(2).ToString());
                            vcdbVehicleTemp.RegionID = Convert.ToInt32(reader.GetValue(3).ToString());
                            vcdbVehicleTemp.PublicationStageID = Convert.ToInt32(reader.GetValue(4).ToString());
                            vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.Add(vehicleidTemp, vcdbVehicleTemp);
                        }
                    }
                }
                reader.Close();
                importProgress = 10;

                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, DriveType.DriveTypeID from Vehicle, VehicleToDriveType, DriveType where Vehicle.VehicleID= VehicleToDriveType.VehicleID and VehicleToDriveType.DriveTypeID = DriveType.DriveTypeID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].DriveTypeIDlist.Add(Convert.ToInt32(reader.GetValue(2).ToString()));
                    }
                }
                reader.Close();
                importProgress = 12;


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, WheelBase.WheelBaseID from Vehicle, VehicleToWheelbase, WheelBase where Vehicle.VehicleID= VehicleToWheelbase.VehicleID and VehicleToWheelbase.WheelbaseID = WheelBase.WheelBaseID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());

                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].WheelBaseIDlist.Add(Convert.ToInt32(reader.GetValue(2).ToString()));
                    }
                }
                reader.Close();
                importProgress =13;


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, MfrBodyCode.MfrBodyCodeID from Vehicle, VehicleToMfrBodyCode, MfrBodyCode where Vehicle.VehicleID= VehicleToMfrBodyCode.VehicleID and VehicleToMfrBodyCode.MfrBodyCodeID =MfrBodyCode.MfrBodyCodeID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());

                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].MfrBodyCodeIDlist.Add(Convert.ToInt32(reader.GetValue(2).ToString()));
                    }
                }
                reader.Close();
                importProgress = 14;

                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, BedLengthID, BedTypeID from Vehicle, VehicleToBedConfig, BedConfig where Vehicle.VehicleID = VehicleToBedConfig.VehicleID and VehicleToBedConfig.BedConfigID = BedConfig.BedConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());

                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBedConfig vcdbBedConfigTemp = new vcdbBedConfig();
                        vcdbBedConfigTemp.BedLengthID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbBedConfigTemp.BedTypeID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].BedConfigList.Add(vcdbBedConfigTemp);
                    }
                }
                reader.Close();
                importProgress = 15;

                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID,  BodyTypeID, BodyNumDoorsID from Vehicle, VehicleToBodyStyleConfig, BodyStyleConfig where Vehicle.VehicleID = VehicleToBodyStyleConfig.VehicleID and VehicleToBodyStyleConfig.BodyStyleConfigID = BodyStyleConfig.BodyStyleConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBodyStyleConfig vcdbBodyStyleConfigTemp = new vcdbBodyStyleConfig();
                        vcdbBodyStyleConfigTemp.BodyTypeID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbBodyStyleConfigTemp.BodyNumDoorsID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].BodyStyleConfigList.Add(vcdbBodyStyleConfigTemp);
                    }
                }
                reader.Close();
                importProgress = 17;


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, FrontBrakeTypeID, RearBrakeTypeID, BrakeSystemID, BrakeABSID from Vehicle, VehicleToBrakeConfig, BrakeConfig where Vehicle.VehicleID = VehicleToBrakeConfig.VehicleID and VehicleToBrakeConfig.BrakeConfigID = BrakeConfig.BrakeConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbBrakeConfig vcdbBrakeConfigTemp = new vcdbBrakeConfig();
                        vcdbBrakeConfigTemp.FrontBrakeTypeID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbBrakeConfigTemp.RearBrakeTypeID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBrakeConfigTemp.BrakeSystemID = Convert.ToInt32(reader.GetValue(4).ToString());
                        vcdbBrakeConfigTemp.BrakeABSID = Convert.ToInt32(reader.GetValue(5).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].BrakeConfigList.Add(vcdbBrakeConfigTemp);
                    }
                }
                reader.Close();
                importProgress = 20;

                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, FrontSpringTypeID, RearSpringTypeID from Vehicle, VehicleToSpringTypeConfig, SpringTypeConfig where Vehicle.VehicleID = VehicleToSpringTypeConfig.VehicleID and VehicleToSpringTypeConfig.SpringTypeConfigID = SpringTypeConfig.SpringTypeConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbSpringTypeConfig vcdbSpringTypeConfigTemp = new vcdbSpringTypeConfig();
                        vcdbSpringTypeConfigTemp.FrontSpringTypeID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbSpringTypeConfigTemp.RearSpringTypeID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].SpringTypeConfigList.Add(vcdbSpringTypeConfigTemp);
                    }
                }
                reader.Close();
                importProgress = 30;


                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, SteeringTypeID, SteeringSystemID from Vehicle, VehicleToSteeringConfig, SteeringConfig where Vehicle.VehicleID = VehicleToSteeringConfig.VehicleID and VehicleToSteeringConfig.SteeringConfigID = SteeringConfig.SteeringConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbSteeringConfig vcdbSteeringConfigTemp = new vcdbSteeringConfig();
                        vcdbSteeringConfigTemp.SteeringTypeID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbSteeringConfigTemp.SteeringSystemID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].SteeringConfigList.Add(vcdbSteeringConfigTemp);
                    }
                }
                reader.Close();
                importProgress = 40;


                // if you wanted to limit the vcdb to type2 (pass car and light truck) vehicles:
                //command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, EngineConfig.EngineBaseID, EngineDesignationID, EngineVINID, ValvesID, AspirationID, CylinderHeadTypeID, FuelTypeID, IgnitionSystemTypeID, EngineMfrID, EngineVersionID, PowerOutputID, FuelDeliveryTypeID, FuelDeliverySubTypeID, FuelSystemControlTypeID, FuelSystemDesignID from BaseVehicle, Model, VehicleType, Vehicle, VehicleToEngineConfig, EngineConfig, EngineBase, FuelDeliveryConfig where Vehicle.BaseVehicleID=BaseVehicle.BaseVehicleID and BaseVehicle.ModelID=Model.ModelID and Model.VehicleTypeID=VehicleType.VehicleTypeID and Vehicle.VehicleID = VehicleToEngineConfig.VehicleID and VehicleToEngineConfig.EngineConfigID = EngineConfig.EngineConfigID and EngineConfig.EngineBaseID = EngineBase.EngineBaseID and EngineConfig.FuelDeliveryConfigID=FuelDeliveryConfig.FuelDeliveryConfigID and VehicleType.VehicleTypeGroupID=2";
                command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, EngineConfig.EngineBaseID, EngineDesignationID, EngineVINID, ValvesID, AspirationID, CylinderHeadTypeID, FuelTypeID, IgnitionSystemTypeID, EngineMfrID, EngineVersionID, PowerOutputID, FuelDeliveryTypeID, FuelDeliverySubTypeID, FuelSystemControlTypeID, FuelSystemDesignID from Vehicle, VehicleToEngineConfig, EngineConfig, EngineBase, FuelDeliveryConfig where Vehicle.VehicleID = VehicleToEngineConfig.VehicleID and VehicleToEngineConfig.EngineConfigID = EngineConfig.EngineConfigID and EngineConfig.EngineBaseID = EngineBase.EngineBaseID and EngineConfig.FuelDeliveryConfigID=FuelDeliveryConfig.FuelDeliveryConfigID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbEngineConfig vcdbEngineConfigTemp = new vcdbEngineConfig();
                        vcdbEngineConfigTemp.EngineBaseID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbEngineConfigTemp.EngineDesignationID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbEngineConfigTemp.EngineVINID = Convert.ToInt32(reader.GetValue(4).ToString());
                        vcdbEngineConfigTemp.ValvesID = Convert.ToInt32(reader.GetValue(5).ToString());
                        vcdbEngineConfigTemp.AspirationID = Convert.ToInt32(reader.GetValue(6).ToString());
                        vcdbEngineConfigTemp.CylinderHeadTypeID = Convert.ToInt32(reader.GetValue(7).ToString());
                        vcdbEngineConfigTemp.FuelTypeID = Convert.ToInt32(reader.GetValue(8).ToString());
                        vcdbEngineConfigTemp.IgnitionSystemTypeID = Convert.ToInt32(reader.GetValue(9).ToString());
                        vcdbEngineConfigTemp.EngineMfrID = Convert.ToInt32(reader.GetValue(10).ToString());
                        vcdbEngineConfigTemp.EngineVersionID = Convert.ToInt32(reader.GetValue(11).ToString());
                        vcdbEngineConfigTemp.PowerOutputID = Convert.ToInt32(reader.GetValue(12).ToString());
                        vcdbEngineConfigTemp.FuelDeliveryTypeID = Convert.ToInt32(reader.GetValue(13).ToString());
                        vcdbEngineConfigTemp.FuelDeliverySubTypeID = Convert.ToInt32(reader.GetValue(14).ToString());
                        vcdbEngineConfigTemp.FuelSystemControlTypeID = Convert.ToInt32(reader.GetValue(15).ToString());
                        vcdbEngineConfigTemp.FuelSystemDesignID = Convert.ToInt32(reader.GetValue(16).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].EngineConfigList.Add(vcdbEngineConfigTemp);
                    }
                }
                reader.Close();
                importProgress = 50;

                // if you wanted to limit the vcdb to type2 (pass car and light truck) vehicles:
                //command.CommandText = "select Vehicle.BaseVehicleID, Vehicle.VehicleID, Transmission.TransmissionBaseID, TransmissionTypeID, TransmissionNumSpeedsID, TransmissionControlTypeID, TransmissionMfrCodeID, TransmissionElecControlledID, TransmissionMfrID from BaseVehicle, Model, VehicleType, Vehicle, VehicleToTransmission, Transmission, TransmissionBase where Vehicle.BaseVehicleID=BaseVehicle.BaseVehicleID and BaseVehicle.ModelID=Model.ModelID and Model.VehicleTypeID=VehicleType.VehicleTypeID and Vehicle.VehicleID = VehicleToTransmission.VehicleID and VehicleToTransmission.TransmissionID = Transmission.TransmissionID and Transmission.TransmissionBaseID = TransmissionBase.TransmissionBaseID and VehicleType.VehicleTypeGroupID=2";
                command.CommandText = "select BaseVehicleID, Vehicle.VehicleID, Transmission.TransmissionBaseID, TransmissionTypeID, TransmissionNumSpeedsID, TransmissionControlTypeID, TransmissionMfrCodeID, TransmissionElecControlledID, TransmissionMfrID from Vehicle, VehicleToTransmission, Transmission, TransmissionBase where Vehicle.VehicleID = VehicleToTransmission.VehicleID and VehicleToTransmission.TransmissionID = Transmission.TransmissionID and Transmission.TransmissionBaseID = TransmissionBase.TransmissionBaseID"; reader = command.ExecuteReader();
                while (reader.Read())
                {
                    basevehicleidTemp = Convert.ToInt32(reader.GetValue(0).ToString());
                    vehicleidTemp = Convert.ToInt32(reader.GetValue(1).ToString());
                    if (vcdbBasevhicleDict.ContainsKey(basevehicleidTemp) && vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict.ContainsKey(vehicleidTemp))
                    {
                        vcdbTransmission vcdbTransmissionTemp = new vcdbTransmission();
                        vcdbTransmissionTemp.TransmissionBaseID = Convert.ToInt32(reader.GetValue(2).ToString());
                        vcdbTransmissionTemp.TransmissionTypeID = Convert.ToInt32(reader.GetValue(3).ToString());
                        vcdbTransmissionTemp.TransmissionNumSpeedsID = Convert.ToInt32(reader.GetValue(4).ToString());
                        vcdbTransmissionTemp.TransmissionControlTypeID = Convert.ToInt32(reader.GetValue(5).ToString());
                        vcdbTransmissionTemp.TransmissionMfrCodeID = Convert.ToInt32(reader.GetValue(6).ToString());
                        vcdbTransmissionTemp.TransmissionElecControlledID = Convert.ToInt32(reader.GetValue(7).ToString());
                        vcdbTransmissionTemp.TransmissionMfrID = Convert.ToInt32(reader.GetValue(8).ToString());
                        vcdbBasevhicleDict[basevehicleidTemp].vcdbVehicleDict[vehicleidTemp].TransmissionList.Add(vcdbTransmissionTemp);
                    }
                }
                reader.Close();
                importProgress = 62;

                //BaseVehicle bvtemp = new BaseVehicle();
                //bvtemp = vcdbBasevhicleDict[2231];

                command.CommandText = "SELECT VersionDate from Version"; reader = command.ExecuteReader();
                while (reader.Read()) { version = reader.GetValue(0).ToString(); }

                DateTime dt = new DateTime();
                if (DateTime.TryParseExact(version, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) { version = dt.ToString("yyyy-MM-dd"); }
                reader.Close();
                importProgress = 63;

                command.CommandText = "SELECT EngineBaseid,liter,cc,cid,cylinders,blocktype from EngineBase"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); enginebaseDict.Add(i, reader.GetValue(5).ToString().Trim() + reader.GetValue(4).ToString().Trim() + " " + reader.GetValue(1).ToString().Trim() + "L"); }
                reader.Close();
                importProgress = 64;

                command.CommandText = "SELECT Submodelid,submodelname from SubModel"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); submodelDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 65;

                command.CommandText = "SELECT Drivetypeid,drivetypename from DriveType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); drivetypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 66;

                command.CommandText = "SELECT aspirationid,aspirationname from Aspiration"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); aspirationDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 67;

                command.CommandText = "SELECT fueltypeid,fueltypename from FuelType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fueltypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 68;

                command.CommandText = "SELECT braketypeid,braketypename from BrakeType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); braketypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 69;

                command.CommandText = "SELECT brakeabsid,brakeabsname from BrakeABS"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); brakeabsDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 70;

                command.CommandText = "SELECT mfrbodycodeid,mfrbodycodename from MfrBodyCode"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); mfrbodycodeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 71;

                command.CommandText = "SELECT bodynumdoorsid,bodynumdoors from BodyNumDoors"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); bodynumdoorsDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 72;

                command.CommandText = "SELECT bodytypeid,bodytypename from BodyType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); bodytypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 73;

                command.CommandText = "SELECT enginedesignationid,enginedesignationname from EngineDesignation"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); enginedesignationDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 74;

                command.CommandText = "SELECT enginevinid,enginevinname from EngineVIN"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); enginevinDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 75;

                command.CommandText = "SELECT engineversionid,engineversion from EngineVersion"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); engineversionDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 76;

                command.CommandText = "SELECT mfrid,mfrname from Mfr"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); mfrDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 77;

                command.CommandText = "SELECT fueldeliverytypeid,fueldeliverytypename from FuelDeliveryType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fueldeliverytypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 78;

                command.CommandText = "SELECT fueldeliverysubtypeid,fueldeliverysubtypename from FuelDeliverySubType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fueldeliverysubtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 79;

                command.CommandText = "SELECT fuelsystemcontroltypeid,fuelsystemcontroltypename from FuelSystemControlType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fuelsystemcontroltypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 80;

                command.CommandText = "SELECT fuelsystemdesignid,fuelsystemdesignname from FuelSystemDesign"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); fuelsystemdesignDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 81;

                command.CommandText = "SELECT cylinderheadtypeid,cylinderheadtypename from CylinderHeadType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); cylinderheadtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 82;

                command.CommandText = "SELECT ignitionsystemtypeid,ignitionsystemtypename from IgnitionSystemType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); ignitionsystemtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 83;

                command.CommandText = "SELECT transmissionmfrcodeid,transmissionmfrcode from TransmissionMfrCode"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissionmfrcodeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 84;

                command.CommandText = "SELECT TransmissionBase.TransmissionBaseID,TransmissionControlTypeName, transmissiontypename, transmissionnumspeeds from TransmissionBase, TransmissionType, TransmissionNumSpeeds, TransmissionControlType WHERE TransmissionBase.TransmissionTypeID = TransmissionType.TransmissionTypeID AND TransmissionBase.TransmissionNumSpeedsID = TransmissionNumSpeeds.TransmissionNumSpeedsID AND TransmissionBase.TransmissionControlTypeID = TransmissionControlType.TransmissionControlTypeID"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissionbaseDict.Add(i, reader.GetValue(1).ToString().Trim() + " " + reader.GetValue(2).ToString().Trim() + " Speed " + reader.GetValue(3).ToString().Trim()); }
                reader.Close();
                importProgress = 85;

                command.CommandText = "SELECT TransmissionTypeID,TransmissionTypeName from TransmissionType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissiontypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 86;

                command.CommandText = "SELECT TransmissionControlTypeID,TransmissionControlTypeName from TransmissionControlType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissioncontroltypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 87;

                command.CommandText = "select ElecControlledID, ElecControlled from ElecControlled"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissioeleccontrolledDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 88;

                command.CommandText = "SELECT TransmissionNumSpeedsID,TransmissionNumSpeeds from TransmissionNumSpeeds"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); transmissionnumspeedsDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 89;

                command.CommandText = "SELECT bedlengthid,bedlength from BedLength"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); bedlengthDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 90;

                command.CommandText = "SELECT bedtypeid,bedtypename from BedType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); bedtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 91;

                command.CommandText = "SELECT wheelbaseid,wheelbase from WheelBase"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); wheelbaseDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 92;

                command.CommandText = "SELECT brakesystemid,brakesystemname from BrakeSystem"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); brakesystemDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 93;

                command.CommandText = "SELECT regionid,regionname from Region"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); regionDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 94;

                command.CommandText = "SELECT springtypeid,springtypename from SpringType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); springtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 95;

                command.CommandText = "SELECT steeringsystemid,steeringsystemname from SteeringSystem"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); steeringsystemDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 96;

                command.CommandText = "SELECT steeringtypeid,steeringtypename from SteeringType"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); steeringtypeDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 97;

                command.CommandText = "SELECT valvesid,valvesperengine from Valves"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); valvesDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 98;

                command.CommandText = "select PowerOutputID,HorsePower from PowerOutput"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); poweroutputDict.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();
                importProgress = 99;

                importSuccess = true;

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                importExceptionMessage = ex.Message;
                importSuccess = false;
                return ex.Message;
            }
            return "";
        }



        public string importMySQLchangelog()
        {
            importSuccess = false;
            try
            {
                int i;
                MySqlCommand command = new MySqlCommand("select PrimaryKeyBefore, ColumnName, ColumnValueBefore  from ChangeDetails where ChangeAttributeStateID = 2 and TableNameID = 16 and PrimaryKeyAfter is NULL order by PrimaryKeyBefore, ColumnName", connectionMySQLlist.First());
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    importSuccess = true;
                    i = Convert.ToInt32(reader.GetValue(0).ToString());
                    KeyValuePair<string, string> myPair = new KeyValuePair<string, string>(reader.GetValue(1).ToString(), reader.GetValue(2).ToString());

                    if (!deletedEngineBaseDict.ContainsKey(i))
                    {// first record that references this enginebaseid

                        List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();
                        deletedEngineBaseDict.Add(i, myList);
                    }
                    deletedEngineBaseDict[i].Add(myPair);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                importExceptionMessage = ex.Message;
                importSuccess = false;
                return ex.Message;
            }
            return "";
        }

    }






    // the contents of the PCdb (local Access file or remote database) are loaded into hash tables ("Dictionaries") for the sake of speed.
    public class PCdb
    {
        public bool useRemoteDB;
        public OleDbConnection connectionOLEDB = new OleDbConnection();
        public string MySQLdatabaseName = "";
        public string MySQLusername = "";
        public string MySQLpassword = "";
        public string MySQLconnectionString = "";
        public List<string> pcdbVersionsOnServerList = new List<string>();
        public MySqlConnection connectionMySQL = null;
        public bool importSuccess = false;
        public string importExceptionMessage = "";
        public string filePath = "";
        public string version = "";

        public Dictionary<int, String> parttypes = new Dictionary<int, string>();
        public Dictionary<int, String> positions = new Dictionary<int, string>();
        public List<string> codmasterParttypePoisitions = new List<string>();


        public string importOLEdb()
        {
            importSuccess = false; int i;
            try
            {
                OleDbCommand command = new OleDbCommand("select VersionDate from Version");
                command.Connection = connectionOLEDB;

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    version = reader.GetValue(0).ToString();
                }
                reader.Close();

                DateTime dt = new DateTime(); if (DateTime.TryParseExact(version, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) { version = dt.ToString("yyyy-MM-dd"); }

                //prebake all the parttype/name relationships into a hashtable ("Dictionary")
                command.CommandText = "select partterminologyid,partterminologyname from Parts"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); parttypes.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();

                //prebake all the position/name relationships into a hashtable ("Dictionary")
                command.CommandText = "select PositionID, [Position] from Positions"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); positions.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();

                // make a composite key string in a List<string> to store all valid combinations of parttype/position from the codemaster table of the pcdb.
                command.CommandText = "select partterminologyid, positionid from codemaster"; reader = command.ExecuteReader();
                while (reader.Read()){codmasterParttypePoisitions.Add(Convert.ToInt32(reader.GetValue(0).ToString()) + "_" + Convert.ToInt32(reader.GetValue(1).ToString()));}
                reader.Close();
                importSuccess = true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                importExceptionMessage = ex.Message;
                importSuccess = false;
                return ex.Message;
            }
            return "";
        }


        public string importMySQLdata()
        {
            importSuccess = false; int i;
            try
            {
                MySqlCommand command = new MySqlCommand("select VersionDate from Version");
                command.Connection = connectionMySQL;

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read()) { version = reader.GetValue(0).ToString(); }
                reader.Close();

                DateTime dt = new DateTime(); if (DateTime.TryParseExact(version, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) { version = dt.ToString("yyyy-MM-dd"); }

                //prebake all the parttype/name relationships into a hashtable ("Dictionary")
                command.CommandText = "select partterminologyid,partterminologyname from Parts"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); parttypes.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();

                //prebake all the position/name relationships into a hashtable ("Dictionary")
                command.CommandText = "select PositionID, Position from Positions"; reader = command.ExecuteReader();
                while (reader.Read()) { i = Convert.ToInt32(reader.GetValue(0).ToString()); positions.Add(i, reader.GetValue(1).ToString()); }
                reader.Close();

                // make a composite key string in a List<string> to store all valid combinations of parttype/position from the codemaster table of the pcdb.
                command.CommandText = "select partterminologyid, positionid from CodeMaster"; reader = command.ExecuteReader();
                while (reader.Read()) { codmasterParttypePoisitions.Add(Convert.ToInt32(reader.GetValue(0).ToString()) + "_" + Convert.ToInt32(reader.GetValue(1).ToString())); }
                reader.Close();
                importSuccess = true;
            }
            catch (Exception ex)
            {
                importExceptionMessage = ex.Message;
                importSuccess = false;
                return ex.Message;
            }
            return "";
        }


        public string connectLocalOLEDB(string path)
        {
            string result = "";
            filePath = path;
            try
            {
                if(connectionOLEDB.State != System.Data.ConnectionState.Closed)
                {// already open - close it first
                    connectionOLEDB.Close();
                }
                connectionOLEDB.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Mode=Read";
                connectionOLEDB.Open();
            }
            catch (Exception ex) { result = ex.Message; }
            return result;
        }
       
        public string connectMySQL()
        {
            string result = "";
            try
            {
                connectionMySQL = new MySqlConnection(MySQLconnectionString);
                connectionMySQL.Open();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public void disconnect()
        {
            if (useRemoteDB)
            {// disconnect MySQL 
                try
                {
                    connectionMySQL.Close();
                    connectionMySQL.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {// disconnect from the local database file
                filePath = "";
                if (connectionOLEDB.State != System.Data.ConnectionState.Closed)
                {
                    connectionOLEDB.Close();
                }
            }
        }

        public void clear()
        {
            filePath = "";
            version = "";
            parttypes.Clear();
            positions.Clear();
            codmasterParttypePoisitions.Clear();
            pcdbVersionsOnServerList.Clear();
        }

        public string niceParttype(int parttypeid)
        {
            string niceValue = "";
            if(parttypes.TryGetValue(parttypeid, out niceValue)){ return niceValue; }
            return parttypeid.ToString(); 
        }


        public string nicePosition(int positionid)
        {
            string niceValue = "";
            if(positions.TryGetValue(positionid, out niceValue)){ return niceValue; }
            return positionid.ToString();
        }



    }

    // the contents of the Qdb are loaded into hash tables ("Dictionaries") for the sake of speed.
    public class Qdb
    {
        public bool useRemoteDB;
        public OleDbConnection connectionOLEDB = new OleDbConnection();
        public string MySQLdatabaseName = "";
        public string MySQLusername = "";
        public string MySQLpassword = "";
        public string MySQLconnectionString = "";
        public List<string> qdbVersionsOnServerList = new List<string>();
        public MySqlConnection connectionMySQL = null;
        public string filePath = "";
        public string version = "";
        public bool importSuccess = false;
        public string importExceptionMessage = "";
        public Dictionary<int, String> qualifiers = new Dictionary<int, string>();
        public Dictionary<int, int> qualifiersTypes = new Dictionary<int, int>();


        public string importOLEdb()
        {
            importSuccess = false; 
            int qualifierid; int qualifiertypeid;
            try
            {
                OleDbCommand command = new OleDbCommand("select versiondate from Version");
                command.Connection = connectionOLEDB;

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read()) { version = reader.GetValue(0).ToString(); }
                reader.Close();

                DateTime dt = new DateTime(); if (DateTime.TryParseExact(version, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) { version = dt.ToString("yyyy-MM-dd"); }

                //prebake all the parttype/name relationships into a hashtable ("Dictionary")
                command.CommandText = "select qualifierid,qualifiertext,qualifiertypeid from Qualifier order by qualifierid"; reader = command.ExecuteReader();
                while (reader.Read()) 
                {
                    qualifierid = Convert.ToInt32(reader.GetValue(0).ToString());
                    qualifiers.Add(qualifierid, reader.GetValue(1).ToString());
                    qualifiertypeid = 0; try { qualifiertypeid = Convert.ToInt32(reader.GetValue(2).ToString()); } catch { } // silently fail - typeid wil default to 0
                    qualifiersTypes.Add(qualifierid, qualifiertypeid);
                }
                reader.Close();
                importSuccess = true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                importExceptionMessage = ex.Message;
                importSuccess = false;
                return ex.Message;
            }
            return "";
        }


        public string importMySQLdata()
        {
            importSuccess = false; int qualifierid; int qualifiertypeid;
            try
            {
                MySqlCommand command = new MySqlCommand("select VersionDate from Version");
                command.Connection = connectionMySQL;

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read()) { version = reader.GetValue(0).ToString(); }
                reader.Close();

                DateTime dt = new DateTime(); if (DateTime.TryParseExact(version, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) { version = dt.ToString("yyyy-MM-dd"); }

                //prebake all the parttype/name relationships into a hashtable ("Dictionary")
                command.CommandText = "select qualifierid,qualifiertext,qualifiertypeid from Qualifier"; reader = command.ExecuteReader();
                while (reader.Read()) 
                {
                    qualifierid = Convert.ToInt32(reader.GetValue(0).ToString()); 
                    qualifiers.Add(qualifierid, reader.GetValue(1).ToString());
                    qualifiertypeid = Convert.ToInt32(reader.GetValue(2).ToString());
                    qualifiersTypes.Add(qualifierid, qualifiertypeid);
                }
                reader.Close();
                importSuccess = true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                importExceptionMessage = ex.Message;
                importSuccess = false;
                return ex.Message;
            }
            return "";
        }

        public string connectLocalOLEDB(string path)
        {
            string result = "";
            filePath = path;
            try
            {
                if (connectionOLEDB.State != System.Data.ConnectionState.Closed)
                {
                    connectionOLEDB.Close();
                }
                connectionOLEDB.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Mode=Read";
                connectionOLEDB.Open();
            }
            catch (Exception ex) { result = ex.Message; }
            return result;
        }

        public string connectMySQL()
        {
            string result = "";
            try
            {
                connectionMySQL = new MySqlConnection(MySQLconnectionString);
                connectionMySQL.Open();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public void disconnect()
        {
            if (useRemoteDB)
            {// disconnect MySQL 
                try
                {
                    connectionMySQL.Close();
                    connectionMySQL.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {// disconnect from the local database file
                filePath = "";
                if (connectionOLEDB.State != System.Data.ConnectionState.Closed)
                {
                    connectionOLEDB.Close();
                }
            }
        }

        public void clear()
        {
            filePath = "";
            version = "";
            qualifiers.Clear();
            qualifiersTypes.Clear();
        }



        public string niceQdbQualifier(int qualifierid, List<String> parameters)
        {
            string niceValue = "";

            if (qualifiers.Count > 0)
            {// Qdb reference database is in-play. try to lookup and nicify the supplied QdbId
                int parameterNumber = 1;
                int parameterStartingPos;
                int parameterEndingPos;
                int typeStartingPos;

                string parameterType;
                string fullRawParameterString;
                string parameterOpenChunk;
                if (qualifiers.TryGetValue(qualifierid, out niceValue))
                {
                    // <p1 type="size"/> - <p2 type="size"/> Or <p3 type="size"/> - <p4 type="size"/>
                    // num, name, size, weight, idlist, date
                    foreach (string parameter in parameters)
                    {
                        parameterOpenChunk = "<p" + parameterNumber.ToString() + " type=\"";
                        if (niceValue.Contains(parameterOpenChunk))
                        {
                            parameterStartingPos = niceValue.IndexOf(parameterOpenChunk, 0);
                            parameterEndingPos = niceValue.IndexOf("\"/>", parameterStartingPos);
                            typeStartingPos = niceValue.IndexOf("\"") + 1;
                            parameterType = niceValue.Substring(typeStartingPos, (parameterEndingPos - typeStartingPos));
                            fullRawParameterString = niceValue.Substring(parameterStartingPos, (parameterEndingPos - parameterStartingPos) + 3);
                            niceValue = niceValue.Replace(fullRawParameterString, parameter);
                        }
                        parameterNumber++;
                    }
                    return niceValue;
                }
                return qualifierid.ToString();
            }
            else
            {// no Qdb reference data was supplied - build a string of the ID and parameters for the sake of overlaps detection functions that call this function

                niceValue = qualifierid.ToString();
                foreach (string parameter in parameters)
                {
                    niceValue += ":" + parameter;
                }
                niceValue += ";";
                return niceValue;
            }

        }


    }
       
    
}
