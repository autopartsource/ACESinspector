using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace ACESinspector
{

    public partial class Form1 : Form
    {
        ACES aces = new ACES();     // this instance will hold all the data imported from our "primary" ACES xml file
        ACES refaces = new ACES();  // this instance can hold all the data imported from our "reference" ACES xml file
        ACES diffaces = new ACES(); // this instance can hold the difference found between the primary and reference datasets 
        ACES errorsaces = new ACES();// this instance can hold the VCdb-configuration errors found in the primary file. This can be used to export the errors to file or webservice
        VCdb vcdb = new VCdb(); // this class will hold all the contents of the the imported VCdb M$Access file - mostly in "Dictionary" type variables for super-fast lookup (way faster than repeatedly querying the underlying Access file)
                                // it can also holds a list of multiple in-parallel connections to a remote mysql database
        PCdb pcdb = new PCdb();
        Qdb qdb = new Qdb();

        List<string> cacheFilesToDeleteOnExit = new List<string>();


        bool toolTipIsShown = false;

        int historyLineCountAtLastCheck = 0;
      

        // global stuff associated with the drawing of fitment tree diagrams
        int mouseDownX, mouseDownY; 
        int treeCanvasXoffset, treeCanvasYoffset;
        int treeCanvasXbase, treeCanvasYbase;
        string macroProblemGroupKeyInView;
        int fitmentProblemAppIdInView;
        fitmentNode treeNodeBeingDragged = new fitmentNode();
        bool treeCanvasIsBeingDragged;

        int largeDatagridRecordThreshold = 3000;  // datgrid control is not great with huge datasets. this threshold is about when to re-direct the user to look at the results in the export spreadsheet

        public Dictionary<String, String> noteTranslationDictionary = new Dictionary<string, string>();

        private TabPage hiddenAddsDropsVehiclesTab = new TabPage();
        private TabPage hiddenAddsDropsPartsTab = new TabPage();
        private TabPage hiddenInvalidConfigurationsTab = new TabPage();
        private TabPage hiddenInvalidVCdbCodesTab = new TabPage();
        private TabPage hiddenInvalidBaseVehiclesTab = new TabPage();
        private TabPage hiddenQdbErrorsTab = new TabPage();
        private TabPage hiddenParttypePositionErrorsTab = new TabPage();
        private TabPage hiddenPartTypeDisagreementTab = new TabPage();
        private TabPage hiddenPartsTab = new TabPage();
        private TabPage hiddenExportsTab = new TabPage();
        private TabPage hiddenQuantityWarningsTab = new TabPage();
        private TabPage hiddenLogicProblemsTab = new TabPage();
        private TabPage hiddenStatsTab = new TabPage();
        private TabPage hiddenSettingsTab = new TabPage();


        private int highestVisableTab1Index;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            pictureBoxFitmentTree.Invalidate();
            treeNodeBeingDragged.deleted = true;

            lblAppVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
//            lblAppVersion.Text = Environment.ProcessorCount.ToString();

            btnSelectReferenceACESfile.Enabled = false;
//            btnSelectVCdbFile.Enabled = false;
//            btnSelectPCdbFile.Enabled = false;
//            btnSelectQdbFile.Enabled = false;

            lblStatus.Text = "";
            lblPrimeACESLoadStatus.Text = "";
            lblRefACESLoadStatus.Text = "";
            lblStatsTitle.Text = "";
            lblStatsVCdbVersion.Text = "";
            lblStatsPCdbVersion.Text = "";
            lblStatsACESversion.Text = "";
            lblStatsAppsCount.Text = "";
            lblStatsPartsCount.Text = "";
            lblStatsQdbVersion.Text = "";
            lblMacroProblems.Text = "";
            lblDifferentialsSummary.Text = "";
            lblIndividualErrors.Text = "";
            lblStatsProcessingTime.Text = "";
            lblProcessTimeTitle.Text = "";
            lblVCdbFilePath.Text = "";
            lblVCdbLoadStatus.Text = "";
            progBarVCdbload.Value = 0; progBarVCdbload.Visible = false;
            lblPCdbFilePath.Text = "";
            lblQdbFilePath.Text = "";
            lblACESfilePath.Text = "";
            lblReferenceACESfilePath.Text = "";
            lblinterchangefilePath.Text = "";
            lblNoteTranslationfilePath.Text = "";
            progBarPrimeACESload.Visible = false;
            progBarRefACESload.Visible = false;

            lblPartsTabRedirect.Visible = false;
            lblFitmentLogicProblemsTabRedirect.Visible = false;
            lblParttypePositionRedirect.Visible = false;
            lblQtyWarningsRedirect.Visible = false;
            lblVCdbConfigErrorRedirect.Visible = false;
            lblInvalidVCdbCodesRedirect.Visible = false;
            lblInvalidBasevehiclesRedirect.Visible = false;
            lblQdbErrorsRedirect.Visible = false;
            lblIndividualErrorsTitle.Visible = false;
            lblMacroProblemsTitle.Visible = false;
            lblAddsDropsPartsErrorRedirect.Visible = false;
            lblAddsDropsVehiclesErrorRedirect.Visible = false;

            btnAppExportSave.Enabled = false;
            btnBgExportSave.Enabled = false;
            btnNetChangeExportSave.Enabled = false;
            btnHolesExportSave.Enabled = false;
            btnExportRelatedParts.Enabled = false;

            btnAnalyze.Enabled = false;
            dgParts.Width = Width - 36;
            dgParts.Height = Height - 320;
            dgParttypeDisagreement.Width = Width - 36;
            dgParttypeDisagreement.Height = Height - 320;


            dgBasevids.Width = this.Width - 36;
            dgBasevids.Height = this.Height - 320;
            dgVCdbCodes.Width = this.Width - 36;
            dgVCdbCodes.Height = this.Height - 320;
            dgParttypePosition.Width = this.Width - 36;
            dgParttypePosition.Height = this.Height - 320;
            dgVCdbConfigs.Width = this.Width - 36;
            dgVCdbConfigs.Height = this.Height - 320;
            dgQdbErrors.Width = this.Width - 36;
            dgQdbErrors.Height = this.Height - 320;
            dgAddsDropsParts.Width = this.Width - 36;
            dgAddsDropsParts.Height = this.Height - 320;
            dgAddsDropsVehicles.Width = this.Width - 36;
            dgAddsDropsVehicles.Height = this.Height - 320;
            dgQuantityWarnings.Width = this.Width - 36;
            dgQuantityWarnings.Height = this.Height - 320;
            dgFitmentLogicProblems.Width = this.Width - 28;


            splitContainerFitmentLogic.Height = this.Height - 320;
            splitContainerFitmentLogic.Width = this.Width - 28;

            //pictureBoxLogicProblems.Visible = true;
            //pictureBoxCommonErrors.Visible = true;

            pictureBoxFitmentTree.Width = this.Width - 36; //236 if listBoxFitmentLogicElements is in use 
            listBoxFitmentLogicElements.Visible = false;

            lblDifferentialsLabel.Visible = false;
            lblDifferentialsSummary.Visible = false;
            progressBarDifferentials.Visible = false;
            dgAddsDropsParts.Visible = false;
            dgAddsDropsVehicles.Visible = false;
            dgBasevids.Visible = false;
            dgParttypeDisagreement.Visible = false;
            dgVCdbCodes.Visible = false;
            dgParttypePosition.Visible = false;
            dgVCdbConfigs.Visible = false;
            dgQdbErrors.Visible = false;
            dgQuantityWarnings.Visible = false;
            dgFitmentLogicProblems.Visible = false;
            pictureBoxFitmentTree.Visible = false;

            comboBoxExportDelimiter.SelectedIndex = 0;
            comboBoxFlatExportFormat.SelectedIndex = 0;

            tabControl1.Width = this.Width - 18;
            tabControl1.Height = this.Height - 288;
            lblAppVersion.Left = this.Width - 65;



            checkBoxLimitDataGridRows.Text = "Limit datagrid displays to "+ largeDatagridRecordThreshold.ToString() + " rows";


            // archive all the tabpages from the main tabcontrol into dedicated variables. These are the pages defined at design time.
            // we will add them back as needed;

            hiddenLogicProblemsTab = tabControl1.TabPages[13];
            hiddenQuantityWarningsTab = tabControl1.TabPages[12];
            hiddenAddsDropsVehiclesTab = tabControl1.TabPages[11];
            hiddenAddsDropsPartsTab = tabControl1.TabPages[10];
            hiddenInvalidConfigurationsTab = tabControl1.TabPages[9];
            hiddenInvalidVCdbCodesTab = tabControl1.TabPages[8];
            hiddenInvalidBaseVehiclesTab = tabControl1.TabPages[7];
            hiddenQdbErrorsTab = tabControl1.TabPages[6];
            hiddenParttypePositionErrorsTab = tabControl1.TabPages[5];
            hiddenPartTypeDisagreementTab = tabControl1.TabPages[4];
            hiddenPartsTab = tabControl1.TabPages[3];
            hiddenExportsTab = tabControl1.TabPages[2];
            hiddenSettingsTab = tabControl1.TabPages[1];
            hiddenStatsTab = tabControl1.TabPages[0];


            tabControl1.TabPages.RemoveAt(13);
            tabControl1.TabPages.RemoveAt(12);
            tabControl1.TabPages.RemoveAt(11);
            tabControl1.TabPages.RemoveAt(10);
            tabControl1.TabPages.RemoveAt(9);
            tabControl1.TabPages.RemoveAt(8);
            tabControl1.TabPages.RemoveAt(7);
            tabControl1.TabPages.RemoveAt(6);
            tabControl1.TabPages.RemoveAt(5);
            tabControl1.TabPages.RemoveAt(4);
            tabControl1.TabPages.RemoveAt(3);
            tabControl1.TabPages.RemoveAt(2);

            // index's 0 and 1 are "Statistics" and "Settings" (respectively) - they stay
            highestVisableTab1Index = 1; // "settings" is the highest at start

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if (key.GetValue("cacheDirectoryPath") == null)
            {
                MessageBox.Show("Please select a folder where ACESinspector can store cache files. This can be done in the Settings tab");
            }
            else
            {// registry contains a path that was provided in the past - verify it before we use it

                if (Directory.Exists(key.GetValue("cacheDirectoryPath").ToString()))
                {// valid directory
                    lblCachePath.Text = key.GetValue("cacheDirectoryPath").ToString();
                    Directory.CreateDirectory(lblCachePath.Text + "\\AiFragments");
                    aces.importFitmentPermutationMiningCache(lblCachePath.Text + @"\ACESinspector-fitment permutations.txt");
                }
                else
                {// registry-provided cache directory does not exist
                    MessageBox.Show("Please select a folder where ACESinspector can store cache files. This can be done in the \"Settings\" tab");
                }
            }

            if (key.GetValue("assessmentDirectoryPath") != null)
            {
                if (Directory.Exists(key.GetValue("assessmentDirectoryPath").ToString()))
                {// valid directory
                    lblAssessmentsPath.Text = key.GetValue("assessmentDirectoryPath").ToString();
                }
            }

            if ((string)key.GetValue("detectQtyOutliers") == "1") { checkBoxQtyOutliers.Checked = true; } else { checkBoxQtyOutliers.Checked = false; }
            if (key.GetValue("qtyOutliersThreshold") != null) { numericUpDownQtyOutliersThreshold.Value = Convert.ToDecimal(key.GetValue("qtyOutliersThreshold")); } else { numericUpDownQtyOutliersThreshold.Value = 1; }
            if (key.GetValue("qtyOutliersSampleSize") != null) { numericUpDownQtyOutliersSample.Value = Convert.ToDecimal(key.GetValue("qtyOutliersSampleSize")); } else { numericUpDownQtyOutliersSample.Value = 500; }
            if (key.GetValue("threadCount") != null) { numericUpDownThreads.Value = Convert.ToDecimal(key.GetValue("threadCount")); } else { numericUpDownThreads.Value = 20; }


            // look for the "secret" key in the registry that entitles bad-branch detection. If you are reading this, you are in pretty exclusive club and 
            // should feel free to add this key to your own registry to enable the feature. Bad branch detection is a feature (among others planned) that is held back from public view. 
            // And on that subject, you should call me (Luke Smith) at 804-525-1450. I can talk for hours about this shit :)
            // if (key.GetValue("enableBadBranchDetection") == null){groupBoxFitmentLogicSettings.Visible = false;}

            if ((string)key.GetValue("concernForDisparateBranches") == "1") { checkBoxConcernForDisparate.Checked = true; } else { checkBoxConcernForDisparate.Checked = false; }
            if ((string)key.GetValue("reportAllAppsInProblemGroup") == "1") { checkBoxReportAllAppsInProblemGroup.Checked = true; } else { checkBoxReportAllAppsInProblemGroup.Checked = false; }
            if (key.GetValue("treePermutationsLimit") != null)
            {
                Decimal decTemp = Convert.ToDecimal(key.GetValue("treePermutationsLimit"));
                if (decTemp < numericUpDownTreeConfigLimit.Minimum) { decTemp = numericUpDownTreeConfigLimit.Minimum; }
                if (decTemp > numericUpDownTreeConfigLimit.Maximum) { decTemp = numericUpDownTreeConfigLimit.Maximum; }
                numericUpDownTreeConfigLimit.Value = decTemp;
            }

            if ((string)key.GetValue("respectValidateNoTag") == "1") { checkBoxRespectValidateTag.Checked = true; } else { checkBoxRespectValidateTag.Checked = false; }
            if ((string)key.GetValue("explodeNoteTagsBySemicolon") == "1") { checkBoxExplodeNotes.Checked = true; } else { checkBoxExplodeNotes.Checked = false; }
            if ((string)key.GetValue("allowGraceForWildcardConfigs") == "1") { checkBoxUKgrace.Checked = true; } else { checkBoxUKgrace.Checked = false; }

            if (key.GetValue("limitDatagridRows") == null)
            {// no key present - we want to assert one and set its value to "1"
                key.SetValue("limitDatagridRows", "1"); checkBoxLimitDataGridRows.Checked = true;
            }
            else
            {
                if ((string)key.GetValue("limitDatagridRows") == "1") { checkBoxLimitDataGridRows.Checked = true; } else { checkBoxLimitDataGridRows.Checked = false; }
            }


            if (key.GetValue("autoloadReferenceDatabases") == null)
            {// no key present - we want to assert one and set its value to "1"
                key.SetValue("autoloadReferenceDatabases", "1"); checkBoxAutoloadLocalDatabases.Checked = true;
            }
            else
            {
                if ((string)key.GetValue("autoloadReferenceDatabases") == "1") { checkBoxAutoloadLocalDatabases.Checked = true; } else { checkBoxAutoloadLocalDatabases.Checked = false; }
            }

            if (key.GetValue("MySQLhost") != null) { textBoxMySQLhost.Text = key.GetValue("MySQLhost").ToString(); }
            if (key.GetValue("MySQLuser") != null) { textBoxMySQLuser.Text = key.GetValue("MySQLuser").ToString(); }
            if (key.GetValue("MySQLpassword") != null) { textBoxMySQLpassword.Text = key.GetValue("MySQLpassword").ToString(); }


            if (key.GetValue("datasource") == null)
            {// no key present - we want to assert one and set its value to "oledbAccess"
                key.SetValue("datasource", "oledbAccess"); radioButtonDataSourceAccess.Checked = true;
            }
            else
            {
                if ((string)key.GetValue("datasource") == "oledbAccess")
                {
                    radioButtonDataSourceAccess.Checked = true;
                }
                else
                {
                    if ((string)key.GetValue("datasource") == "mysql")
                    {
                        radioButtonDataSourceMySQL.Checked = true;
                    }
                }
            }


            if (checkBoxAutoloadLocalDatabases.Checked)
            {

                if (radioButtonDataSourceAccess.Checked)
                {
                    vcdb.importIsRunning = true;
                    progBarVCdbload.Visible = true;
                    btnSelectVCdbFile.Enabled = false;
                    btnSelectPCdbFile.Enabled = false;
                    btnSelectQdbFile.Enabled = false;

                    await Task.Run(() => loadLastLocalVCdb());
                    if (vcdb.version != "") { lblVCdbFilePath.Text = "Autoloaded " + vcdb.version + " from local Access file"; }
                    btnSelectVCdbFile.Enabled = true;
                    progBarVCdbload.Visible = false;
                    lblVCdbLoadStatus.Text = ""; lblVCdbLoadStatus.Visible = false;

                    await Task.Run(() => loadLastLocalPCdb());
                    if (pcdb.version != "") { lblPCdbFilePath.Text = "Autoloaded " + pcdb.version + " from local Access file"; }
                    btnSelectPCdbFile.Enabled = true;

                    await Task.Run(() => loadLastLocalQdb());
                    if (qdb.version != "") { lblQdbFilePath.Text = "Autoloaded " + qdb.version + " from local Access file"; }
                    btnSelectQdbFile.Enabled = true;

                    vcdb.importIsRunning = false;
                    btnSelectVCdbFile.Enabled = true;
                    btnSelectPCdbFile.Enabled = true;
                    btnSelectQdbFile.Enabled = true;


                    if (pcdb.version != "" && qdb.version != "" && aces.successfulImport) { btnAnalyze.Enabled = true; }

                }
            }

            // connect to MySQL source and populate list of avail versions
            if (radioButtonDataSourceMySQL.Checked)
            {
                getAvailableMySQLdatabaseList(); 

                comboBoxMySQLvcdbVersion.Items.Clear();
                if (vcdb.vcdbVersionsOnServerList.Count() > 0)
                {
                    comboBoxMySQLvcdbVersion.Items.AddRange(vcdb.vcdbVersionsOnServerList.ToArray());
                    comboBoxMySQLvcdbVersion.SelectedIndex = 0;
                    comboBoxMySQLvcdbVersion.Visible = true;
                }
                btnSelectVCdbFile.Visible = false;
                buttonMySQLloadVCdb.Visible = true;

                comboBoxMySQLpcdbVersion.Items.Clear();
                if (pcdb.pcdbVersionsOnServerList.Count() > 0)
                {
                    comboBoxMySQLpcdbVersion.Items.AddRange(pcdb.pcdbVersionsOnServerList.ToArray());
                    comboBoxMySQLpcdbVersion.SelectedIndex = 0;
                    comboBoxMySQLpcdbVersion.Visible = true;
                }
                buttonMySQLloadPCdb.Visible = true;
                btnSelectPCdbFile.Visible = false;

                comboBoxMySQLqdbVersion.Items.Clear();
                if (qdb.qdbVersionsOnServerList.Count() > 0)
                {
                    comboBoxMySQLqdbVersion.Items.AddRange(qdb.qdbVersionsOnServerList.ToArray());
                    comboBoxMySQLqdbVersion.SelectedIndex = 0;
                    comboBoxMySQLqdbVersion.Visible = true;
                }
                buttonMySQLloadQdb.Visible = true;
                btnSelectQdbFile.Visible = false;
            }

        }


        void ReportPrimeACESImportProgress(int value)
        {
            if (value > 100) { value = 100; }
            progBarPrimeACESload.Value = value;
            lblPrimeACESLoadStatus.Text = value.ToString() + " % ";
        }

        void ReportRefACESImportProgress(int value)
        {
            if (value > 100) { value = 100; }
            progBarPrimeACESload.Value = value;
            lblPrimeACESLoadStatus.Text = value.ToString() + " % ";
        }


        void ReportVCdbImportProgress(int value)
        {
            progBarVCdbload.Value = value;
            lblVCdbLoadStatus.Text = "Loading VCdb - "+value.ToString()+"%";
        }


        void ReportBuyersGuideExportProgress(int value)
        {
            if (value > 100) { value = 100; }
            progBarExportBuyersGuide.Value = value;
        }

        void ReportExportFlatAppsProgress(int value)
        {
            if (value > 100) { value = 100; }
            progBarExportBuyersGuide.Value = value;
        }

        void ReportExportrelatedPartsProgress(int value)
        {
            if (value > 100) { value = 100; }
            progBarExportRelatedParts.Value = value;
        }

        private async void btnSelectACESfile_Click(object sender, EventArgs e)
        {
            aces.logHistoryEvent("", "10\tbtnSelectACESfile clicked");

            btnSelectACESfile.Enabled = false;
            btnSelectVCdbFile.Enabled = false;
            btnSelectPCdbFile.Enabled = false;
            btnSelectQdbFile.Enabled = false;
            btnSelectPartInterchange.Enabled = false;
            btnSelectNoteTranslationFile.Enabled = false;
            btnExportRelatedParts.Enabled = false;
            btnAppExportSave.Enabled = false;
            progBarPrimeACESload.Visible = true;
            lblPrimeACESLoadStatus.Visible = true;

            lblACESfilePath.Visible = false;

            lblStatus.Visible = false;

            var progressIndicator = new Progress<int>(ReportPrimeACESImportProgress);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if (key.GetValue("lastACESDirectoryPath") != null) { openFileDialog.InitialDirectory = key.GetValue("lastACESDirectoryPath").ToString(); }

            aces.logHistoryEvent("", "10\tbtnSelectACESfile - opening dialog box");

            openFileDialog.Title = "Open primary  ACES XML file";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            DialogResult openFileResult = openFileDialog.ShowDialog();
            if (openFileResult.ToString() == "OK")
            {
                textBoxAnalysisHostory.Text = "";
                aces.analysisHistory.Clear();
                historyLineCountAtLastCheck = 0;

                progBarPrimeACESload.Value = 1;
                lblPrimeACESLoadStatus.Text = "1%";

                lblStatsTitle.Text = "";
                lblStatsACESversion.Text = "";
                lblStatsVCdbVersion.Text = ""; lblStatsVCdbVersion.ForeColor = SystemColors.ControlText;
                lblStatsPCdbVersion.Text = ""; lblStatsPCdbVersion.ForeColor = SystemColors.ControlText;
                lblStatsQdbVersion.Text = ""; lblStatsQdbVersion.ForeColor = SystemColors.ControlText;
                lblStatsAppsCount.Text = "";
                lblStatsPartsCount.Text = "";
                lblStatsProcessingTime.Text = "";
                lblProcessTimeTitle.Text = "";
                lblMacroProblems.Text = "";
                lblIndividualErrors.Text = "";
                lblDifferentialsSummary.Text = "";
                lblStatus.BackColor = SystemColors.ButtonFace;

                lblPartsTabRedirect.Visible = false;
                lblFitmentLogicProblemsTabRedirect.Visible = false;
                lblParttypePositionRedirect.Visible = false;
                lblQdbErrorsRedirect.Visible = false;
                lblQtyWarningsRedirect.Visible = false;
                lblVCdbConfigErrorRedirect.Visible = false;
                lblInvalidVCdbCodesRedirect.Visible = false;
                lblInvalidBasevehiclesRedirect.Visible = false;
                lblAddsDropsPartsErrorRedirect.Visible = false;
                lblAddsDropsVehiclesErrorRedirect.Visible = false;

                //progressBarCommonErrors.Value = 0;
                //progressBarLogicProblems.Value = 0;
                progressBarDifferentials.Value = 0;

                pictureBoxLogicProblems.Visible = false;
                pictureBoxCommonErrors.Visible = false;
                progressBarDifferentials.Visible = false;
                lblIndividualErrorsTitle.Visible = false;
                lblMacroProblemsTitle.Visible = false;

                dgParts.Rows.Clear();
                dgParts.Visible = false;

                dgParttypeDisagreement.Rows.Clear();
                dgParttypeDisagreement.Visible = false;

                dgBasevids.Rows.Clear();
                dgBasevids.Visible = false;

                dgVCdbCodes.Rows.Clear();
                dgVCdbCodes.Visible = false;

                dgParttypePosition.Rows.Clear();
                dgParttypePosition.Visible = false;

                dgVCdbConfigs.Rows.Clear();
                dgVCdbConfigs.Visible = false;

                dgQdbErrors.Rows.Clear();
                dgQdbErrors.Visible = false;

                dgQuantityWarnings.Rows.Clear();
                dgQuantityWarnings.Visible = false;

                dgFitmentLogicProblems.Rows.Clear();
                dgFitmentLogicProblems.Visible = false;
                pictureBoxFitmentTree.Visible = false;

                comboBoxRelatedTypesLeft.Items.Clear();
                comboBoxRelatedTypesRight.Items.Clear();

                // remove any tabs that may be visible from a previous analysis
                // there are 2 tabs that are always visable (0,1) they are Stats and Settings
                /*
                while (tabControl1.TabPages.Count > 0)
                {
                    tabControl1.TabPages.RemoveAt(tabControl1.TabPages.Count - 1);
                    highestVisableTab1Index--;
                }
                */
                tabControl1.TabPages.Clear();
                tabControl1.TabPages.Add(hiddenStatsTab); highestVisableTab1Index = 0;
                tabControl1.TabPages.Add(hiddenSettingsTab); highestVisableTab1Index++;
                tabControl1.TabPages[0].Invalidate();
                tabControl1.TabPages[1].Invalidate();


                aces.clear(); errorsaces.clear();
                lblStatus.Text = "Importing primary ACES xml file";
                lblACESfilePath.Text = Path.GetFileName(openFileDialog.FileName);
                lblACESfilePath.Left = 352;
                lblACESfilePath.Visible = true;

                aces.logHistoryEvent("", "10\tbtnSelectACESfile - calculating md5 hash of selected ACES file ("+ openFileDialog.FileName + ")");

                // calculate an md5 hash of the ACES file for later storage in the registry
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(openFileDialog.FileName))
                    {
                        aces.fileMD5hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                    }
                }

                aces.logHistoryEvent("", "10\tbtnSelectACESfile - importing ACES file");

                var result = await Task.Run(() => aces.importXML(openFileDialog.FileName, "", checkBoxRespectValidateTag.Checked,noteTranslationDictionary,vcdb, progressIndicator));

                if (aces.discardedDeletsOnImport > 0) { MessageBox.Show(openFileDialog.FileName + " contains \"D\" (delete) applications. These were excluded from the import. Only the \"A\" (add) application are used.");  aces.logHistoryEvent("", "10\tbtnSelectACESfile - found some deleted apps in import");}


                btnSelectACESfile.Enabled = true;
                btnSelectVCdbFile.Enabled = true;
                btnSelectPCdbFile.Enabled = true;
                btnSelectQdbFile.Enabled = true;
                btnBgExportSave.Enabled = true;
                btnHolesExportSave.Enabled = true;

                //if (aces.xmlValidationErrors.Count == 0 && aces.apps.Count > 0)
                if (aces.xmlValidationErrors.Count == 0)
                {
                    lblStatsTitle.Text = aces.DocumentTitle;
                    lblStatsAppsCount.Text = aces.apps.Count.ToString();

                    if(aces.xmlAppNodeCount!=aces.FooterRecordCount)
                    {
                        aces.logHistoryEvent("", "0\tApp nodes count ("+ aces.xmlAppNodeCount.ToString()+") does not agree with footer count claim ("+ aces.FooterRecordCount .ToString()+ ")");
                        //lblStatsAppsCount.Text = aces.apps.Count.ToString() + " (footer claims " + aces.FooterRecordCount.ToString() + " records)";
                    }

                    lblStatsACESversion.Text = aces.version;
                    lblStatsVCdbVersion.Text = aces.VcdbVersionDate;
                    lblStatsPCdbVersion.Text = aces.PcdbVersionDate;
                    lblStatsQdbVersion.Text = aces.QdbVersionDate;
                    lblStatsPartsCount.Text = aces.partsAppCounts.Count.ToString();

                    string versionDateTemp = "vcdb"+aces.VcdbVersionDate.Replace("-","");
                    if(vcdb.vcdbVersionsOnServerList.Contains(versionDateTemp))
                    {
                        comboBoxMySQLvcdbVersion.Items.Clear();
                        int matchingVersionIndex = 0;
                        foreach(string vcdbVersion in vcdb.vcdbVersionsOnServerList)
                        {
                            if (vcdbVersion == versionDateTemp) { matchingVersionIndex = comboBoxMySQLvcdbVersion.Items.Count; }
                            comboBoxMySQLvcdbVersion.Items.Add(vcdbVersion);
                        }
                        comboBoxMySQLvcdbVersion.SelectedIndex = matchingVersionIndex;
                    }

                    versionDateTemp = "pcdb" + aces.PcdbVersionDate.Replace("-", "");
                    if (pcdb.pcdbVersionsOnServerList.Contains(versionDateTemp))
                    {
                        comboBoxMySQLpcdbVersion.Items.Clear();
                        int matchingVersionIndex = 0;
                        foreach (string pcdbVersion in pcdb.pcdbVersionsOnServerList)
                        {
                            if (pcdbVersion == versionDateTemp) { matchingVersionIndex = comboBoxMySQLpcdbVersion.Items.Count; }
                            comboBoxMySQLpcdbVersion.Items.Add(pcdbVersion);
                        }
                        comboBoxMySQLpcdbVersion.SelectedIndex = matchingVersionIndex;
                    }

                    versionDateTemp = "qdb" + aces.QdbVersionDate.Replace("-", "");
                    if (qdb.qdbVersionsOnServerList.Contains(versionDateTemp))
                    {
                        comboBoxMySQLqdbVersion.Items.Clear();
                        int matchingVersionIndex = 0;
                        foreach (string qdbVersion in qdb.qdbVersionsOnServerList)
                        {
                            if (qdbVersion == versionDateTemp) { matchingVersionIndex = comboBoxMySQLqdbVersion.Items.Count; }
                            comboBoxMySQLqdbVersion.Items.Add(qdbVersion);
                        }
                        comboBoxMySQLqdbVersion.SelectedIndex = matchingVersionIndex;
                    }





                    // copy over the header stuff from the primary aces object to the diffs aces object
                    diffaces.clear();
                    diffaces.version = aces.version;
                    diffaces.Company = aces.Company;
                    diffaces.SenderName = aces.SenderName;
                    diffaces.SenderPhone = aces.SenderPhone;
                    diffaces.TransferDate = aces.TransferDate;
                    diffaces.EffectiveDate = aces.EffectiveDate;
                    diffaces.BrandAAIAID = aces.BrandAAIAID;
                    diffaces.DocumentTitle = aces.DocumentTitle;
                    diffaces.VcdbVersionDate = aces.VcdbVersionDate;
                    diffaces.QdbVersionDate = aces.QdbVersionDate;
                    diffaces.PcdbVersionDate = aces.PcdbVersionDate;

                    tabControl1.TabPages.Add(hiddenExportsTab); highestVisableTab1Index++; // make visible the "exports" tab page

                    foreach (int distinctPartType in aces.distinctPartTypes) { comboBoxRelatedTypesLeft.Items.Add(pcdb.niceParttype(distinctPartType)); comboBoxRelatedTypesRight.Items.Add(pcdb.niceParttype(distinctPartType)); }
                    comboBoxRelatedTypesLeft.SelectedIndex = 0; comboBoxRelatedTypesRight.SelectedIndex = 0;

                    progBarPrimeACESload.Visible = false;  progBarPrimeACESload.Value = 0; lblPrimeACESLoadStatus.Text = ""; lblPrimeACESLoadStatus.Visible = false;
                    lblACESfilePath.Left = progBarPrimeACESload.Left;
                    aces.logHistoryEvent("", "0\tValid ACES ("+aces.version+") imported as primary: "+ Path.GetFileName(aces.filePath));
                    btnAppExportSave.Enabled = true;
                    btnExportRelatedParts.Enabled = true;
                    
                    if ( vcdb.importSuccess  && pcdb.importSuccess && qdb.importSuccess && aces.successfulImport) { btnAnalyze.Enabled = true; }

                    key.SetValue("lastACESDirectoryPath", Path.GetDirectoryName(openFileDialog.FileName));

                    btnSelectReferenceACESfile.Enabled = true; // allow reference file to be seleecte now that primary is loaded and valid
                    btnAnalyze.Focus();
                }
                else
                {
                    MessageBox.Show("XML failed schema validation.\r\n\r\n" + aces.xmlValidationErrors[0]);
                    aces.logHistoryEvent("", "0\tXML failed schema validation.\r\n\r\n" + aces.xmlValidationErrors[0]);
                    progBarPrimeACESload.Value = 0; progBarPrimeACESload.Visible = false; lblPrimeACESLoadStatus.Text = ""; lblPrimeACESLoadStatus.Visible = false; lblACESfilePath.Text = ""; lblStatus.Text = ""; btnSelectPartInterchange.Enabled = true; btnSelectNoteTranslationFile.Enabled = true;
                    lblACESfilePath.Left = 352;
                }
            }
            else
            {// file open dialog result was not "OK" (probably candeled) 
                progBarPrimeACESload.Visible = false;
                btnSelectACESfile.Enabled = true; // re-enable the select button for primary aces file
                btnSelectPartInterchange.Enabled = true;
                btnSelectNoteTranslationFile.Enabled = true;
                btnSelectVCdbFile.Enabled = true;
                btnSelectPCdbFile.Enabled = true;
                btnSelectQdbFile.Enabled = true;
                aces.logHistoryEvent("", "10\tbtnSelectACESfile - result != ok");
            }

        }

        private async void btnSelectReferenceACESfile_Click(object sender, EventArgs e)
        {
            var progressIndicator = new Progress<int>(ReportRefACESImportProgress);

            progBarRefACESload.Visible = true;
            lblRefACESLoadStatus.Visible = true;


            OpenFileDialog openFileDialog = new OpenFileDialog();
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if (key.GetValue("lastReferenceACESDirectoryPath") != null) { openFileDialog.InitialDirectory = key.GetValue("lastReferenceACESDirectoryPath").ToString(); }

            openFileDialog.Title = "Open Reference ACES XML file";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            DialogResult openFileResult = openFileDialog.ShowDialog();
            if (openFileResult.ToString() == "OK")
            {
                refaces.clear();
                refaces.filePath = openFileDialog.FileName;

                using (var md5 = MD5.Create())
                {// calculate an md5 hash of the ACES file for later storage in the registry
                    using (var stream = File.OpenRead(openFileDialog.FileName)) { refaces.fileMD5hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty); }
                }

                lblStatus.Text = "Importing Reference ACES xml file";
                lblReferenceACESfilePath.Text = Path.GetFileName(openFileDialog.FileName);
                key.SetValue("lastReferenceACESDirectoryPath", Path.GetDirectoryName(openFileDialog.FileName));

                if (refaces.fileHasBeenAnalyzed(vcdb.version, pcdb.version)>0)
                {
                    var result = await Task.Run(() => refaces.importXML(openFileDialog.FileName, "", checkBoxRespectValidateTag.Checked,noteTranslationDictionary,vcdb, progressIndicator));
                    lblDifferentialsLabel.Visible = true; lblDifferentialsSummary.Visible = true; progressBarDifferentials.Visible = true;
                    progBarRefACESload.Value = 0; progBarRefACESload.Visible = false; lblRefACESLoadStatus.Text = ""; lblRefACESLoadStatus.Visible = false;
                    lblReferenceACESfilePath.Left = progBarRefACESload.Left;
                    if (refaces.xmlValidationErrors.Count()==0 && refaces.apps.Count()>0)
                    {
                        aces.logHistoryEvent("", "0\tValid ACES (" + aces.version + ") imported as reference: " + Path.GetFileName(refaces.filePath));
                    }
                    else
                    {
                        aces.logHistoryEvent("", "0\tProblems importing reference ACES: " + Path.GetFileName(refaces.filePath));
                    }
                }
                else
                {
                    MessageBox.Show("You must select a reference ACES file that has previoulsy been analyzed. Open this file first as a PRIMARY ACES and run an analysis. Then select it as a REFERENCE file.");
                    lblReferenceACESfilePath.Text = ""; lblDifferentialsLabel.Visible = true; lblDifferentialsSummary.Visible = true; progressBarDifferentials.Visible = true;
                    progBarRefACESload.Visible = false; progBarRefACESload.Value = 0; lblRefACESLoadStatus.Text = ""; lblRefACESLoadStatus.Visible = false;
                    lblReferenceACESfilePath.Left = 352;
                }
            }
            else
            {// dialog was canceled
                progBarRefACESload.Visible = false;

            }

        }

        private async void btnSelectVCdbFile_Click(object sender, EventArgs e)
        {
            string md5Hash = "";

            using (var openFileDialog = new OpenFileDialog())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key.CreateSubKey("ACESinspector");
                key = key.OpenSubKey("ACESinspector", true);
                if (key.GetValue("lastVCdbDirectoryPath") != null) { openFileDialog.InitialDirectory = key.GetValue("lastVCdbDirectoryPath").ToString(); }

                openFileDialog.RestoreDirectory = false;
                openFileDialog.Filter = "Access Database files (*.accdb)|*.accdb";

                DialogResult openFileResult = openFileDialog.ShowDialog();

                if (openFileResult.ToString() == "OK")
                {
                    vcdb.clear();
                    lblVCdbFilePath.Text = "Loading...";
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(openFileDialog.FileName))
                        {
                            md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                        }
                    }

                    vcdb.importIsRunning = true;
                    progBarVCdbload.Visible = true; progBarVCdbload.Value = 0; lblVCdbFilePath.Text = ""; lblVCdbLoadStatus.Text = "Loading VCdb - 0%";
                    vcdb.useRemoteDB = false;
                    vcdb.connectLocalOLEDB(openFileDialog.FileName);
                    var result = await Task.Run(() => vcdb.importOLEDBdata());
                    vcdb.importIsRunning = false;
                    lblVCdbFilePath.Text = "Local VCdb Version: " + vcdb.version;
                    lblVCdbLoadStatus.Text = "";
                    progBarVCdbload.Value = 0; progBarVCdbload.Visible = false;
                    if (vcdb.version != "")
                    {
                        key.SetValue("lastVCdbDirectoryPath", Path.GetDirectoryName(openFileDialog.FileName));
                        key.SetValue("lastVCdbFilePath", openFileDialog.FileName);
                        key.SetValue("lastVCdbFileMD5", md5Hash);
                        aces.logHistoryEvent("", "1\tLoaded local VCdb: " + vcdb.version);
                        if (pcdb.version != "" && qdb.version != "" && aces.successfulImport) { btnAnalyze.Enabled = true; }
                    }
                    else
                    {
                        MessageBox.Show("Error importing VCdb (" + result + ")"); aces.logHistoryEvent("", "0\tError importing VCdb (" + result + ")");
                    }
                }
            }
        }

        private async void btnSelectPCdbFile_Click(object sender, EventArgs e)
        {
//            var progressIndicator = new Progress<int>(ReportImportProgress);
            string md5Hash = "";
            using (var openFileDialog = new OpenFileDialog())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key.CreateSubKey("ACESinspector");
                key = key.OpenSubKey("ACESinspector", true);
                if (key.GetValue("lastPCdbDirectoryPath") != null) { openFileDialog.InitialDirectory = key.GetValue("lastPCdbDirectoryPath").ToString(); }
                openFileDialog.RestoreDirectory = false;
                openFileDialog.Filter = "Access Database files (*.accdb)|*.accdb";
                DialogResult openFileResult = openFileDialog.ShowDialog();

                if (openFileResult.ToString() == "OK")
                {
                    pcdb.clear();
                    pcdb.useRemoteDB = false;
                    lblPCdbFilePath.Text = "Loading...";
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(openFileDialog.FileName))
                        {
                            md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                        }
                    }

                    pcdb.connectLocalOLEDB(openFileDialog.FileName);
                    var result = await Task.Run(() => pcdb.importOLEdb());
                    lblPCdbFilePath.Text = "Local PCdb Version: " + pcdb.version;
                    if (pcdb.version != "")
                    {
                        key.SetValue("lastPCdbDirectoryPath", Path.GetDirectoryName(openFileDialog.FileName));
                        key.SetValue("lastPCdbFilePath", openFileDialog.FileName);
                        key.SetValue("lastPCdbFileMD5", md5Hash);
                        aces.logHistoryEvent("", "1\tLoaded local PCdb: " + pcdb.version);
                        if (qdb.version != "" && vcdb.version!= "" && aces.successfulImport) { btnAnalyze.Enabled = true; }
                    }
                    else
                    {
                        MessageBox.Show("Error importing local PCdb (" + result + ")"); aces.logHistoryEvent("", "0\tError importing local PCdb (" + result + ")");
                    }
                }
            }
        }



        private async void btnSelectQdbFile_Click(object sender, EventArgs e)
        {
            //var progressIndicator = new Progress<int>(ReportImportProgress);
            string md5Hash = "";
            using (var openFileDialog = new OpenFileDialog())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key.CreateSubKey("ACESinspector");
                key = key.OpenSubKey("ACESinspector", true);
                if (key.GetValue("lastQdbDirectoryPath") != null) { openFileDialog.InitialDirectory = key.GetValue("lastQdbDirectoryPath").ToString(); }
                openFileDialog.RestoreDirectory = false;
                openFileDialog.Filter = "Access Database files (*.accdb)|*.accdb";
                DialogResult openFileResult = openFileDialog.ShowDialog();

                if (openFileResult.ToString() == "OK")
                {
                    qdb.disconnect();
                    qdb.clear();
                    lblQdbFilePath.Text = "Loading...";
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(openFileDialog.FileName))
                        {
                            md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                        }
                    }

                    qdb.connectLocalOLEDB(openFileDialog.FileName);
                    var result = await Task.Run(() => qdb.importOLEdb());
                    lblQdbFilePath.Text = "Local Qdb Version: " + qdb.version;
                    if (qdb.version != "")
                    {
                        key.SetValue("lastQdbDirectoryPath", Path.GetDirectoryName(openFileDialog.FileName));
                        key.SetValue("lastQdbFilePath", openFileDialog.FileName);
                        key.SetValue("lastQdbFileMD5", md5Hash);
                        aces.logHistoryEvent("", "1\tLoaded local Qdb: " + qdb.version);
                        if (pcdb.version != "" && vcdb.version!="" && aces.successfulImport) { btnAnalyze.Enabled = true; }
                    }
                    else
                    {
                        MessageBox.Show("Error importing Qdb (" + result + ")"); aces.logHistoryEvent("", "0\tError importing Qdb (" + result + ")");
                    }
                }
            }


        }


        public async void loadLastLocalVCdb()
        {
            var progressIndicator = new Progress<int>(ReportVCdbImportProgress);
            string vcdbFilePath = null;
            string md5Hash;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            vcdbFilePath = key.GetValue("lastVCdbFilePath", "").ToString();
            if (vcdbFilePath != "")
            {
                using (var md5 = MD5.Create())
                {
                    try
                    {
                        using (var stream = File.OpenRead(vcdbFilePath))
                        {
                            md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                            if (md5Hash == key.GetValue("lastVCdbFileMD5").ToString())
                            {
                                vcdb.clear();
                                vcdb.useRemoteDB = false;
                                vcdb.connectLocalOLEDB(vcdbFilePath);
                                vcdb.importOLEDBdata();
                                aces.logHistoryEvent("", "1\tAuto-loaded local VCdb: " + vcdb.version);
                            }
                            else
                            {// md5hash of the VCdb Access file is different than last time
                                MessageBox.Show("We tried to auto-load the last VCdb file that was used ("+ vcdbFilePath +") but its fingerprint (md5 hash) is different than last time we used it. This indicates that the file was modified. We are taking a defensive posture and NOT auto-loading it.");
                                aces.logHistoryEvent("", "0\tVCdb fingerprint mismatch - did not load");
                            }
                        }
                    }
                    catch (Exception ex)
                    {// most likely a file-not-found error
                        aces.logHistoryEvent("", "0\t"+ex.Message);
                    }
                }
            }
        }


        public async void loadLastLocalPCdb()
        {
            string pcdbFilePath = null;
            string md5Hash;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            pcdbFilePath = key.GetValue("lastPCdbFilePath", "").ToString();
            if (pcdbFilePath != "")
            {
                using (var md5 = MD5.Create())
                {
                    try
                    {
                        using (var stream = File.OpenRead(pcdbFilePath))
                        {
                            md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                            if (md5Hash == key.GetValue("lastPCdbFileMD5").ToString())
                            {
                                pcdb.disconnect();
                                pcdb.clear();
                                pcdb.connectLocalOLEDB(pcdbFilePath);
                                pcdb.importOLEdb();
                                aces.logHistoryEvent("", "1\tAuto-loaded PCdb: " + pcdb.version);
                            }
                            else
                            {
                                MessageBox.Show("We tried to auto-load the last local PCdb Access file that was used (" + pcdbFilePath + ") but its fingerprint (md5 hash) is different than last time we used it. This indicates that the file was modified. We are taking a defensive posture and NOT auto-loading it.");
                                aces.logHistoryEvent("", "0\tPCdb file fingerprint mismatch - did not load");
                            }
                        }
                    }
                    catch (Exception ex)
                    {// most likely a file-not-found error
                        aces.logHistoryEvent("", "0\t"+ex.Message);
                    }
                }
            }
        }

        public async void loadLastLocalQdb()
        {
            string qdbFilePath = null;
            string md5Hash;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            qdbFilePath = key.GetValue("lastQdbFilePath", "").ToString();
            if (qdbFilePath != "")
            {
                using (var md5 = MD5.Create())
                {
                    try
                    {
                        using (var stream = File.OpenRead(qdbFilePath))
                        {
                            md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                            if (md5Hash == key.GetValue("lastQdbFileMD5").ToString())
                            {
                                qdb.disconnect();
                                qdb.clear();
                                qdb.connectLocalOLEDB(qdbFilePath);
                                qdb.importOLEdb();
                                aces.logHistoryEvent("", "1\tAuto-loaded Qdb: " + qdb.version);
                            }
                            else
                            {
                                MessageBox.Show("We tried to auto-load the last Qdb file that was used (" + qdbFilePath + ") but its fingerprint (md5 hash) is different than last time we used it. This indicates that the file was modified. We are taking a defensive posture and NOT auto-loading it.");
                                aces.logHistoryEvent("", "0\tQdb fingerprint mismatch - did not load");
                            }
                        }
                    }
                    catch (Exception ex)
                    {// most likely a file-not-found error
                        aces.logHistoryEvent("", "0\t"+ex.Message);
                    }
                }
            }
        }

        // use the first instance of database connection in the list to stuff the various dictionaries and then disconnect. More connections will be instanced as needed when "Analyze" is clicked
        private async void importMySQLvcdb()
        {
            vcdb.importMySQLdata();
            if(vcdb.importSuccess)
            {
                aces.logHistoryEvent("", "0\tImported VCdb from remote database (version:"+ vcdb.version + ", server thread:"+vcdb.connectionMySQLlist.First().ServerThread.ToString()+")");
            }
            else
            {
                aces.logHistoryEvent("", "0\tImport of remote database failed: (" + vcdb.importExceptionMessage + ")");
            }
            vcdb.disconnect();
        }


        private async void importMySQLpcdb()
        {
            pcdb.importMySQLdata();
            if (pcdb.importSuccess)
            {
                aces.logHistoryEvent("", "0\tImported PCdb from remote database (version:" + pcdb.version + ", server thread:" + pcdb.connectionMySQL.ServerThread.ToString() + ")");
            }
            else
            {
                aces.logHistoryEvent("", "0\tImport of remote PCdb database failed: (" + pcdb.importExceptionMessage + ")");
            }
            pcdb.disconnect();
        }

        private async void importMySQLqdb()
        {
            qdb.importMySQLdata();
            if (qdb.importSuccess)
            {
                aces.logHistoryEvent("", "0\tImported Qdb from remote database (version:" + qdb.version + ", server thread:" + qdb.connectionMySQL.ServerThread.ToString() + ")");
            }
            else
            {
                aces.logHistoryEvent("", "0\tImport of remote Qdb database failed: " + qdb.importExceptionMessage + ")");
            }
            qdb.disconnect();
        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            dgParts.Width = this.Width - 36;
            dgParts.Height = this.Height - 320;
            dgParttypeDisagreement.Width = Width - 36;
            dgParttypeDisagreement.Height = Height - 320;

            dgBasevids.Width = this.Width - 36;
            dgBasevids.Height = this.Height - 320;
            dgVCdbCodes.Width = this.Width - 36;
            dgVCdbCodes.Height = this.Height - 320;
            dgQdbErrors.Width = this.Width - 36;
            dgQdbErrors.Height = this.Height - 320;
            dgParttypePosition.Width = this.Width - 36;
            dgParttypePosition.Height = this.Height - 320;
            dgVCdbConfigs.Width = this.Width - 36;
            dgVCdbConfigs.Height = this.Height - 320;
            dgAddsDropsParts.Width = this.Width - 36;
            dgAddsDropsParts.Height = this.Height - 320;
            dgAddsDropsVehicles.Width = this.Width - 36;
            dgAddsDropsVehicles.Height = this.Height - 320;
            dgQuantityWarnings.Width = this.Width - 36;
            dgQuantityWarnings.Height = this.Height - 320;

            splitContainerFitmentLogic.Width = this.Width - 14;
            splitContainerFitmentLogic.Height = this.Height - 320;

            //pictureBoxFitmentTree.Width = this.Width-236;
            pictureBoxFitmentTree.Width = this.Width - 36;
            

            listBoxFitmentLogicElements.Left = this.Width - 229;
            dgFitmentLogicProblems.Width = this.Width-36;

            tabControl1.Width = this.Width - 18;
            tabControl1.Height = this.Height - 288;
            lblAppVersion.Left = this.Width - 85;

            textBoxAnalysisHostory.Width = this.Width - 615;
            textBoxAnalysisHostory.Height = this.Height - 321;
        }

        void ReportAnalyzeProgressDifferentials(int value)
        {
            progressBarDifferentials.Value = value;
        }

        void ReportFindHoles(int value)
        {
            progressBarDifferentials.Value = value;
        }



        private async void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (lblCachePath.Text == "")
            {
                MessageBox.Show("Please select a folder where ACESinspector can store cache files. This can be done in the Settings tab");
                return;
            }

            if (lblAssessmentsPath.Text == "")
            {
                MessageBox.Show("Please select a folder where ACESinspector can write assessment files. This can be done in the Settings tab");
                return;
            }



            aces.logHistoryEvent("", "5\tAnalysis started");

            btnAnalyze.Enabled = false;
            btnSelectACESfile.Enabled = false;
            btnSelectPartInterchange.Enabled = false;
            btnSelectReferenceACESfile.Enabled = false;
            btnSelectVCdbFile.Enabled = false;
            btnSelectPCdbFile.Enabled = false;
            btnSelectQdbFile.Enabled = false;
            progressBarDifferentials.Value = 0;
            string problemDescription = "";
            int elementPrevalence = 0;
            Dictionary<string, int> fitmentElementPrevalence = new Dictionary<string, int>();

            aces.logHistoryEvent("", "10\tbtnSelectACESfile - populating parts tab");
            
            // populate the "Parts" tab with distinct parts and their positions, types and app counts
            List<string> partTypeNameList = new List<string>();
            List<string> positionNameList = new List<string>();
            if (aces.partsAppCounts.Count() < largeDatagridRecordThreshold)
            {
                foreach (KeyValuePair<string, int> partEntry in aces.partsAppCounts)
                {
                    partTypeNameList.Clear(); foreach (int partTypeId in aces.partsPartTypes[partEntry.Key]) { partTypeNameList.Add(pcdb.niceParttype(partTypeId)); }
                    positionNameList.Clear(); foreach (int positionId in aces.partsPositions[partEntry.Key]) { positionNameList.Add(pcdb.nicePosition(positionId)); }
                    dgParts.Rows.Add(partEntry.Key, Convert.ToInt32(aces.partsAppCounts[partEntry.Key].ToString()), string.Join(",", partTypeNameList), string.Join(",", positionNameList));
                }
                dgParts.Visible = true;
            }
            else
            {// our part count is above "large" datagrid capacity threshold - 
                if (checkBoxLimitDataGridRows.Checked)
                {// so the "see assessment file for your stuff" message
                    lblPartsTabRedirect.Visible = true;
                    lblPartsTabRedirect.Text = "Parts list is too large for display here (" + aces.partsAppCounts.Count() + " items). Assessment file contains the full list.";
                    dgParts.Visible = false;
                }
                else
                {// go ahead and populate the datagrid - against your doctor's advice
                    foreach (KeyValuePair<string, int> partEntry in aces.partsAppCounts)
                    {
                        partTypeNameList.Clear(); foreach (int partTypeId in aces.partsPartTypes[partEntry.Key]) { partTypeNameList.Add(pcdb.niceParttype(partTypeId)); }
                        positionNameList.Clear(); foreach (int positionId in aces.partsPositions[partEntry.Key]) { positionNameList.Add(pcdb.nicePosition(positionId)); }
                        dgParts.Rows.Add(partEntry.Key, Convert.ToInt32(aces.partsAppCounts[partEntry.Key].ToString()), string.Join(",", partTypeNameList), string.Join(",", positionNameList));
                    }
                    dgParts.Visible = true;
                }
            }
            tabControl1.TabPages.Add(hiddenPartsTab); highestVisableTab1Index++; // make visible the "parts" tab page

            lblStatsVCdbVersion.Text = aces.VcdbVersionDate; lblStatsVCdbVersion.ForeColor = SystemColors.ControlText;
            lblStatsPCdbVersion.Text = aces.PcdbVersionDate; lblStatsPCdbVersion.ForeColor = SystemColors.ControlText;
            lblStatsQdbVersion.Text = aces.QdbVersionDate; lblStatsQdbVersion.ForeColor = SystemColors.ControlText;

            lblMacroProblems.Text = "";
            lblDifferentialsSummary.Text = "";
            lblIndividualErrors.Text = "";
            lblStatus.BackColor = SystemColors.ButtonFace;

            dgBasevids.Rows.Clear();
            dgVCdbCodes.Rows.Clear();
            dgParttypePosition.Rows.Clear();
            dgVCdbConfigs.Rows.Clear();
            dgQdbErrors.Rows.Clear();
            dgAddsDropsParts.Rows.Clear();
            dgAddsDropsVehicles.Rows.Clear();
            dgQuantityWarnings.Rows.Clear();


            lblIndividualErrorsTitle.Visible = true;
            lblMacroProblemsTitle.Visible = true;
            pictureBoxLogicProblems.Visible = true;
            pictureBoxCommonErrors.Visible = true;

            aces.clearAnalysisResults();
            var taskList = new List<Task>();


            var progressDifferentials = new Progress<int>(ReportAnalyzeProgressDifferentials);
            if (refaces.apps.Count > 0)
            { 
                lblDifferentialsSummary.Text = "(analyzing)"; progressBarDifferentials.Visible = true;
                var differentialsAnalysisTask = new Task(() => { diffaces.findDifferentials(aces, refaces, vcdb, pcdb, progressDifferentials); });
                taskList.Add(differentialsAnalysisTask);
                differentialsAnalysisTask.Start();

            }


            aces.qtyOutlierThreshold = numericUpDownQtyOutliersThreshold.Value;
            aces.qtyOutlierSampleSize = numericUpDownQtyOutliersSample.Value;

            aces.establishFitmentTreeRoots(); // maybe should move this to be the last step in the import process. It is not threadable, so it must be run in a blocking way before the threadable stuff is run

            int numberOfSections =Convert.ToInt32(numericUpDownThreads.Value);
            if((numberOfSections*5) > aces.apps.Count()){numberOfSections = 1; } // ensure at least 5apps per sections or dont break up


            int sectionSize = Convert.ToInt32(aces.apps.Count() / numberOfSections);
            //int remainder = aces.apps.Count() - (sectionCount * sectionSize);
            int sectionNumber = 1;

            aces.individualAnanlysisChunksList.Add(new analysisChunk());
            aces.individualAnanlysisChunksList.Last().appsList = new List<App>();
            aces.individualAnanlysisChunksList.Last().id = 1; aces.individualAnanlysisChunksList.Last().cachefile=lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash;

            cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_parttypePositionErrors1.txt");
            cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_QdbErrors1.txt");
            cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_questionableNotes1.txt");
            cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidBasevehicles1.txt");
            cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidVCdbCodes1.txt");
            cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_configurationErrors1.txt");

            for (int i=0; i<aces.apps.Count();i++)
            {
                if (aces.individualAnanlysisChunksList.Last().appsList.Count() >= sectionSize)
                {
                    sectionNumber++;
                    aces.individualAnanlysisChunksList.Add(new analysisChunk());
                    aces.individualAnanlysisChunksList.Last().id = sectionNumber;
                    aces.individualAnanlysisChunksList.Last().cachefile = lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash;
                    aces.individualAnanlysisChunksList.Last().appsList = new List<App>();
                    cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_parttypePositionErrors" + sectionNumber.ToString() + ".txt");
                    cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_QdbErrors" + sectionNumber.ToString() + ".txt");
                    cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_questionableNotes" + sectionNumber.ToString() + ".txt");
                    cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidBasevehicles" + sectionNumber.ToString() + ".txt");
                    cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidVCdbCodes" + sectionNumber.ToString() + ".txt");
                    cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_configurationErrors" + sectionNumber.ToString() + ".txt");
                }
                aces.individualAnanlysisChunksList.Last().appsList.Add(aces.apps[i]);
            }

            // run in a parallel (an arbitrary number a chunks) the individaul analysis of the primary ACES apps list
           
            //if (vcdb.useRemoteDB) { foreach (analysisChunk chunk in aces.individualAnanlysisChunksList) { vcdb.addNewMySQLconnection(); } } // instance one mysql connection for each chunk
            var individialAppAnalysisTask = new Task(() => { Parallel.ForEach(aces.individualAnanlysisChunksList, chunk => { aces.findIndividualAppErrors(chunk, vcdb,pcdb, qdb); }); });
            taskList.Add(individialAppAnalysisTask);
            individialAppAnalysisTask.Start();

            //run a single, sequential thread looking for outlier-ness in the while apps list (qty outliers and part-type/position disagreement)
            // this can be run concurently with the other analysis threads, it just can't be broken into its own concurent chunks
            aces.outlierAnanlysisChunksList.Add(new analysisChunk());
            aces.outlierAnanlysisChunksList.Last().cachefile = lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash;
            aces.outlierAnanlysisChunksList.Last().appsList = aces.apps;
            cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_parttypeDisagreements.txt");
            cacheFilesToDeleteOnExit.Add(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_qtyOutliers.txt");

            var outlierAppAnalysisTask = new Task(() => { aces.findIndividualAppOutliers(aces.outlierAnanlysisChunksList.Last(),vcdb, pcdb, qdb); });
            taskList.Add(outlierAppAnalysisTask);
            outlierAppAnalysisTask.Start();


            numberOfSections = Convert.ToInt32(numericUpDownThreads.Value);
            if ((numberOfSections * 5) > aces.fitmentAnalysisChunksList.Count()) { numberOfSections = 1; } // ensure at least 5 chunkgroups per sections or dont break up

            sectionSize = Convert.ToInt32(aces.fitmentAnalysisChunksList.Count() / numberOfSections);


            sectionNumber = 1;
            aces.fitmentAnalysisChunksGroups.Add(new analysisChunkGroup());
            aces.fitmentAnalysisChunksGroups.Last().chunkList = new List<analysisChunk>();
            aces.fitmentAnalysisChunksGroups.Last().id = sectionNumber;

            for (int i=0; i< aces.fitmentAnalysisChunksList.Count();i++)
            {// every chunk represents the apps in one fitment tree (MMY/parttype/position/mfrlabel/asset)
                // chunkgroups represent an arbitrary-sized collection of chunks for the purpose for parrallel multi-tasking (each group is a new task)
                if (aces.fitmentAnalysisChunksGroups.Last().chunkList.Count() >= sectionSize)
                {
                    sectionNumber++;
                    aces.fitmentAnalysisChunksGroups.Add(new analysisChunkGroup());
                    aces.fitmentAnalysisChunksGroups.Last().chunkList = new List<analysisChunk>();
                    aces.fitmentAnalysisChunksGroups.Last().id = sectionNumber;
                }
                aces.fitmentAnalysisChunksGroups.Last().chunkList.Add(aces.fitmentAnalysisChunksList[i]);
            }

            var macroAppAnalysisTask = new Task(() => { Parallel.ForEach(aces.fitmentAnalysisChunksGroups, chunkGroup => {aces.findFitmentLogicProblems(chunkGroup, vcdb, pcdb, qdb, lblCachePath.Text + @"\ACESinspector-fitment permutations.txt", Convert.ToInt32(numericUpDownTreeConfigLimit.Value), lblCachePath.Text, checkBoxConcernForDisparate.Checked);}); });
            taskList.Add(macroAppAnalysisTask);
            macroAppAnalysisTask.Start();

            aces.analysisRunning = true;

            await Task.WhenAll(taskList.ToArray());

            aces.analysisRunning = false;



            //-------------------------------------------- all analysis is done and recorded at this point (all tasks terminated) -------------------------------------------------
            // compile the total warning and errors counts 
            aces.parttypePositionErrorsCount = 0; aces.qdbErrorsCount = 0; aces.questionableNotesCount = 0; aces.basevehicleidsErrorsCount = 0; aces.vcdbCodesErrorsCount = 0; aces.vcdbConfigurationsErrorsCount = 0; aces.parttypeDisagreementCount = 0; aces.qtyOutlierCount = 0; 
            foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
            {
                aces.parttypePositionErrorsCount +=chunk.parttypePositionErrorsCount;
                aces.qdbErrorsCount += chunk.qdbErrorsCount;
                aces.questionableNotesCount += chunk.questionableNotesCount;
                aces.basevehicleidsErrorsCount += chunk.basevehicleidsErrorsCount;
                aces.vcdbCodesErrorsCount +=chunk.vcdbCodesErrorsCount;
                aces.vcdbConfigurationsErrorsCount += chunk.vcdbConfigurationsErrorsCount;
            }

            foreach (analysisChunk chunk in aces.outlierAnanlysisChunksList)
            {// should only be one item in list 
                aces.parttypeDisagreementCount += chunk.parttypeDisagreementErrorsCount;
                aces.qtyOutlierCount += chunk.qtyOutlierCount;
            }



            aces.fitmentLogicProblemsCount = 0;
            int problemGroupNumber = 0;

            for(int i=0; i< aces.fitmentAnalysisChunksGroups.Count;i++)
            {
                for(int j=0; j< aces.fitmentAnalysisChunksGroups[i].chunkList.Count; j++)
                {
                    if(aces.fitmentAnalysisChunksGroups[i].chunkList[j].problemAppsList.Count >0)
                    {
                        aces.fitmentLogicProblemsCount += aces.fitmentAnalysisChunksGroups[i].chunkList[j].problemAppsList.Count;
                        problemGroupNumber++;
                        if (checkBoxReportAllAppsInProblemGroup.Checked)
                        {
                            aces.fitmentProblemGroupsAppLists.Add(problemGroupNumber.ToString(), aces.fitmentAnalysisChunksGroups[i].chunkList[j].appsList);
                        }
                        else
                        {
                            aces.fitmentProblemGroupsAppLists.Add(problemGroupNumber.ToString(), aces.fitmentAnalysisChunksGroups[i].chunkList[j].problemAppsList);
                        }
                        aces.fitmentProblemGroupsBestPermutations.Add(problemGroupNumber.ToString(), aces.fitmentAnalysisChunksGroups[i].chunkList[j].lowestBadnessPermutation);
                    }
                }
            }

            lblIndividualErrors.Text = (aces.basevehicleidsErrorsCount + aces.vcdbCodesErrorsCount + aces.vcdbConfigurationsErrorsCount + aces.qdbErrorsCount + aces.parttypePositionErrorsCount).ToString() + " errors";

            List<string> problemsListTemp = new List<string>();
            if (aces.fitmentLogicProblemsCount > 0) { problemsListTemp.Add(aces.fitmentLogicProblemsCount.ToString() + " logic flaws"); }
            if (aces.qtyOutlierCount > 0) { problemsListTemp.Add(aces.qtyOutlierCount.ToString() + " qty outliers"); }
            if (aces.parttypeDisagreementCount > 0) { problemsListTemp.Add(aces.parttypeDisagreementCount.ToString() + " type disagreements"); }
            if (problemsListTemp.Count() == 0) { lblMacroProblems.Text = "0 problems"; } else { lblMacroProblems.Text = string.Join(", ", problemsListTemp); }
            

            // start building the assessment file

            string validatedAgainstVCdb = ""; if (aces.VcdbVersionDate != vcdb.version) { validatedAgainstVCdb = "validated against version " + vcdb.version; }
            string validatedAgainstPCdb = ""; if (aces.PcdbVersionDate != pcdb.version) { validatedAgainstPCdb = "validated against version " + pcdb.version; }
            string validatedAgainstQdb = ""; if (aces.QdbVersionDate != qdb.version) { validatedAgainstQdb = "validated against version " + qdb.version; }
            string excelTabColorXMLtag = "";

            string assessmentFilename = Path.GetDirectoryName(aces.filePath) + "\\" + Path.GetFileNameWithoutExtension(aces.filePath) + "_assessment.xml";
            if (lblAssessmentsPath.Text != "---")
            {
                assessmentFilename = lblAssessmentsPath.Text + "\\" + Path.GetFileNameWithoutExtension(aces.filePath) + "_assessment.xml";
            }

            try
            {

                using (StreamWriter sw = new StreamWriter(assessmentFilename))
                {
                    sw.Write("<?xml version=\"1.0\"?><?mso-application progid=\"Excel.Sheet\"?><Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:html=\"http://www.w3.org/TR/REC-html40\"><DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\"><Author>ACESinspector</Author><LastAuthor>ACESinspector</LastAuthor><Created>2017-02-20T01:10:23Z</Created><LastSaved>2017-02-20T02:49:36Z</LastSaved><Version>14.00</Version></DocumentProperties><OfficeDocumentSettings xmlns=\"urn:schemas-microsoft-com:office:office\"><AllowPNG/></OfficeDocumentSettings><ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\"><WindowHeight>7500</WindowHeight><WindowWidth>15315</WindowWidth><WindowTopX>120</WindowTopX><WindowTopY>150</WindowTopY><TabRatio>785</TabRatio><ProtectStructure>False</ProtectStructure><ProtectWindows>False</ProtectWindows></ExcelWorkbook><Styles><Style ss:ID=\"Default\" ss:Name=\"Normal\"><Alignment ss:Vertical=\"Bottom\"/><Borders/><Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#000000\"/><Interior/><NumberFormat/><Protection/></Style><Style ss:ID=\"s62\"><NumberFormat ss:Format=\"Short Date\"/></Style><Style ss:ID=\"s64\" ss:Name=\"Hyperlink\"><Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#0000FF\" ss:Underline=\"Single\"/></Style><Style ss:ID=\"s65\"><Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#000000\" ss:Bold=\"1\"/><Interior ss:Color=\"#D9D9D9\" ss:Pattern=\"Solid\"/></Style></Styles><Worksheet ss:Name=\"Stats\"><Table ss:ExpandedColumnCount=\"3\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"116.25\"/><Column ss:Width=\"225\"/><Column ss:Width=\"225\"/>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">Input Filename</Data></Cell><Cell><Data ss:Type=\"String\">" + Path.GetFileName(aces.filePath) + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">Title</Data></Cell><Cell><Data ss:Type=\"String\">" + aces.DocumentTitle + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">ACES version</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">" + aces.version + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">VCdb version cited</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">" + aces.VcdbVersionDate + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">" + validatedAgainstVCdb + "</Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">PCdb version cited</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">" + aces.PcdbVersionDate + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">" + validatedAgainstPCdb + "</Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">Qdb version cited</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">" + aces.QdbVersionDate + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">" + validatedAgainstQdb + "</Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">Application count</Data></Cell><Cell><Data ss:Type=\"Number\">" + aces.apps.Count.ToString() + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">Unique Part count</Data></Cell><Cell><Data ss:Type=\"Number\">" + aces.partsAppCounts.Count.ToString() + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">Unique MfrLabel count</Data></Cell><Cell><Data ss:Type=\"Number\">" + aces.distinctMfrLabels.Count.ToString() + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">Unique Parttypes count</Data></Cell><Cell><Data ss:Type=\"Number\">" + aces.distinctPartTypes.Count.ToString() + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>");
                    sw.Write("<Row><Cell><Data ss:Type=\"String\">Validation tool</Data></Cell><Cell ss:StyleID=\"s64\" ss:HRef=\"https://autopartsource.com/ACESinspector\"><Data ss:Type=\"String\">ACESinspector version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "</Data></Cell><Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row>");
                    sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup><Selected/><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");

                    sw.Write("<Worksheet ss:Name=\"Parts\"><Table ss:ExpandedColumnCount=\"4\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Applications Count</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Types</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Positions</Data></Cell></Row>");
                    foreach (KeyValuePair<string, int> partsAppCountEntry in aces.partsAppCounts)
                    {
                        partTypeNameList.Clear(); foreach (int partTypeId in aces.partsPartTypes[partsAppCountEntry.Key]) { partTypeNameList.Add(pcdb.niceParttype(partTypeId)); }
                        positionNameList.Clear(); foreach (int positionId in aces.partsPositions[partsAppCountEntry.Key]) { positionNameList.Add(pcdb.nicePosition(positionId)); }
                        sw.Write("<Row><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(partsAppCountEntry.Key) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + partsAppCountEntry.Value.ToString() + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(string.Join(",", partTypeNameList)) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(String.Join(",", positionNameList)) + "</Data></Cell></Row>");
                    }
                    sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup><FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");

                    sw.Write("<Worksheet ss:Name=\"Part Types\"><Table ss:ExpandedColumnCount=\"2\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Index=\"2\" ss:AutoFitWidth=\"0\" ss:Width=\"183.75\"/>");
                    foreach (int distinctPartType in aces.distinctPartTypes) { sw.Write("<Row><Cell><Data ss:Type=\"Number\">" + distinctPartType + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(pcdb.niceParttype(distinctPartType)) + "</Data></Cell></Row>"); }
                    sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");

                    if (aces.distinctMfrLabels.Count > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"MfrLabels\"><Table ss:ExpandedColumnCount=\"1\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:AutoFitWidth=\"0\" ss:Width=\"151.5\"/>");
                        foreach (string distinctMfrLabel in aces.distinctMfrLabels) { sw.Write("<Row><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(distinctMfrLabel) + "</Data></Cell></Row>"); }
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }

                    if (aces.parttypeDisagreementCount > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"Parttype Disagreement\"><Table ss:ExpandedColumnCount=\"2\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:AutoFitWidth=\"0\" ss:Width=\"45\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"78.75\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Parttypes</Data></Cell></Row>");
                        using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_parttypeDisagreements.txt"))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine(); string[] fileds = line.Split('\t');
                                sw.Write("<Row><Cell><Data ss:Type=\"String\">" + fileds[0] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + fileds[1] + "</Data></Cell></Row>");
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>13</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }


                    if (aces.qtyOutlierCount > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"Qty Outliers\"><Table ss:ExpandedColumnCount=\"13\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"180\"/><Column ss:Width=\"36\"/><Column ss:Width=\"77\"/><Column ss:Width=\"120\"/><Column ss:Width=\"33\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"46\"/><Column ss:Width=\"120\"/><Column ss:Width=\"180\"/><Column ss:Width=\"180\"/><Column ss:Width=\"180\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Error Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Base Vehicle Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Make</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Model</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Year</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Quantity</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">VCdb-coded attributes</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Qdb-coded qualifiers</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Notes</Data></Cell></Row>");
                        using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_qtyOutliers.txt"))
                        {
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine(); string[] fileds = line.Split('\t');
                                sw.Write("<Row><Cell><Data ss:Type=\"String\">" + fileds[0] + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[1] + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[2] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[3]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[4]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[5]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[6]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[7]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[8] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[9]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[10]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[11]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[12]) + "</Data></Cell></Row>");
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>13</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }

                    if (aces.parttypePositionErrorsCount > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"PartType-Position Errors\"><Table ss:ExpandedColumnCount=\"11\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"115\"/><Column ss:Width=\"36\"/><Column ss:Width=\"77\"/><Column ss:Width=\"120\"/><Column ss:Width=\"33\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"46\"/><Column ss:Width=\"120\"/><Column ss:Width=\"180\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Error Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Base Vehicle Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Make</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Model</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Year</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Quantity</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Fitment</Data></Cell></Row>");
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.parttypePositionErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_parttypePositionErrors" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            string line = reader.ReadLine(); string[] fileds = line.Split('\t');
                                            sw.Write("<Row><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[0]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[1] + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[2] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[3]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[4]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[5]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[6]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[7]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[8] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[9]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[10]) + "</Data></Cell></Row>");
                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>10</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }

                    if (aces.qdbErrorsCount > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"Qdb Errors\"><Table ss:ExpandedColumnCount=\"12\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"115\"/><Column ss:Width=\"36\"/><Column ss:Width=\"77\"/><Column ss:Width=\"120\"/><Column ss:Width=\"33\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"46\"/><Column ss:Width=\"120\"/><Column ss:Width=\"180\"/><Column ss:Width=\"180\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Error Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Base Vehicle Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Make</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Model</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Year</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Quantity</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">VCdb-coded attributes</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Notes</Data></Cell></Row>");
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.qdbErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_qdbErrors" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            string line = reader.ReadLine(); string[] fileds = line.Split('\t');
                                            sw.Write("<Row><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[0]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[1] + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[2] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[3]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[4]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[5]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[6]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[7]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[8] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[9]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[10]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[11]) + "</Data></Cell></Row>");
                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>10</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }


                    if (aces.questionableNotesCount > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"Questionable Notes\"><Table ss:ExpandedColumnCount=\"12\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"115\"/><Column ss:Width=\"36\"/><Column ss:Width=\"77\"/><Column ss:Width=\"120\"/><Column ss:Width=\"33\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"120\"/><Column ss:Width=\"46\"/><Column ss:Width=\"120\"/><Column ss:Width=\"180\"/><Column ss:Width=\"180\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Error Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Base Vehicle Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Make</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Model</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Year</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Quantity</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">VCdb-coded attributes</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Notes</Data></Cell></Row>");
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.questionableNotesCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_questionableNotes" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            string line = reader.ReadLine(); string[] fileds = line.Split('\t');
                                            sw.Write("<Row><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[0]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[1] + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[2] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[3]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[4]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[5]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[6]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[7]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[8] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[9]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[10]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[11]) + "</Data></Cell></Row>");
                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>13</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }



                    if (aces.basevehicleidsErrorsCount > 0)
                    {
                        sw.Write("<Worksheet ss:Name =\"Invalid Base Vids\"><Table ss:ExpandedColumnCount=\"7\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:AutoFitWidth=\"0\" ss:Width=\"45\"/><Column ss:Width=\"77.25\"/><Column ss:Index=\"4\" ss:AutoFitWidth=\"0\" ss:Width=\"96\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"73.5\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"253.5\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"371.25\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Invalid BaseVid</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Quantity</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Fitment</Data></Cell></Row>");
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.basevehicleidsErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidBasevehicles" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            string line = reader.ReadLine(); string[] fileds = line.Split('\t');
                                            sw.Write("<Row><Cell><Data ss:Type=\"Number\">" + fileds[0] + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[1] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[2]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[3]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[4] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[5]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[6]) + "</Data></Cell></Row>");
                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>10</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup><Print><ValidPrinterInfo/><HorizontalResolution>600</HorizontalResolution><VerticalResolution>600</VerticalResolution></Print>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }

                    if (aces.vcdbCodesErrorsCount > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"Invalid VCdb Codes\"><Table ss:ExpandedColumnCount=\"10\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:AutoFitWidth=\"0\" ss:Width=\"45\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"78.75\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"99.75\"/><Column ss:Width=\"31.5\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"60\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"112.5\"/><Column ss:Width=\"43.5\"/><Column ss:Width =\"43.5\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"237\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"319.5\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Make</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Model</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Year</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Quantity</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">VCdb Attributes</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Notes</Data></Cell></Row>");
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.vcdbCodesErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidVCdbCodes" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            string line = reader.ReadLine(); string[] fileds = line.Split('\t');
                                            sw.Write("<Row><Cell><Data ss:Type=\"Number\">" + fileds[0] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[1]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[2]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[3]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[4]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[5]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[6] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[7]) + "</Data></Cell><Cell><Data ss:Type =\"String\">" + escapeXMLspecialChars(fileds[8]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[9]) + "</Data></Cell></Row>");
                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>10</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }


                    if (aces.vcdbConfigurationsErrorsCount > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"Invalid VCdb Configs\"><Table ss:ExpandedColumnCount=\"12\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:AutoFitWidth=\"0\" ss:Width=\"45\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"45\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"78.75\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"99.75\"/><Column ss:Width=\"31.5\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"60\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"112.5\"/><Column ss:Width=\"43.5\"/><Column ss:Width =\"43.5\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"237\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"319.5\"/><Column ss:AutoFitWidth=\"0\" ss:Width=\"319.5\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Base Vehiclce id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Make</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Model</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Year</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Quantity</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">VCdb-coded Attributes</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Qdb-coded Qualifiers</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Notes</Data></Cell></Row>");
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.vcdbConfigurationsErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_configurationErrors" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            string line = reader.ReadLine(); string[] fileds = line.Split('\t');
                                            sw.Write("<Row><Cell><Data ss:Type=\"Number\">" + fileds[0] + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[1] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[2]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[3]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[4]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[5]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[6]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + fileds[7] + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[8]) + "</Data></Cell><Cell><Data ss:Type =\"String\">" + escapeXMLspecialChars(fileds[9]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[10]) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fileds[11]) + "</Data></Cell></Row>");
                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>10</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }


                    if (aces.fitmentLogicProblemsCount > 0)
                    {
                        problemDescription = "";
                        elementPrevalence = 0;
                        fitmentElementPrevalence.Clear();

                        sw.Write("<Worksheet ss:Name=\"Fitment Logic Problems\"><Table ss:ExpandedColumnCount=\"12\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Problem Description</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Group</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">App Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">BaseVehcile Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Make</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Model</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Year</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Quantity</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Fitment</Data></Cell></Row>");
                        foreach (KeyValuePair<string, List<App>> entry in aces.fitmentProblemGroupsAppLists)
                        {
                            // construct a tree in order to re-discover the problmes with it.
                            aces.fitmentNodeList.Clear();
                            fitmentElementPrevalence.Clear();
                            foreach (string fitmentElement in aces.fitmentProblemGroupsBestPermutations[entry.Key])
                            {
                                fitmentElementPrevalence.Add(fitmentElement, elementPrevalence); elementPrevalence++;
                            }
                            aces.fitmentNodeList.AddRange(aces.buildFitmentTreeFromAppList(entry.Value, fitmentElementPrevalence, -1, false,false, vcdb, qdb));
                            problemDescription=aces.fitmentTreeProblemDescription(aces.fitmentNodeList, checkBoxConcernForDisparate.Checked);

                            foreach (App app in entry.Value)
                            {
                                sw.Write("<Row><Cell><Data ss:Type=\"String\">"+ problemDescription + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(entry.Key) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + app.id.ToString() + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + app.basevehilceid.ToString() + "</Data></Cell><Cell><Data ss:Type=\"String\">" + vcdb.niceMakeOfBasevid(app.basevehilceid) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(vcdb.niceModelOfBasevid(app.basevehilceid)) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + vcdb.niceYearOfBasevid(app.basevehilceid) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(pcdb.niceParttype(app.parttypeid)) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(pcdb.nicePosition(app.positionid)) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + app.quantity.ToString() + "</Data></Cell><Cell><Data ss:Type=\"String\">" + app.part + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(app.niceFullFitmentString(vcdb, qdb)) + "</Data></Cell></Row>");
                            }
                        }
                        excelTabColorXMLtag = "<TabColorIndex>10</TabColorIndex>";
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup>" + excelTabColorXMLtag + "<FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }

                    if (diffaces.differentialParts.Count > 0)
                    {
                        sw.Write("<Worksheet ss:Name=\"Differential Parts\"><Table ss:ExpandedColumnCount=\"2\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Add/Drop</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part</Data></Cell></Row>");
                        foreach (string line in diffaces.differentialParts)
                        {
                            string[] fields = line.Split('\t');
                            sw.Write("<Row><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fields[0])+ "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fields[1]) + "</Data></Cell></Row>");
                        }
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup><FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }


                    if (diffaces.differentialVehicles.Count > 0)
                    {
                        App tempApp = new App();

                        sw.Write("<Worksheet ss:Name=\"Differential Vehicles\"><Table ss:ExpandedColumnCount=\"9\" x:FullColumns=\"1\" x:FullRows=\"1\" ss:DefaultRowHeight=\"15\"><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Column ss:Width=\"100\"/><Row><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Add/Drop</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">BaseVehcile Id</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Make</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Model</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Year</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Part Type</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Position</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Fitment</Data></Cell><Cell ss:StyleID=\"s65\"><Data ss:Type=\"String\">Mfr Label</Data></Cell></Row>");
                        foreach (string line in diffaces.differentialVehicles)
                        {
                            string[] fields = line.Split('\t');
                            tempApp.Clear();
                            tempApp.basevehilceid = Convert.ToInt32(fields[1]);
                            if(vcdb.niceMakeOfBasevid(tempApp.basevehilceid)== "not found"){continue;}
                            tempApp.parttypeid = Convert.ToInt32(fields[2]);
                            tempApp.positionid = Convert.ToInt32(fields[3]);
                            if (fields[5] != "") { tempApp.VCdbAttributes = aces.parseAttributePairsString(fields[4]); }
                            tempApp.notes = fields[5].Split(';').ToList();
                            tempApp.mfrlabel = fields[6];
                            sw.Write("<Row><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(fields[0]) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + tempApp.basevehilceid.ToString() + "</Data></Cell><Cell><Data ss:Type=\"String\">" + vcdb.niceMakeOfBasevid(tempApp.basevehilceid) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(vcdb.niceModelOfBasevid(tempApp.basevehilceid)) + "</Data></Cell><Cell><Data ss:Type=\"Number\">" + vcdb.niceYearOfBasevid(tempApp.basevehilceid) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(pcdb.niceParttype(tempApp.parttypeid)) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(pcdb.nicePosition(tempApp.positionid)) + "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(tempApp.niceAttributesString(vcdb, true))+ "</Data></Cell><Cell><Data ss:Type=\"String\">" + escapeXMLspecialChars(tempApp.mfrlabel) + "</Data></Cell></Row>");
                        }
                        sw.Write("</Table><WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><PageSetup><Header x:Margin=\"0.3\"/><Footer x:Margin=\"0.3\"/><PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/></PageSetup><FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane><ActivePane>2</ActivePane><Panes><Pane><Number>3</Number></Pane><Pane><Number>2</Number><ActiveRow>0</ActiveRow></Pane></Panes><ProtectObjects>False</ProtectObjects><ProtectScenarios>False</ProtectScenarios></WorksheetOptions></Worksheet>");
                    }
                    sw.Write("</Workbook>");
                }
                aces.logHistoryEvent("", "0\tAssessment file created (" + assessmentFilename + ")");
            }
            catch(Exception ex)
            {
                aces.logHistoryEvent("", "0\tAssessment file NOT created: " +ex.Message);
            }



            if (aces.parttypeDisagreementCount > 0)
            {
                tabControl1.TabPages.Add(hiddenPartTypeDisagreementTab); highestVisableTab1Index++; // make visible the "part-type disagreement" page
                dgParttypeDisagreement.Visible = true;

                try
                {
                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_parttypeDisagreements.txt"))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            dgParttypeDisagreement.Rows.Add(line.Split('\t'));
                        }
                    }
                }
                catch(Exception ex) { }
            }

            if (aces.qtyOutlierCount > 0)
            {    
                tabControl1.TabPages.Add(hiddenQuantityWarningsTab);
                highestVisableTab1Index++;
                if (aces.qtyOutlierCount > largeDatagridRecordThreshold)
                {
                    if (checkBoxLimitDataGridRows.Checked)
                    {
                        lblQtyWarningsRedirect.Visible = true;
                        lblQtyWarningsRedirect.Text = "Qty outlier warnings are too numerous (" + aces.qtyOutlierCount.ToString() + ") to display here. See assessment file for full list.";
                    }
                    else
                    {
                        dgQuantityWarnings.Visible = true;
                        try
                        {
                            using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_qtyOutliers.txt"))
                            {
                                while (!reader.EndOfStream)
                                {
                                    var line = reader.ReadLine();
                                    dgQuantityWarnings.Rows.Add(line.Split('\t'));
                                }
                            }
                        }catch(Exception ex) { }
                    }
                }
                else
                {
                    dgQuantityWarnings.Visible = true;
                    try
                    {
                        using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_qtyOutliers.txt"))
                        {
                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                dgQuantityWarnings.Rows.Add(line.Split('\t'));
                            }
                        }
                    }catch(Exception ex) { }
                }
            }

            if (aces.parttypePositionErrorsCount > 0)
            {
                tabControl1.TabPages.Add(hiddenParttypePositionErrorsTab); highestVisableTab1Index++;
                if (aces.parttypePositionErrorsCount > largeDatagridRecordThreshold)
                {
                    if (checkBoxLimitDataGridRows.Checked)
                    {
                        lblParttypePositionRedirect.Visible = true;
                        lblParttypePositionRedirect.Text = "Parttypes/Positions errors are too numerous (" + aces.parttypePositionErrorsCount.ToString() + ") to display here. See assessment file for full list.";
                    }
                    else
                    {
                        dgParttypePosition.Visible = true;
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.parttypePositionErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_parttypePositionErrors" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            var line = reader.ReadLine();
                                            dgParttypePosition.Rows.Add(line.Split('\t'));
                                        }
                                    }
                                }catch(Exception ex) { }
                            }
                        }
                    }
                }
                else
                {
                    dgParttypePosition.Visible = true;
                    foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                    {
                        if (chunk.parttypePositionErrorsCount > 0)
                        {
                            try
                            {
                                using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_parttypePositionErrors" + chunk.id.ToString() + ".txt"))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var line = reader.ReadLine();
                                        dgParttypePosition.Rows.Add(line.Split('\t'));
                                    }
                                }
                            }
                            catch(Exception ex) { }
                        }
                    }
                }
            }

            if (aces.qdbErrorsCount > 0)
            {
                tabControl1.TabPages.Add(hiddenQdbErrorsTab); highestVisableTab1Index++;
                if (aces.qdbErrorsCount > largeDatagridRecordThreshold)
                {
                    if (checkBoxLimitDataGridRows.Checked)
                    {
                        lblQdbErrorsRedirect.Visible = true;
                        lblQdbErrorsRedirect.Text = "Qdb errors are too numerous (" + aces.qdbErrorsCount.ToString() + ") to display here. See assessment file for full list.";
                    }
                    else
                    {
                        dgQdbErrors.Visible = true;
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.qdbErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_qdbErrors" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            var line = reader.ReadLine();
                                            dgQdbErrors.Rows.Add(line.Split('\t'));
                                        }
                                    }
                                }catch(Exception ex) { }
                            }
                        }
                    }
                }
                else
                {
                    dgQdbErrors.Visible = true;
                    foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                    {
                        if (chunk.qdbErrorsCount > 0)
                        {
                            try
                            {
                                using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_qdbErrors" + chunk.id.ToString() + ".txt"))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var line = reader.ReadLine();
                                        dgQdbErrors.Rows.Add(line.Split('\t'));
                                    }
                                }
                            }catch(Exception ex) { }
                        }
                    }
                }
            }

            if (aces.basevehicleidsErrorsCount > 0)
            {
                tabControl1.TabPages.Add(hiddenInvalidBaseVehiclesTab); highestVisableTab1Index++;
                if (aces.basevehicleidsErrorsCount > largeDatagridRecordThreshold)
                {
                    if (checkBoxLimitDataGridRows.Checked)
                    {
                        lblInvalidBasevehiclesRedirect.Visible = true;
                        lblInvalidBasevehiclesRedirect.Text = "Invalid basevehicles are too numerous (" + aces.basevehicleidsErrorsCount.ToString() + ") to display here. See assessment file for full list.";
                    }
                    else
                    {
                        dgBasevids.Visible = true;
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.basevehicleidsErrorsCount> 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidBasevehicles" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            var line = reader.ReadLine();
                                            dgBasevids.Rows.Add(line.Split('\t'));
                                        }
                                    }
                                }
                                catch(Exception ex) { }
                            }
                        }
                    }
                }
                else
                {
                    dgBasevids.Visible = true;
                    foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                    {
                        if (chunk.basevehicleidsErrorsCount > 0)
                        {

                            try
                            {
                                using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidBasevehicles" + chunk.id.ToString() + ".txt"))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var line = reader.ReadLine();
                                        dgBasevids.Rows.Add(line.Split('\t'));
                                    }
                                }
                            }catch(Exception ex) { }
                        }
                    }
                }
            }
            

            if (aces.vcdbCodesErrorsCount > 0)
            {
                tabControl1.TabPages.Add(hiddenInvalidVCdbCodesTab); highestVisableTab1Index++;
                if(aces.vcdbCodesErrorsCount>largeDatagridRecordThreshold)
                {
                    if(checkBoxLimitDataGridRows.Checked)
                    {
                        lblInvalidVCdbCodesRedirect.Visible = true;
                        lblInvalidVCdbCodesRedirect.Text = "Invalid VCdb code errors are too numerous (" + aces.vcdbCodesErrorsCount.ToString() + ") to display here. See assessment file for full list.";
                    }
                    else
                    {
                        dgVCdbCodes.Visible = true;
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.vcdbCodesErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidVCdbCodes" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            var line = reader.ReadLine();
                                            dgVCdbCodes.Rows.Add(line.Split('\t'));
                                        }
                                    }
                                }catch(Exception ex) { }
                            }
                        }
                    }
                }
                else
                {
                    dgVCdbCodes.Visible = true;
                    foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                    {
                        if (chunk.vcdbCodesErrorsCount > 0)
                        {
                            try
                            {
                                using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_invalidVCdbCodes" + chunk.id.ToString() + ".txt"))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var line = reader.ReadLine();
                                        dgVCdbCodes.Rows.Add(line.Split('\t'));
                                    }
                                }
                            }
                            catch(Exception ex) { }
                        }
                    }
                }
            }

            if (aces.vcdbConfigurationsErrorsCount > 0)
            {
                tabControl1.TabPages.Add(hiddenInvalidConfigurationsTab); highestVisableTab1Index++;
                if (aces.vcdbConfigurationsErrorsCount > largeDatagridRecordThreshold)
                {
                    if (checkBoxLimitDataGridRows.Checked)
                    {
                        lblVCdbConfigErrorRedirect.Visible = true;
                        lblVCdbConfigErrorRedirect.Text = "VCdb configuration errors are too numerous (" + aces.vcdbConfigurationsErrorsCount.ToString() + ") to display here. See assessment file for full list.";
                    }
                    else
                    {
                        dgVCdbConfigs.Visible = true;
                        foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                        {
                            if (chunk.vcdbConfigurationsErrorsCount > 0)
                            {
                                try
                                {
                                    using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_configurationErrors" + chunk.id.ToString() + ".txt"))
                                    {
                                        while (!reader.EndOfStream)
                                        {
                                            var line = reader.ReadLine();
                                            dgVCdbConfigs.Rows.Add(line.Split('\t'));
                                        }
                                    }
                                }catch(Exception ex) { }
                            }
                        }
                    }
                }
                else
                {
                    dgVCdbConfigs.Visible = true;
                    foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                    {
                        if (chunk.vcdbConfigurationsErrorsCount > 0)
                        {
                            try
                            {
                                using (var reader = new StreamReader(lblCachePath.Text + "\\AiFragments\\" + aces.fileMD5hash + "_configurationErrors" + chunk.id.ToString() + ".txt"))
                                {
                                    while (!reader.EndOfStream)
                                    {
                                        var line = reader.ReadLine();
                                        dgVCdbConfigs.Rows.Add(line.Split('\t'));
                                    }
                                }
                            }catch(Exception ex) { }
                        }
                    }
                }
            }


            if (aces.fitmentLogicProblemsCount > 0)
            {
                tabControl1.TabPages.Add(hiddenLogicProblemsTab); highestVisableTab1Index++; 

                if (aces.fitmentLogicProblemsCount > largeDatagridRecordThreshold)
                {
                    if (checkBoxLimitDataGridRows.Checked)
                    {
                        lblFitmentLogicProblemsTabRedirect.Visible = true;
                        lblFitmentLogicProblemsTabRedirect.Text = "Fitment logic problems are too numerous (" + aces.fitmentLogicProblemsCount.ToString() + ") to display here. See assessment file for full list.";
                    }
                    else
                    {
                        dgFitmentLogicProblems.Visible = true;
                        pictureBoxFitmentTree.Visible = true;

                        foreach(KeyValuePair<string,List<App>> entry in aces.fitmentProblemGroupsAppLists)
                        {// construct a tree in order to re-discover the problmes with it.
                            aces.fitmentNodeList.Clear(); fitmentElementPrevalence.Clear();
                            foreach (string fitmentElement in aces.fitmentProblemGroupsBestPermutations[entry.Key]){fitmentElementPrevalence.Add(fitmentElement, elementPrevalence); elementPrevalence++;}
                            aces.fitmentNodeList.AddRange(aces.buildFitmentTreeFromAppList(entry.Value, fitmentElementPrevalence, -1, false, false, vcdb, qdb));
                            problemDescription = aces.fitmentTreeProblemDescription(aces.fitmentNodeList, checkBoxConcernForDisparate.Checked);

                            foreach (App app in entry.Value)
                            {
                                dgFitmentLogicProblems.Rows.Add(problemDescription,entry.Key,app.id.ToString(),app.reference,app.basevehilceid.ToString(), vcdb.niceMakeOfBasevid(app.basevehilceid), vcdb.niceModelOfBasevid(app.basevehilceid), vcdb.niceYearOfBasevid(app.basevehilceid),pcdb.niceParttype(app.parttypeid),pcdb.nicePosition(app.positionid),app.quantity.ToString(),app.part,app.niceFullFitmentString(vcdb, qdb));
                            }
                        }
                    }
                }
                else
                {
                    dgFitmentLogicProblems.Visible = true;
                    pictureBoxFitmentTree.Visible = true;
                    foreach (KeyValuePair<string, List<App>> entry in aces.fitmentProblemGroupsAppLists)
                    {   // construct a tree in order to re-discover the problmes with it.
                        aces.fitmentNodeList.Clear(); fitmentElementPrevalence.Clear();
                        foreach (string fitmentElement in aces.fitmentProblemGroupsBestPermutations[entry.Key]) { fitmentElementPrevalence.Add(fitmentElement, elementPrevalence); elementPrevalence++; }
                        aces.fitmentNodeList.AddRange(aces.buildFitmentTreeFromAppList(entry.Value, fitmentElementPrevalence, -1, false, false, vcdb, qdb));
                        problemDescription = aces.fitmentTreeProblemDescription(aces.fitmentNodeList, checkBoxConcernForDisparate.Checked);

                        foreach (App app in entry.Value)
                        {
                            dgFitmentLogicProblems.Rows.Add(problemDescription, entry.Key, app.id.ToString(), app.reference, app.basevehilceid.ToString(), vcdb.niceMakeOfBasevid(app.basevehilceid), vcdb.niceModelOfBasevid(app.basevehilceid), vcdb.niceYearOfBasevid(app.basevehilceid), pcdb.niceParttype(app.parttypeid), pcdb.nicePosition(app.positionid), app.quantity.ToString(), app.part, app.niceFullFitmentString(vcdb, qdb));
                        }
                    }
                }
            }



            if (refaces.apps.Count > 0)
            {
                if (diffaces.differentialParts.Count > largeDatagridRecordThreshold)
                {// too many diff parts to display
                    if (checkBoxLimitDataGridRows.Checked)
                    {
                        // show the "not here" lable
                        lblAddsDropsPartsErrorRedirect.Visible = true;
                        lblAddsDropsPartsErrorRedirect.Text = "Part adds/drops list is too large to show here (" + diffaces.differentialParts.Count.ToString() + "). See assessment file for full list.";
                    }
                }
                else
                {// diff parts small enough to be displayed
                    foreach (string line in diffaces.differentialParts) { dgAddsDropsParts.Rows.Add(line.Split('\t')); }
                }

                if (diffaces.differentialVehicles.Count > largeDatagridRecordThreshold)
                {// too many diff vehciles to display
                    if (checkBoxLimitDataGridRows.Checked)
                    {
                        // show the "not here" lable
                        lblAddsDropsPartsErrorRedirect.Visible = true;
                        lblAddsDropsPartsErrorRedirect.Text = "Part adds/drops list is too large to show here (" + diffaces.differentialParts.Count.ToString() + "). See assessment file for full list.";
                    }
                }
                else
                {// diff vehicles small enough to be displayed
                    App tempApp = new App();
                    foreach (string line in diffaces.differentialVehicles)
                    {
                        string[] fields = line.Split('\t');
                        tempApp.Clear();
                        tempApp.basevehilceid = Convert.ToInt32(fields[1]);
                        tempApp.parttypeid = Convert.ToInt32(fields[2]);
                        tempApp.positionid = Convert.ToInt32(fields[3]);
                        if (fields[5] != "") { tempApp.VCdbAttributes = aces.parseAttributePairsString(fields[4]); }
                        tempApp.notes = fields[5].Split(';').ToList();
                        tempApp.mfrlabel = fields[6];
                        dgAddsDropsVehicles.Rows.Add(fields[0], tempApp.basevehilceid.ToString(), vcdb.niceMakeOfBasevid(tempApp.basevehilceid), vcdb.niceModelOfBasevid(tempApp.basevehilceid), vcdb.niceYearOfBasevid(tempApp.basevehilceid), pcdb.niceParttype(tempApp.parttypeid), pcdb.nicePosition(tempApp.positionid), tempApp.niceFullFitmentString(vcdb, qdb), tempApp.mfrlabel);
                    }
                }

                lblDifferentialsSummary.Text = diffaces.differentialsSummary;
                if (diffaces.differentialParts.Count > 0) { tabControl1.TabPages.Add(hiddenAddsDropsPartsTab); highestVisableTab1Index++; dgAddsDropsParts.Visible = true; }
                if (diffaces.differentialVehicles.Count > 0) { tabControl1.TabPages.Add(hiddenAddsDropsVehiclesTab); highestVisableTab1Index++; dgAddsDropsVehicles.Visible = true; }
            }












            pictureBoxLogicProblems.Invalidate();
            pictureBoxCommonErrors.Invalidate();


            btnSelectReferenceACESfile.Enabled = true;

            btnSelectACESfile.Enabled = true;
            btnSelectVCdbFile.Enabled = true;
            btnSelectPCdbFile.Enabled = true;
            btnSelectQdbFile.Enabled = true;

            lblStatsProcessingTime.Text = Math.Round(Convert.ToDecimal(aces.analysisTime)/5,1).ToString() + " Seconds";
            lblProcessTimeTitle.Text = "Processing Time";
            lblStatus.Visible = true; lblStatus.Text = "";

            if ((aces.parttypePositionErrorsCount + aces.vcdbCodesErrorsCount + aces.vcdbConfigurationsErrorsCount + aces.basevehicleidsErrorsCount + aces.qdbErrorsCount + aces.vcdbConfigurationsErrorsCount + aces.fitmentLogicProblemsCount) > 0)
            {
                lblStatus.Text = "Analysis complete (Failed)" ; lblStatus.BackColor = Color.Red;
                aces.logHistoryEvent("", "0\tAnalysis complete (Failed)");
            }
            else
            {
                if ((aces.parttypeDisagreementCount + aces.qtyOutlierCount) > 0)
                {
                    lblStatus.Text = "Analysis complete (Passed with warnings)"; lblStatus.BackColor = Color.Yellow;
                    aces.logHistoryEvent("", "0\tAnalysis complete (Passed with warnings)");
                }
                else
                {
                    lblStatus.Text = "Analysis complete (Passed)"; lblStatus.BackColor = Color.LightGreen;
                    aces.logHistoryEvent("", "0\tAnalysis complete (Passed)");

                }
            }



            if (aces.VcdbVersionDate != vcdb.version)
            {
                lblStatsVCdbVersion.ForeColor = Color.DarkOrange;
                lblStatsVCdbVersion.Text = aces.VcdbVersionDate + "  (validated against VCdb:" + vcdb.version + ")";
            }

            if (aces.PcdbVersionDate != pcdb.version)
            {
                lblStatsPCdbVersion.ForeColor = Color.DarkOrange;
                lblStatsPCdbVersion.Text = aces.PcdbVersionDate + "  (validated against PCdb:" + pcdb.version + ")";
            }

            if (aces.QdbVersionDate != qdb.version)
            {
                lblStatsQdbVersion.ForeColor = Color.DarkOrange;
                lblStatsQdbVersion.Text = aces.QdbVersionDate + "  (validated against Qdb:" + qdb.version + ")";
            }

            aces.recordAnalysisResults(vcdb.version, pcdb.version); // record file hash and results in registry
        }

        
        private async void btnAppExportSave_Click(object sender, EventArgs e)
        {
            string result = ""; string delimiter = "";
            var progressIndicator = new Progress<int>(ReportExportFlatAppsProgress);

            if (comboBoxExportDelimiter.SelectedIndex == 0) { delimiter = "\t"; }
            if (comboBoxExportDelimiter.SelectedIndex == 1) { delimiter = "|"; }
            if (comboBoxExportDelimiter.SelectedIndex == 2) { delimiter = "~"; }
            string exportFormat = comboBoxFlatExportFormat.Items[comboBoxFlatExportFormat.SelectedIndex].ToString();

            using (var fbd = new FolderBrowserDialog())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key.CreateSubKey("ACESinspector");
                key = key.OpenSubKey("ACESinspector", true);
                if (key.GetValue("lastAppExportDirectoryPath") != null) { fbd.SelectedPath = key.GetValue("lastAppExportDirectoryPath").ToString(); }
                DialogResult dialogResult = fbd.ShowDialog();
                if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && aces.apps.Count > 0)
                {
                    key.SetValue("lastAppExportDirectoryPath", fbd.SelectedPath);
                    result = await Task.Run(() => aces.exportFlatApps(fbd.SelectedPath + "\\"+ Path.GetFileNameWithoutExtension(aces.filePath) + "_flattened.txt", vcdb, pcdb, qdb, delimiter,exportFormat, progressIndicator));
                    MessageBox.Show(result);
                }
            }

        }

        private async void btnBGExportSave_Click(object sender, EventArgs e)
        {
            string result = "";
            var progressIndicator = new Progress<int>(ReportBuyersGuideExportProgress);
            btnAnalyze.Enabled = false; btnBgExportSave.Enabled = false; btnExportRelatedParts.Enabled = false; btnAppExportSave.Enabled = false;

            using (var fbd = new FolderBrowserDialog())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key.CreateSubKey("ACESinspector");
                key = key.OpenSubKey("ACESinspector", true);
                if (key.GetValue("lastBuyersGuideDirectoryPath") != null) { fbd.SelectedPath = key.GetValue("lastBuyersGuideDirectoryPath").ToString(); }
                DialogResult dialogResult = fbd.ShowDialog();
                if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && aces.apps.Count > 0)
                {
                    key.SetValue("lastBuyersGuideDirectoryPath", fbd.SelectedPath);
                    lblStatus.Text = "Exporting buyer's guide";
                    result = await Task.Run(() => aces.exportBuyersGuide(fbd.SelectedPath + "\\"+ Path.GetFileNameWithoutExtension(aces.filePath) + "_buyersguide.txt", vcdb, progressIndicator));
                    MessageBox.Show(result);
                }
            }
            btnAnalyze.Enabled = true; btnBgExportSave.Enabled = true; btnExportRelatedParts.Enabled = true; btnAppExportSave.Enabled = true;
        }


        private void btnNetChangeExportSave_Click(object sender, EventArgs e)
        {
            string result = "";

            if (diffaces.apps.Count > 0)
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                    key.CreateSubKey("ACESinspector");
                    key = key.OpenSubKey("ACESinspector", true);
                    if (key.GetValue("lastNetChangeDirectoryPath") != null) { fbd.SelectedPath = key.GetValue("lastNetChangeDirectoryPath").ToString(); }
                    DialogResult dialogResult = fbd.ShowDialog();
                    if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && aces.apps.Count > 0)
                    {
                        key.SetValue("lastNetChangeDirectoryPath", fbd.SelectedPath);
                        result = diffaces.exportXMLApps(fbd.SelectedPath + "\\" + Path.GetFileNameWithoutExtension(aces.filePath)+"_diffs.xml", "UPDATE", "mypath", false);
                        MessageBox.Show(result);
                    }
                }
            }
            else
            {
                MessageBox.Show("No net-change applications found for export");
            }

        }


        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void dgCNCoverlaps_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            //Column 0 is group number - it needs to be compared numerically for sort instead of the default alpha
            if (e.Column.Index == 0)
            {
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

            //Column 1 is app id - it needs to be compared numerically for sort instead of the default alpha
            if (e.Column.Index == 1)
            {
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

            //Column 2 is basevid - it needs to be compared numerically for sort instead of the default alpha
            if (e.Column.Index == 2)
            {
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }


            //Column 5 is year - it needs to be compared numerically for sort instead of the default alpha
            if (e.Column.Index == 5)
            {
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }
        }

        private void dgDuplicates_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            //Column 0 is basevid - it needs to be compared numerically for sort instead of the default alpha
            if (e.Column.Index == 0)
            {
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

        }

        private void dgOverlaps_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            //Column 0 is group number - it needs to be compared numerically for sort instead of the default alpha
            if (e.Column.Index == 0)
            {
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

        }

        private void dgParttypePosition_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            //Column 0 is group number - it needs to be compared numerically for sort instead of the default alpha
            if (e.Column.Index == 1)
            {
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

            if (e.Column.Index == 2)
            {
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

        }

        private void dgVCdbConfigs_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0)
            {// appid column
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

            if (e.Column.Index == 1)
            {// basevid column
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

            if (e.Column.Index == 7)
            {// qty column
                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }



        }



        private void btnSelectPartInterchange_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if (key.GetValue("lastInterchangeDirectoryPath") != null) { openFileDialog.InitialDirectory = key.GetValue("lastInterchangeDirectoryPath").ToString(); }

            openFileDialog.Title = "Open part interchange text file";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            DialogResult openFileResult = openFileDialog.ShowDialog();
            if (openFileResult.ToString() == "OK")
            {
                aces.interchange.Clear();
                //lblinterchangefilePath.Text = Path.GetFileName(openFileDialog.FileName);
                key.SetValue("lastInterchangeDirectoryPath", Path.GetDirectoryName(openFileDialog.FileName));
                aces.importInterchange(openFileDialog.FileName);
                if (aces.interchange.Count() > 0)
                {
                    lblinterchangefilePath.Text = Path.GetFileName(openFileDialog.FileName) + "   (Contains " + aces.interchange.Count.ToString() + " part translation records)";
                }
                else
                {
                    lblinterchangefilePath.Text = "";
                    MessageBox.Show(openFileDialog.FileName + " does not contain any properly formatted interchange records.\r\n\r\nFormat is:\r\n  <input item 1><tab><output item 1>\r\n  <input item 2><tab><output item 2>\r\n  <input item 3><tab><output item 3>\r\n  ...\r\n\r\nThe file must contain exactly two columns and contain no header row. Every app node in the imported ACES file will have its part translated by lookup in the fist column. Apps containing parts not found in the first column will not be imported from the ACES file.");
                }
            }
        }


        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // handle popup tooltip for controls that are disabled
            bool foundControl = false;
            if (btnSelectPartInterchange == this.GetChildAtPoint(e.Location))
            {
                foundControl = true;
                if (!toolTipIsShown)
                {
                    //toolTip1.Show("Part interchange must be selected before an XML file is imported", this, e.Location);
                    //toolTipIsShown = true;
                }
            }

            if (btnSelectReferenceACESfile == this.GetChildAtPoint(e.Location))
            {
                foundControl = true;
                if (!toolTipIsShown)
                {
                    //toolTip1.Show("Reference ACES file can be selected after successful import of a primary ACES file", this, e.Location);
                    //toolTipIsShown = true;
                }
            }

            if (btnAnalyze == this.GetChildAtPoint(e.Location))
            {
                foundControl = true;
                if (!toolTipIsShown)
                {
                    //toolTip1.Show("You must successfully import a primary ACES file before an analysis can be run.", this, e.Location);
                    //toolTipIsShown = true;
                }
            }



            if (!foundControl && toolTipIsShown)
            {
                //toolTip1.Hide(btnSelectPartInterchange);
                //toolTipIsShown = false;
            }


        }

        private void btnHolesExportSave_Click(object sender, EventArgs e)
        {
            string result = "";
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult dialogResult = fbd.ShowDialog();
                if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                    key.CreateSubKey("ACESinspector");
                    key = key.OpenSubKey("ACESinspector", true);
                    key.SetValue("lastHolesDirectoryPath", fbd.SelectedPath);
                    result = aces.exportHolesReport(vcdb, fbd.SelectedPath + @"\MissingBsevehicles.txt");
                    MessageBox.Show(result);
                }
            }
        }


        
        private async void btnExportRelatedParts_Click(object sender, EventArgs e)
        {
            string result = "";
            bool usePosition = checkBoxRelatedPartsUsePosition.Checked; bool useAttributes = checkBoxRelatedPartsUseAttributes.Checked; bool useNotes = checkBoxRelatedPartsUseNotes.Checked;
            string leftType = comboBoxRelatedTypesLeft.Items[comboBoxRelatedTypesLeft.SelectedIndex].ToString(); string rightType = comboBoxRelatedTypesRight.Items[comboBoxRelatedTypesRight.SelectedIndex].ToString();
            var progressIndicator = new Progress<int>(ReportExportrelatedPartsProgress);

            if (comboBoxRelatedTypesLeft.SelectedIndex == comboBoxRelatedTypesRight.SelectedIndex)
            {
                MessageBox.Show("You must select two differnt part types from the drop-down lists.");
            }
            else
            {
                lblStatus.Text = "Exporting buyer's related parts pairings";

                using (var fbd = new FolderBrowserDialog())
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                    key.CreateSubKey("ACESinspector");
                    key = key.OpenSubKey("ACESinspector", true);
                    if (key.GetValue("lastRelatedPartsDirectoryPath") != null) { fbd.SelectedPath = key.GetValue("lastRelatedPartsDirectoryPath").ToString(); }
                    DialogResult dialogResult = fbd.ShowDialog();
                    if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && aces.apps.Count > 0)
                    {
                        key.SetValue("lastRelatedPartsDirectoryPath", fbd.SelectedPath);
                        result = await Task.Run(() => aces.exportRelatedParts(fbd.SelectedPath + @"\RelatedParts.txt", pcdb, leftType, rightType, usePosition, useAttributes, useNotes, progressIndicator));
                        MessageBox.Show(result);
                    }
                }
            }
        }

        private void btnExportPrimaryACES_Click(object sender, EventArgs e)
        {
            string result = "";

            if (aces.apps.Count > 0)
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                    key.CreateSubKey("ACESinspector");
                    key = key.OpenSubKey("ACESinspector", true);
                    if (key.GetValue("lastNetChangeDirectoryPath") != null) { fbd.SelectedPath = key.GetValue("lastNetChangeDirectoryPath").ToString(); }
                    DialogResult dialogResult = fbd.ShowDialog();
                    if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && aces.apps.Count > 0)
                    {
                        key.SetValue("lastNetChangeDirectoryPath", fbd.SelectedPath);
                        if (checkBoxEncipherExport.Checked)
                        {
                            result = aces.exportXMLApps(fbd.SelectedPath + "\\" + Path.GetFileNameWithoutExtension(aces.filePath)+ "_enciphered.xml", "TEST", fbd.SelectedPath + "\\" + Path.GetFileNameWithoutExtension(aces.filePath) + "_decipher_interchange.txt", false);
                        }
                        else
                        {
                            result = aces.exportXMLApps(fbd.SelectedPath + "\\" + Path.GetFileNameWithoutExtension(aces.filePath)+ "_re-export.xml", "FULL", "", false);
                        }
                        MessageBox.Show(result);
                    }
                }
            }
            else
            {
                MessageBox.Show("No applications found for export");
            }

        }

        private void btnExportConfigerrorsACES_Click(object sender, EventArgs e)
        {

        }

        private void pictureBoxFitmentTree_Paint(object sender, PaintEventArgs e)
        {
            if (aces.fitmentNodeList.Count() > 0)
            {
                float fitmentElementFontSize = 10F;
                Graphics g = e.Graphics;
                int yPos, maxDepth, z, firstNodeId, lastNodeId;

                Rectangle fitmentBoxLayoutRcetangle = new Rectangle();
                Font fitmentElementTitleFont = new Font("Microsoft Sans Serif", fitmentElementFontSize, FontStyle.Regular);//Microsoft Sans Serif
                Brush fitmentElementTitleBrush = new SolidBrush(System.Drawing.Color.Black);
                StringFormat fitmentElementTitleStringFormat = new StringFormat();
                //fitmentElementTitleStringFormat.FormatFlags = StringFormatFlags.NoWrap;
                List<fitmentNode> nodes = new List<fitmentNode>();

                aces.fitmentTreeAddFillerNodes(0, 0);
                nodes = aces.fitmentNodeList;

                int nodeBcounter, nodeCcounter, nodeDcounter, nodeEcounter, nodeFcounter, nodeGcounter, nodeHcounter, nodeIcounter;
                int xPos; int verticalGenerationSpacing; int fitmentElementBoxWidth = 100; //divide the canvas width by this number to get actual width of box

                Pen penNormalLine = new Pen(Brushes.Black, 2);
                Pen penCNCLine = new Pen(Brushes.Red, 2);
                Pen penDisparateLine = new Pen(Brushes.Orange, 2);
                Pen penVCdbElement = new Pen(Brushes.ForestGreen, 3);
                Pen penQdbElement = new Pen(Brushes.DarkBlue, 3);
                Pen penNoteElement = new Pen(Brushes.Gray, 3);
                Pen penAppElement = new Pen(Brushes.Purple, 3);
                Pen penAppElementHighlited = new Pen(Brushes.Fuchsia, 4);
                Pen penLine = penNormalLine;
                Pen penBox = penNoteElement;
                // holy crap thats complex!                        MK 12/19/2017 
                // why, thank you, Mike :)  LPS 12/19/2017



                //g.DrawString("branchyness:" + aces.fitmentTreeBranchyness(nodes).ToString(), fitmentElementTitleFont, fitmentElementTitleBrush, 10, 5, fitmentElementTitleStringFormat);

                g.DrawString(nodes[0].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, 10, 5, fitmentElementTitleStringFormat);

                // g.DrawString("badness:    " + aces.fitmentTreeTotalBadBranches(nodes,chkBadBranches.Checked).ToString(), fitmentElementTitleFont, fitmentElementTitleBrush, 10, 20, fitmentElementTitleStringFormat);

                // look for a "touched" (think Madonna, 1981?) node, and render its particulars in the the top-left corner of the pictureBox 
                foreach (fitmentNode myNode in nodes)
                {
                    if (myNode.deleted) { continue; }

                    /*
                                        if (myNode.touched)
                                        {
                                            nodes[myNode.nodeId].touched = false;
                                            if (myNode.clickType == 1)
                                            {
                                                aces.CNCtreeSOMETHING(nodes);
                                                nodes[myNode.nodeId].clickType = 0;
                                                break;
                                            }
                                            if (myNode.clickType == 3)
                                            {
                                                aces.CNCmergeRedundantChildren(nodes, nodes[myNode.parentNode].parentNode);
                                                nodes[myNode.nodeId].clickType = 0;
                                                break;
                                            }
                                            if (myNode.clickType == 2)
                                            {
                                                aces.CNCtreeSplitBranch(nodes, myNode.nodeId);
                                                nodes[myNode.nodeId].clickType = 0;
                                                break;
                                            }
                                        }

                                    */


                }

                List<int> nodeIdsListA = new List<int>();
                nodeIdsListA = nodes[0].childNodeIds;
                foreach (int nodeIdA in nodeIdsListA)
                {
                    if (nodes[nodeIdA].deleted) { continue; }
                    nodes[nodeIdA].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdA].fitmentElementString , fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                    nodes[nodeIdA].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdA].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                    List<int> nodeIdsListB = new List<int>();
                    nodeIdsListB = nodes[nodeIdA].childNodeIds;
                    foreach (int nodeIdB in nodeIdsListB)
                    {
                        if (nodes[nodeIdB].deleted) { continue; }
                        nodes[nodeIdB].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdB].fitmentElementString , fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                        nodes[nodeIdB].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdB].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                        List<int> nodeIdsListC = new List<int>();
                        nodeIdsListC = nodes[nodeIdB].childNodeIds;
                        foreach (int nodeIdC in nodeIdsListC)
                        {
                            if (nodes[nodeIdC].deleted) { continue; }
                            nodes[nodeIdC].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdC].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                            nodes[nodeIdC].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdC].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                            List<int> nodeIdsListD = new List<int>();
                            nodeIdsListD = nodes[nodeIdC].childNodeIds;
                            foreach (int nodeIdD in nodeIdsListD)
                            { 
                                if (nodes[nodeIdD].deleted) { continue; }
                                nodes[nodeIdD].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdD].fitmentElementString , fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                                nodes[nodeIdD].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdD].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                                List<int> nodeIdsListE = new List<int>();
                                nodeIdsListE = nodes[nodeIdD].childNodeIds;
                                foreach (int nodeIdE in nodeIdsListE)
                                { 
                                    if (nodes[nodeIdE].deleted) { continue; }
                                    nodes[nodeIdE].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdE].fitmentElementString , fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                                    nodes[nodeIdE].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdE].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                                    List<int> nodeIdsListF = new List<int>();
                                    nodeIdsListF = nodes[nodeIdE].childNodeIds;
                                    foreach (int nodeIdF in nodeIdsListF)
                                    { 
                                        if (nodes[nodeIdF].deleted) { continue; }
                                        nodes[nodeIdF].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdF].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                                        nodes[nodeIdF].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdF].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                                        List<int> nodeIdsListG = new List<int>();
                                        nodeIdsListG = nodes[nodeIdF].childNodeIds;
                                        foreach (int nodeIdG in nodeIdsListG)
                                        {
                                            if (nodes[nodeIdG].deleted) { continue; }
                                            nodes[nodeIdG].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdG].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                                            nodes[nodeIdG].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdG].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                                            List<int> nodeIdsListH = new List<int>();
                                            nodeIdsListH = nodes[nodeIdG].childNodeIds;
                                            foreach (int nodeIdH in nodeIdsListH)
                                            {
                                                if (nodes[nodeIdH].deleted) { continue; }
                                                nodes[nodeIdH].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdH].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                                                nodes[nodeIdH].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdH].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                                                List<int> nodeIdsListI = new List<int>();
                                                nodeIdsListI = nodes[nodeIdH].childNodeIds;
                                                foreach (int nodeIdI in nodeIdsListI)
                                                {
                                                    if (nodes[nodeIdI].deleted) { continue; }
                                                    nodes[nodeIdI].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdI].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                                                    nodes[nodeIdI].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdI].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
                                                    List<int> nodeIdsListJ = new List<int>();
                                                    nodeIdsListJ = nodes[nodeIdI].childNodeIds;
                                                    foreach (int nodeIdJ in nodeIdsListJ)
                                                    {
                                                        if (nodes[nodeIdJ].deleted) { continue; }
                                                        nodes[nodeIdJ].graphicalWidth = Convert.ToInt32(g.MeasureString(nodes[nodeIdJ].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Width);
                                                        nodes[nodeIdJ].graphicalHeight = Convert.ToInt32(g.MeasureString(nodes[nodeIdJ].fitmentElementString, fitmentElementTitleFont, fitmentElementBoxWidth, fitmentElementTitleStringFormat).Height);
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

                int tallestNode = 0; maxDepth = 0;
                foreach (fitmentNode myNode in nodes){if (myNode.graphicalHeight > tallestNode){tallestNode = myNode.graphicalHeight;}if(myNode.pathFromRoot.Count() > maxDepth) { maxDepth = myNode.pathFromRoot.Count(); }}

                verticalGenerationSpacing = (pictureBoxFitmentTree.Height) / (maxDepth + 2);
                if (verticalGenerationSpacing < tallestNode) { verticalGenerationSpacing = tallestNode; }
                yPos = (verticalGenerationSpacing * maxDepth)+30+treeCanvasYbase + treeCanvasYoffset;

                List<int> nodeIdsAtLevel = new List<int>();
                for (z = maxDepth; z >= 0; z--)
                {
                    xPos = 50 + treeCanvasXbase + treeCanvasXoffset;
                    nodeIdsAtLevel = aces.fitmentNodeIdsAtLevel(nodes, z);
                    foreach(int nodeIdAtLevel in nodeIdsAtLevel)
                    {
                        if (nodes[nodeIdAtLevel].childNodeIds.Count() == 0)
                        {// this is a bottom-est level node
                            nodes[nodeIdAtLevel].graphicalXpos = xPos;
                            nodes[nodeIdAtLevel].graphicalYpos = yPos;
                            xPos += Math.Max(nodes[nodeIdAtLevel].graphicalWidth,103);
                        }
                        else
                        {// this node has children that already determine it's position. find child list
                            firstNodeId=nodes[nodeIdAtLevel].childNodeIds.First();
                            lastNodeId = nodes[nodeIdAtLevel].childNodeIds.Last();
                            nodes[nodeIdAtLevel].graphicalXpos = ((nodes[lastNodeId].graphicalXpos + nodes[firstNodeId].graphicalXpos + nodes[lastNodeId].graphicalWidth)/2)-(nodes[nodeIdAtLevel].graphicalWidth/2);
                            nodes[nodeIdAtLevel].graphicalYpos = yPos;
                        }
                    }
                    yPos -= verticalGenerationSpacing;
                }

                // I'm well aware of recursion - I decided not to use it here

                nodeIdsListA = nodes[0].childNodeIds;
                foreach (int nodeIdA in nodeIdsListA)
                {
                    if (nodes[nodeIdA].deleted) { continue; }
                    if (nodes[nodeIdA].fitmentElementType == "vcdb") { penBox = penVCdbElement; } if (nodes[nodeIdA].fitmentElementType == "qdb") { penBox = penQdbElement; } if (nodes[nodeIdA].fitmentElementType == "note") { penBox = penNoteElement; } if (nodes[nodeIdA].app != null) { penBox = penAppElement; if (nodes[nodeIdA].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } }
                    fitmentBoxLayoutRcetangle.X = nodes[nodeIdA].graphicalXpos;
                    fitmentBoxLayoutRcetangle.Y = nodes[nodeIdA].graphicalYpos; fitmentBoxLayoutRcetangle.Width = nodes[nodeIdA].graphicalWidth+10; fitmentBoxLayoutRcetangle.Height = nodes[nodeIdA].graphicalHeight;
                    g.DrawLine(aces.fitmentBranchPen(0, nodes, true), nodes[nodeIdA].graphicalXpos + (nodes[nodeIdA].graphicalWidth / 2), nodes[nodeIdA].graphicalYpos, nodes[0].graphicalXpos, 20 + treeCanvasYbase + treeCanvasYoffset);
                    g.DrawRectangle(penBox, nodes[nodeIdA].graphicalXpos, nodes[nodeIdA].graphicalYpos, nodes[nodeIdA].graphicalWidth, nodes[nodeIdA].graphicalHeight);
                    g.DrawString(nodes[nodeIdA].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);

                    List<int> nodeIdsListB = new List<int>();
                    nodeIdsListB = nodes[nodeIdA].childNodeIds;
                    nodeBcounter = 0;
                    foreach (int nodeIdB in nodeIdsListB)
                    {
                        if (nodes[nodeIdB].deleted || nodes[nodeIdB].filler) { continue; }
                        nodeBcounter++;
                        if (nodes[nodeIdB].fitmentElementType == "vcdb") { penBox = penVCdbElement; }  if (nodes[nodeIdB].fitmentElementType == "qdb") { penBox = penQdbElement; } if (nodes[nodeIdB].fitmentElementType == "note") { penBox = penNoteElement; } if (nodes[nodeIdB].app != null) { penBox = penAppElement; if (nodes[nodeIdB].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } } 
                        fitmentBoxLayoutRcetangle.X = nodes[nodeIdB].graphicalXpos; fitmentBoxLayoutRcetangle.Y = nodes[nodeIdB].graphicalYpos; fitmentBoxLayoutRcetangle.Width = nodes[nodeIdB].graphicalWidth+10; fitmentBoxLayoutRcetangle.Height = nodes[nodeIdB].graphicalHeight;
                        g.DrawLine(aces.fitmentBranchPen(nodeIdA, nodes, true), nodes[nodeIdB].graphicalXpos + (nodes[nodeIdB].graphicalWidth / 2), nodes[nodeIdB].graphicalYpos, nodes[nodeIdA].graphicalXpos + (nodes[nodeIdA].graphicalWidth / (nodeIdsListB.Count() + 1) * nodeBcounter), nodes[nodeIdA].graphicalYpos + nodes[nodeIdA].graphicalHeight);
                        g.DrawRectangle(penBox, nodes[nodeIdB].graphicalXpos, nodes[nodeIdB].graphicalYpos, nodes[nodeIdB].graphicalWidth, nodes[nodeIdB].graphicalHeight);
                        g.DrawString(nodes[nodeIdB].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);

                        List<int> nodeIdsListC = new List<int>();
                        nodeIdsListC = nodes[nodeIdB].childNodeIds;
                        nodeCcounter = 0;
                        foreach (int nodeIdC in nodeIdsListC)
                        {
                            if (nodes[nodeIdC].deleted || nodes[nodeIdC].filler) { continue; }
                            nodeCcounter++;
                            if (nodes[nodeIdC].fitmentElementType == "vcdb") { penBox = penVCdbElement; } if (nodes[nodeIdC].fitmentElementType == "qdb") { penBox = penQdbElement; } if (nodes[nodeIdC].fitmentElementType == "note") { penBox = penNoteElement; } if (nodes[nodeIdC].app != null) { penBox = penAppElement; if (nodes[nodeIdC].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } }
                            fitmentBoxLayoutRcetangle.X = nodes[nodeIdC].graphicalXpos; fitmentBoxLayoutRcetangle.Y = nodes[nodeIdC].graphicalYpos;
                            fitmentBoxLayoutRcetangle.Width = nodes[nodeIdC].graphicalWidth+10; fitmentBoxLayoutRcetangle.Height = nodes[nodeIdC].graphicalHeight;
                            g.DrawLine(aces.fitmentBranchPen(nodeIdB, nodes, true), nodes[nodeIdC].graphicalXpos + (nodes[nodeIdC].graphicalWidth / 2), nodes[nodeIdC].graphicalYpos, nodes[nodeIdB].graphicalXpos + (nodes[nodeIdB].graphicalWidth / (nodeIdsListC.Count() + 1) * nodeCcounter), nodes[nodeIdB].graphicalYpos + nodes[nodeIdB].graphicalHeight);
                            g.DrawRectangle(penBox, nodes[nodeIdC].graphicalXpos, nodes[nodeIdC].graphicalYpos, nodes[nodeIdC].graphicalWidth, nodes[nodeIdC].graphicalHeight);
                            g.DrawString(nodes[nodeIdC].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);

                            List<int> nodeIdsListD = new List<int>();
                            nodeIdsListD = nodes[nodeIdC].childNodeIds;
                            nodeDcounter = 0;
                            foreach (int nodeIdD in nodeIdsListD)
                            {
                                if (nodes[nodeIdD].deleted || nodes[nodeIdD].filler) { continue; }
                                nodeDcounter++;
                                if (nodes[nodeIdD].fitmentElementType == "vcdb") { penBox = penVCdbElement; } if (nodes[nodeIdD].fitmentElementType == "qdb") { penBox = penQdbElement; } if (nodes[nodeIdD].fitmentElementType == "note") { penBox = penNoteElement; } if (nodes[nodeIdD].app != null) { penBox = penAppElement; if (nodes[nodeIdD].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } }
                                fitmentBoxLayoutRcetangle.X = nodes[nodeIdD].graphicalXpos; fitmentBoxLayoutRcetangle.Y = nodes[nodeIdD].graphicalYpos; fitmentBoxLayoutRcetangle.Width = nodes[nodeIdD].graphicalWidth+10; fitmentBoxLayoutRcetangle.Height = nodes[nodeIdD].graphicalHeight;
                                g.DrawLine(aces.fitmentBranchPen(nodeIdC, nodes, true), nodes[nodeIdD].graphicalXpos + (nodes[nodeIdD].graphicalWidth / 2), nodes[nodeIdD].graphicalYpos, nodes[nodeIdC].graphicalXpos + (nodes[nodeIdC].graphicalWidth / (nodeIdsListD.Count() + 1) * nodeDcounter), nodes[nodeIdC].graphicalYpos + nodes[nodeIdC].graphicalHeight);
                                g.DrawRectangle(penBox, nodes[nodeIdD].graphicalXpos, nodes[nodeIdD].graphicalYpos, nodes[nodeIdD].graphicalWidth, nodes[nodeIdD].graphicalHeight);
                                g.DrawString(nodes[nodeIdD].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);

                                List<int> nodeIdsListE = new List<int>();
                                nodeIdsListE = nodes[nodeIdD].childNodeIds;
                                nodeEcounter = 0;
                                foreach (int nodeIdE in nodeIdsListE)
                                {
                                    if (nodes[nodeIdE].deleted || nodes[nodeIdE].filler) { continue; }
                                    nodeEcounter++;
                                    if (nodes[nodeIdE].fitmentElementType == "vcdb") { penBox = penVCdbElement; } if (nodes[nodeIdE].fitmentElementType == "qdb") { penBox = penQdbElement; } if (nodes[nodeIdE].fitmentElementType == "note") { penBox = penNoteElement; } if (nodes[nodeIdE].app != null) { penBox = penAppElement; if (nodes[nodeIdE].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } }
                                    fitmentBoxLayoutRcetangle.X = nodes[nodeIdE].graphicalXpos;  fitmentBoxLayoutRcetangle.Y = nodes[nodeIdE].graphicalYpos; fitmentBoxLayoutRcetangle.Width = nodes[nodeIdE].graphicalWidth+10;  fitmentBoxLayoutRcetangle.Height = nodes[nodeIdE].graphicalHeight;
                                    g.DrawLine(aces.fitmentBranchPen(nodeIdD, nodes, true), nodes[nodeIdE].graphicalXpos + (nodes[nodeIdE].graphicalWidth / 2), nodes[nodeIdE].graphicalYpos, nodes[nodeIdD].graphicalXpos + (nodes[nodeIdD].graphicalWidth / (nodeIdsListE.Count() + 1) * nodeEcounter), nodes[nodeIdD].graphicalYpos + nodes[nodeIdD].graphicalHeight);
                                    g.DrawRectangle(penBox, nodes[nodeIdE].graphicalXpos, nodes[nodeIdE].graphicalYpos, nodes[nodeIdE].graphicalWidth, nodes[nodeIdE].graphicalHeight);
                                    g.DrawString(nodes[nodeIdE].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);

                                    List<int> nodeIdsListF = new List<int>();
                                    nodeIdsListF = nodes[nodeIdE].childNodeIds;
                                    nodeFcounter = 0;
                                    foreach (int nodeIdF in nodeIdsListF)
                                    {
                                        if (nodes[nodeIdF].deleted || nodes[nodeIdF].filler) { continue; }
                                        nodeFcounter++;
                                        if (nodes[nodeIdF].fitmentElementType == "vcdb") { penBox = penVCdbElement; }  if (nodes[nodeIdF].fitmentElementType == "qdb") { penBox = penQdbElement; } if (nodes[nodeIdF].fitmentElementType == "note") { penBox = penNoteElement; } if (nodes[nodeIdF].app != null) { penBox = penAppElement; if (nodes[nodeIdF].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } }
                                        fitmentBoxLayoutRcetangle.X = nodes[nodeIdF].graphicalXpos; fitmentBoxLayoutRcetangle.Y = nodes[nodeIdF].graphicalYpos; fitmentBoxLayoutRcetangle.Width = nodes[nodeIdF].graphicalWidth+10; fitmentBoxLayoutRcetangle.Height = nodes[nodeIdF].graphicalHeight;
                                        g.DrawLine(aces.fitmentBranchPen(nodeIdE, nodes, true), nodes[nodeIdF].graphicalXpos + (nodes[nodeIdF].graphicalWidth / 2), nodes[nodeIdF].graphicalYpos, nodes[nodeIdE].graphicalXpos + (nodes[nodeIdE].graphicalWidth / (nodeIdsListF.Count() + 1) * nodeFcounter), nodes[nodeIdE].graphicalYpos + nodes[nodeIdE].graphicalHeight);
                                        g.DrawRectangle(penBox, nodes[nodeIdF].graphicalXpos, nodes[nodeIdF].graphicalYpos, nodes[nodeIdF].graphicalWidth, nodes[nodeIdF].graphicalHeight);
                                        g.DrawString(nodes[nodeIdF].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);

                                        List<int> nodeIdsListG = new List<int>();
                                        nodeIdsListG = nodes[nodeIdF].childNodeIds;
                                        nodeGcounter = 0;
                                        foreach (int nodeIdG in nodeIdsListG)
                                        {
                                            if (nodes[nodeIdG].deleted || nodes[nodeIdG].filler) { continue; }
                                            nodeGcounter++;
                                            if (nodes[nodeIdG].fitmentElementType == "vcdb") { penBox = penVCdbElement; } if (nodes[nodeIdG].fitmentElementType == "qdb") { penBox = penQdbElement; } if (nodes[nodeIdG].fitmentElementType == "note") { penBox = penNoteElement; } if (nodes[nodeIdG].app != null) { penBox = penAppElement; if (nodes[nodeIdG].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } }
                                            fitmentBoxLayoutRcetangle.X = nodes[nodeIdG].graphicalXpos; fitmentBoxLayoutRcetangle.Y = nodes[nodeIdG].graphicalYpos; fitmentBoxLayoutRcetangle.Width = nodes[nodeIdG].graphicalWidth+10; fitmentBoxLayoutRcetangle.Height = nodes[nodeIdG].graphicalHeight;
                                            g.DrawLine(aces.fitmentBranchPen(nodeIdF, nodes, true), nodes[nodeIdG].graphicalXpos + (nodes[nodeIdG].graphicalWidth / 2), nodes[nodeIdG].graphicalYpos, nodes[nodeIdF].graphicalXpos + (nodes[nodeIdF].graphicalWidth / (nodeIdsListG.Count() + 1) * nodeGcounter), nodes[nodeIdF].graphicalYpos + nodes[nodeIdF].graphicalHeight);
                                            g.DrawRectangle(penBox, nodes[nodeIdG].graphicalXpos, nodes[nodeIdG].graphicalYpos, nodes[nodeIdG].graphicalWidth, nodes[nodeIdG].graphicalHeight);
                                            g.DrawString(nodes[nodeIdG].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);

                                            List<int> nodeIdsListH = new List<int>();
                                            nodeIdsListH = nodes[nodeIdG].childNodeIds;
                                            nodeHcounter = 0;
                                            foreach (int nodeIdH in nodeIdsListH)
                                            {
                                                if (nodes[nodeIdH].deleted || nodes[nodeIdH].filler) { continue; }
                                                nodeHcounter++;
                                                if (nodes[nodeIdH].fitmentElementType == "vcdb") { penBox = penVCdbElement; } if (nodes[nodeIdH].fitmentElementType == "qdb") { penBox = penQdbElement; } if (nodes[nodeIdH].fitmentElementType == "note") { penBox = penNoteElement; } if (nodes[nodeIdH].app != null) { penBox = penAppElement; if (nodes[nodeIdH].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } }
                                                
                                                fitmentBoxLayoutRcetangle.X = nodes[nodeIdH].graphicalXpos; fitmentBoxLayoutRcetangle.Y = nodes[nodeIdH].graphicalYpos; fitmentBoxLayoutRcetangle.Width = nodes[nodeIdH].graphicalWidth + 10; fitmentBoxLayoutRcetangle.Height = nodes[nodeIdG].graphicalHeight;
                                                g.DrawLine(aces.fitmentBranchPen(nodeIdG, nodes, true), nodes[nodeIdH].graphicalXpos + (nodes[nodeIdH].graphicalWidth / 2), nodes[nodeIdH].graphicalYpos, nodes[nodeIdG].graphicalXpos + (nodes[nodeIdG].graphicalWidth / (nodeIdsListH.Count() + 1) * nodeHcounter), nodes[nodeIdG].graphicalYpos + nodes[nodeIdG].graphicalHeight);
                                                g.DrawRectangle(penBox, nodes[nodeIdH].graphicalXpos, nodes[nodeIdH].graphicalYpos, nodes[nodeIdH].graphicalWidth, nodes[nodeIdH].graphicalHeight);
                                                g.DrawString(nodes[nodeIdH].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);
                                                                                                
                                                List<int> nodeIdsListI = new List<int>();
                                                nodeIdsListI = nodes[nodeIdH].childNodeIds;
                                                nodeIcounter = 0;
                                                foreach (int nodeIdI in nodeIdsListI)
                                                {
                                                    if (nodes[nodeIdI].deleted || nodes[nodeIdI].filler) { continue; }
                                                    nodeIcounter++;
                                                    if (nodes[nodeIdI].fitmentElementType == "vcdb") { penBox = penVCdbElement; }
                                                    if (nodes[nodeIdI].fitmentElementType == "qdb") { penBox = penQdbElement; }
                                                    if (nodes[nodeIdI].fitmentElementType == "note") { penBox = penNoteElement; }
                                                    if (nodes[nodeIdI].app != null) { penBox = penAppElement; if (nodes[nodeIdI].app.id == fitmentProblemAppIdInView) { penBox = penAppElementHighlited; } }
                                                    
                                                    fitmentBoxLayoutRcetangle.X = nodes[nodeIdI].graphicalXpos; fitmentBoxLayoutRcetangle.Y = nodes[nodeIdI].graphicalYpos; fitmentBoxLayoutRcetangle.Width = nodes[nodeIdI].graphicalWidth + 10; fitmentBoxLayoutRcetangle.Height = nodes[nodeIdH].graphicalHeight;
                                                    g.DrawLine(aces.fitmentBranchPen(nodeIdH, nodes, true), nodes[nodeIdI].graphicalXpos + (nodes[nodeIdI].graphicalWidth / 2), nodes[nodeIdI].graphicalYpos, nodes[nodeIdH].graphicalXpos + (nodes[nodeIdH].graphicalWidth / (nodeIdsListI.Count() + 1) * nodeIcounter), nodes[nodeIdH].graphicalYpos + nodes[nodeIdH].graphicalHeight);
                                                    g.DrawRectangle(penBox, nodes[nodeIdI].graphicalXpos, nodes[nodeIdI].graphicalYpos, nodes[nodeIdI].graphicalWidth, nodes[nodeIdI].graphicalHeight);
                                                    g.DrawString(nodes[nodeIdI].fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (aces.fitmentNodeBoxesGraphicallyOverlap(nodes))
                {
                    //g.DrawString("boxes touch!", fitmentElementTitleFont, fitmentElementTitleBrush, 10, 40, fitmentElementTitleStringFormat);
                }

                if (!treeNodeBeingDragged.deleted)
                {
                    penBox = penNoteElement;
                    fitmentBoxLayoutRcetangle.X = treeNodeBeingDragged.graphicalXpos;
                    fitmentBoxLayoutRcetangle.Y = treeNodeBeingDragged.graphicalYpos; fitmentBoxLayoutRcetangle.Width = treeNodeBeingDragged.graphicalWidth + 10; fitmentBoxLayoutRcetangle.Height = treeNodeBeingDragged.graphicalHeight;
                    g.DrawRectangle(penBox, treeNodeBeingDragged.graphicalXpos, treeNodeBeingDragged.graphicalYpos, treeNodeBeingDragged.graphicalWidth, treeNodeBeingDragged.graphicalHeight);
                    g.DrawString(treeNodeBeingDragged.fitmentElementString, fitmentElementTitleFont, fitmentElementTitleBrush, fitmentBoxLayoutRcetangle, fitmentElementTitleStringFormat);
                }
            }
        }


        private void pictureBoxFitmentTree_MouseDown(object sender, MouseEventArgs e)
        {//roll through all nodes in the nodes in the aces.fitmentNodes list to see if our click XY fell within the bounds of a specific node
            int i; mouseDownX = e.X; mouseDownY = e.Y;
            treeCanvasIsBeingDragged = true;
            pictureBoxFitmentTree.Invalidate();
            for (i=0; i<=aces.fitmentNodeList.Count()-1;i++)
            {
                if (aces.fitmentNodeList[i].deleted || aces.fitmentNodeList[i].filler) { continue; }
                aces.fitmentNodeList[i].touched = false;
                if (e.X >= aces.fitmentNodeList[i].graphicalXpos && e.X<=(aces.fitmentNodeList[i].graphicalXpos+ aces.fitmentNodeList[i].graphicalWidth) && e.Y>= aces.fitmentNodeList[i].graphicalYpos && e.Y<= (aces.fitmentNodeList[i].graphicalYpos+ aces.fitmentNodeList[i].graphicalHeight))
                {// mouse down XY is within the bounds of this node
                    treeCanvasIsBeingDragged = false;

                    if (e.Button == MouseButtons.Left)
                    {
                        if (aces.fitmentNodeList[i].fitmentElementType == "note")
                        {
                            treeNodeBeingDragged.fitmentElementData = aces.fitmentNodeList[i].fitmentElementData;
                            treeNodeBeingDragged.fitmentElementString = aces.fitmentNodeList[i].fitmentElementString;
                            treeNodeBeingDragged.fitmentElementType = aces.fitmentNodeList[i].fitmentElementType;
                            treeNodeBeingDragged.graphicalXpos = aces.fitmentNodeList[i].graphicalXpos + 1;
                            treeNodeBeingDragged.graphicalYpos = aces.fitmentNodeList[i].graphicalYpos - 1;
                            treeNodeBeingDragged.graphicalHeight = aces.fitmentNodeList[i].graphicalHeight;
                            treeNodeBeingDragged.graphicalWidth = aces.fitmentNodeList[i].graphicalWidth;
                            treeNodeBeingDragged.nodeId = aces.fitmentNodeList[i].nodeId;
                            treeNodeBeingDragged.deleted = false;
                        }
                    }
                    
                    /*
                    aces.fitmentNodeList[i].touched = true;
                    if (e.Button == MouseButtons.Left)
                    {
                        aces.fitmentNodeList[i].clickType = 1;
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        aces.fitmentNodeList[i].clickType = 2;
                    }
                    if (e.Button == MouseButtons.Middle)
                    {
                        aces.fitmentNodeList[i].clickType = 3;
                    }
                    */

                }
            }



        }


        private void pictureBoxFitmentTree_MouseUp(object sender, MouseEventArgs e)
        {
            int i;

            if (treeCanvasIsBeingDragged)
            {
                treeCanvasIsBeingDragged = false;
                treeCanvasXbase += (e.X-mouseDownX);
                treeCanvasYbase += (e.Y-mouseDownY);
                treeCanvasXoffset = 0; treeCanvasYoffset = 0;
            }

            if (!treeNodeBeingDragged.deleted)
            {

                for (i = 0; i <= aces.fitmentNodeList.Count() - 1; i++)
                {
                    if (e.X >= aces.fitmentNodeList[i].graphicalXpos && e.X <= (aces.fitmentNodeList[i].graphicalXpos + aces.fitmentNodeList[i].graphicalWidth) && e.Y >= aces.fitmentNodeList[i].graphicalYpos && e.Y <= (aces.fitmentNodeList[i].graphicalYpos + aces.fitmentNodeList[i].graphicalHeight))
                    {// mouse up XY is within the bounds of this node
                        if (treeNodeBeingDragged.nodeId != aces.fitmentNodeList[i].nodeId && aces.fitmentNodeList[i].fitmentElementType == "note" && treeNodeBeingDragged.fitmentElementData!= aces.fitmentNodeList[i].fitmentElementData)
                        {// dragged node was dropped on aonther (not equal) note node
                            // put these two into a group in the cachefile


                            /*
                            // check the cache for the existance of either note already existing
                            if(aces.noteGroupings.ContainsKey(treeNodeBeingDragged.fitmentElementString))
                            {// the being-dragged node already is represented in the cache. see if these two notes already share a group

                                if(aces.noteGroupings.ContainsKey(aces.fitmentNodeList[i].fitmentElementString))
                                {//the dropped-on node already is represented in the cache. 

                                    if(aces.noteGroupings[treeNodeBeingDragged.fitmentElementString] != aces.noteGroupings[aces.fitmentNodeList[i].fitmentElementString])
                                    {// dragged and dropped nodes are note already represented together in a group in the cache - add them under a new group id


                                    }
                                }
                            }
                            
                            Guid groupGUID = Guid.NewGuid();
                            foreach (fitmentNode myNode in aces.fitmentNodeList)
                            {
                                if (myNode.fitmentElementType == "note")
                                {
                                    if (!aces.noteGroupings.ContainsKey(myNode.fitmentElementString))
                                    {
                                        aces.noteGroupings.Add(myNode.fitmentElementString, groupGUID.ToString());
                                    }
                                }
                            }

                            if (aces.noteGroupings.Count() > 4)
                            {
                                aces.writeNoteGroupingsCache(lblCachePath.Text + @"\ACESinspector-notegroupings.txt");
                            }

    */






                        }
                    }
                }


                treeNodeBeingDragged.deleted = true;
            }

            pictureBoxFitmentTree.Invalidate();

        }

        private void pictureBoxFitmentTree_MouseMove(object sender, MouseEventArgs e)
        {
            if (!treeNodeBeingDragged.deleted)
            {
                treeNodeBeingDragged.graphicalXpos = e.X;
                treeNodeBeingDragged.graphicalYpos = e.Y;
                pictureBoxFitmentTree.Invalidate();
            }

            if(treeCanvasIsBeingDragged)
            {
                treeCanvasXoffset = (e.X - mouseDownX);
                treeCanvasYoffset = (e.Y - mouseDownY);
                pictureBoxFitmentTree.Invalidate();
            }
        }


        private void btnSelectCacheDir_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key.CreateSubKey("ACESinspector");
                key = key.OpenSubKey("ACESinspector", true);
                if (key.GetValue("cacheDirectoryPath") != null) { fbd.SelectedPath = key.GetValue("cacheDirectoryPath").ToString(); }
                DialogResult dialogResult = fbd.ShowDialog();
                if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    key.SetValue("cacheDirectoryPath", fbd.SelectedPath);
                    lblCachePath.Text = fbd.SelectedPath;
                    if(lblAssessmentsPath.Text == "")
                    {
                        lblAssessmentsPath.Text = fbd.SelectedPath;
                        key.SetValue("assessmentDirectoryPath", fbd.SelectedPath);
                    }
                }
            }
        }

        private void checkBoxRespectValidateTag_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true); key.CreateSubKey("ACESinspector"); key = key.OpenSubKey("ACESinspector", true);
            if (checkBoxRespectValidateTag.Checked) { key.SetValue("respectValidateNoTag", "1"); } else { key.SetValue("respectValidateNoTag", "0"); }
        }

        private void splitContainerFitmentLogic_SplitterMoved(object sender, SplitterEventArgs e)
        {
            dgFitmentLogicProblems.Height = splitContainerFitmentLogic.Panel1.Height;
            pictureBoxFitmentTree.Height = splitContainerFitmentLogic.Panel2.Height;
            listBoxFitmentLogicElements.Height = splitContainerFitmentLogic.Panel2.Height;
        }

        private void numericUpDownQtyOutliersThreshold_ValueChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            key.SetValue("qtyOutliersThreshold", numericUpDownQtyOutliersThreshold.Value);
        }

        private void checkBoxQtyOutliers_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if(checkBoxQtyOutliers.Checked)
            {
                key.SetValue("detectQtyOutliers", "1");
            }
            else
            {
                key.SetValue("detectQtyOutliers", "0");
            }
        }

        private void numericUpDownQtyOutliersSample_ValueChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            key.SetValue("qtyOutliersSampleSize", numericUpDownQtyOutliersSample.Value);
        }

        private void listBoxFitmentLogicElements_Click(object sender, EventArgs e)
        {
        }

        private void listBoxFitmentLogicElements_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBoxFitmentLogicElements.Items.Count > 1)
            {
                if(listBoxFitmentLogicElements.SelectedIndex>0)
                {

                    //float the selected list item up 
                    var temp = listBoxFitmentLogicElements.Items[listBoxFitmentLogicElements.SelectedIndex];
                    listBoxFitmentLogicElements.Items[listBoxFitmentLogicElements.SelectedIndex] = listBoxFitmentLogicElements.Items[listBoxFitmentLogicElements.SelectedIndex - 1];
                    listBoxFitmentLogicElements.Items[listBoxFitmentLogicElements.SelectedIndex-1] = temp;
                    listBoxFitmentLogicElements.SelectedIndex--;


                    int elementPrevalence = 0;
                    Dictionary<string, int> fitmentElementPrevalence = new Dictionary<string, int>();
                    List<string> tempStringList = new List<string>();

                    for (int i =0; i < listBoxFitmentLogicElements.Items.Count; i++)
                    {
                        tempStringList.Add(listBoxFitmentLogicElements.Items[i].ToString());
                    }

                    aces.fitmentProblemGroupsBestPermutations[macroProblemGroupKeyInView] = tempStringList;

                    aces.fitmentNodeList.Clear();

                    foreach (string fitmentElement in aces.fitmentProblemGroupsBestPermutations[macroProblemGroupKeyInView])
                    {
                        fitmentElementPrevalence.Add(fitmentElement, elementPrevalence); elementPrevalence++;
                    }
                    aces.fitmentNodeList.AddRange(aces.buildFitmentTreeFromAppList(aces.fitmentProblemGroupsAppLists[macroProblemGroupKeyInView], fitmentElementPrevalence, -1, true, true, vcdb, qdb));
                    listBoxFitmentLogicElements.Items.Clear();
                    listBoxFitmentLogicElements.Items.AddRange(fitmentElementPrevalence.Keys.ToArray());
                    pictureBoxFitmentTree.Invalidate();
                    treeCanvasXbase = 0; treeCanvasYbase = 0;


                    // record this high-value human-supplied sequence intel in the permutations cache

                    // cache key is basevid,parttype,position

                    App firstAppInGroup =  aces.fitmentProblemGroupsAppLists[macroProblemGroupKeyInView][0];
                    string cacheHashkey = firstAppInGroup.basevehilceid.ToString() + "," + firstAppInGroup.parttypeid.ToString() + "," + firstAppInGroup.positionid.ToString();

                    if (!aces.fitmentPermutationMiningCache.ContainsKey(cacheHashkey))
                    {
                        aces.fitmentPermutationMiningCache.Add(cacheHashkey, string.Join("|", aces.fitmentProblemGroupsBestPermutations[macroProblemGroupKeyInView]));
                    }
                    else
                    {
                        aces.fitmentPermutationMiningCache[cacheHashkey] = string.Join("|", aces.fitmentProblemGroupsBestPermutations[macroProblemGroupKeyInView]);
                    }
                    aces.addedToFitmentPermutationMineingCache = true;
                }
            }
        }

        private void checkBoxConcernForVCdbVCdb_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true); key.CreateSubKey("ACESinspector"); key = key.OpenSubKey("ACESinspector", true);
            if (checkBoxConcernForDisparate.Checked){key.SetValue("concernForDisparateBranches", "1");}else{key.SetValue("concernForDisparateBranches", "0");}
        }

        private void checkBoxReportAllAppsInProblemGroup_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true); key.CreateSubKey("ACESinspector"); key = key.OpenSubKey("ACESinspector", true);
            if (checkBoxReportAllAppsInProblemGroup.Checked) { key.SetValue("reportAllAppsInProblemGroup", "1"); } else { key.SetValue("reportAllAppsInProblemGroup", "0"); }
        }

        private void numericUpDownTreeConfigLimit_ValueChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true); key.CreateSubKey("ACESinspector"); key = key.OpenSubKey("ACESinspector", true);
            key.SetValue("treePermutationsLimit", numericUpDownTreeConfigLimit.Value);
        }

        private void checkBoxExplodeNotes_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true); key.CreateSubKey("ACESinspector"); key = key.OpenSubKey("ACESinspector", true);
            if (checkBoxExplodeNotes.Checked) { key.SetValue("explodeNoteTagsBySemicolon", "1"); } else { key.SetValue("explodeNoteTagsBySemicolon", "0"); }
        }

        private void checkBoxUKgrace_CheckedChanged(object sender, EventArgs e)
        {
            aces.allowGraceForWildcardConfigs = checkBoxUKgrace.Checked;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true); key.CreateSubKey("ACESinspector"); key = key.OpenSubKey("ACESinspector", true);
            if (checkBoxUKgrace.Checked) { key.SetValue("allowGraceForWildcardConfigs", "1"); } else { key.SetValue("allowGraceForWildcardConfigs", "0"); }
        }

        private void checkBoxAutoloadDatabases_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true); key.CreateSubKey("ACESinspector"); key = key.OpenSubKey("ACESinspector", true);
            if (checkBoxAutoloadLocalDatabases.Checked) { key.SetValue("autoloadReferenceDatabases", "1"); } else { key.SetValue("autoloadReferenceDatabases", "0"); }
        }

        private void checkBoxLimitDataGridRows_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true); key.CreateSubKey("ACESinspector"); key = key.OpenSubKey("ACESinspector", true);
            if (checkBoxLimitDataGridRows.Checked) { key.SetValue("limitDatagridRows", "1"); } else { key.SetValue("limitDatagridRows", "0"); }
        }

        private void btnSelectNoteInterchangeFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if (key.GetValue("lastNoteTranslationDirectoryPath") != null) { openFileDialog.InitialDirectory = key.GetValue("lastNoteTranslationDirectoryPath").ToString(); }

            openFileDialog.Title = "Open note translation text file";
            openFileDialog.RestoreDirectory = false;
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            DialogResult openFileResult = openFileDialog.ShowDialog();
            if (openFileResult.ToString() == "OK")
            {
                noteTranslationDictionary.Clear();
                key.SetValue("lastNoteTranslationDirectoryPath", Path.GetDirectoryName(openFileDialog.FileName));
                importNoteTranslation(openFileDialog.FileName);
                if (noteTranslationDictionary.Count() > 0)
                {
                    lblNoteTranslationfilePath.Text = Path.GetFileName(openFileDialog.FileName) + "   (Contains " + noteTranslationDictionary.Count().ToString() + " note translation records)";
                }
                else
                {
                    lblNoteTranslationfilePath.Text = "";
                    MessageBox.Show(openFileDialog.FileName + " does not contain any properly formatted note translation records.\r\n\r\nThe required format is:\r\n  input note string  <tab>  output note string\r\n\r\nThe file must contain exactly two columns and contain no header row. Every <Note> node in the imported ACES file will be checked against this list and replaced by the contents of the second column if found. The second column can be blank - this would effectively remove every instance of the note in the fist column.");
                }
            }
        }

        private void dgFitmentLogicProblems_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            // extract the overlapgroup number from the second column and use it to extract the list of apps that make up that group from the aces.overlapgroups dictionary
            // this dictionary was populated by the overlaps detection function when "Analyze" was run
            // the small list of apps that make up a specific overlap group is used to build a tree in aces.fitmentNodes list by the function aces.buildOperlapsTreeFromAppList()

            int elementPrevalence = 0;
            Dictionary<string, int> fitmentElementPrevalence = new Dictionary<string, int>();
            //List<fitmentNode> nodeslist = new List<fitmentNode>();
            if (e.RowIndex >= 0)
            {
                macroProblemGroupKeyInView = dgFitmentLogicProblems.Rows[e.RowIndex].Cells[1].Value.ToString();
                fitmentProblemAppIdInView = Convert.ToInt32(dgFitmentLogicProblems.Rows[e.RowIndex].Cells[2].Value.ToString());
                if (aces.fitmentProblemGroupsAppLists.ContainsKey(macroProblemGroupKeyInView))
                {
                    aces.fitmentNodeList.Clear();
                    listBoxFitmentLogicElements.Items.Clear();

                    if (aces.fitmentProblemGroupsBestPermutations.ContainsKey(macroProblemGroupKeyInView))
                    {
                        foreach (string fitmentElement in aces.fitmentProblemGroupsBestPermutations[macroProblemGroupKeyInView])
                        {
                            fitmentElementPrevalence.Add(fitmentElement, elementPrevalence); elementPrevalence++;
                            listBoxFitmentLogicElements.Items.Add(fitmentElement);
                        }
                        aces.fitmentNodeList.AddRange(aces.buildFitmentTreeFromAppList(aces.fitmentProblemGroupsAppLists[macroProblemGroupKeyInView], fitmentElementPrevalence, -1, true, true, vcdb, qdb));
                    }
                    pictureBoxFitmentTree.Invalidate();
                    treeCanvasXbase = 0; treeCanvasYbase = 0;
                }
                else
                {
                    macroProblemGroupKeyInView = "-1";
                }
            }
        }

        private void timerHistoryUpdate_Tick(object sender, EventArgs e)
        {// 200mS interval for updating history list on stats tab and watching for analysis results to roll back in from background threads

            if(vcdb.importIsRunning)
            {
                ReportVCdbImportProgress(vcdb.importProgress);
            }


            if (aces.analysisRunning)
            {
                aces.analysisTime++;
                int completeChunksCount =0;
                int globalChunksComlete = 0;
                bool individualAnalysisComplete = false;
                bool macroAnalysisComplete = false;
                pictureBoxLogicProblems.Invalidate();
                pictureBoxCommonErrors.Invalidate();

                //------------------ individual analysis -----------------------------

                foreach (analysisChunk chunk in aces.individualAnanlysisChunksList)
                {
                    if (chunk.complete) { completeChunksCount++;}
                }

                if (completeChunksCount == aces.individualAnanlysisChunksList.Count()){individualAnalysisComplete = true;}
                lblIndividualErrors.Text = ((completeChunksCount * 100) / aces.individualAnanlysisChunksList.Count()).ToString() + "% complete";

                //------------------ macro analysis -----------------------------

                globalChunksComlete += completeChunksCount;
                completeChunksCount = 0;
                foreach (analysisChunkGroup chunkGroup in aces.fitmentAnalysisChunksGroups)
                {
                    if (chunkGroup.complete){completeChunksCount++;}
                }

                foreach (analysisChunk chunk in aces.outlierAnanlysisChunksList)
                {
                    if (chunk.complete) { completeChunksCount++;}
                }


                if (completeChunksCount == (aces.fitmentAnalysisChunksGroups.Count() + aces.outlierAnanlysisChunksList.Count())){macroAnalysisComplete = true;}

                globalChunksComlete += completeChunksCount;
                lblMacroProblems.Text = ((completeChunksCount * 100) / (aces.fitmentAnalysisChunksGroups.Count() + aces.outlierAnanlysisChunksList.Count())).ToString() + "% complete";
                //lblAnalyzeStatus.Text = ((globalChunksComlete * 100) / (aces.individualAnanlysisChunksList.Count() + aces.fitmentAnalysisChunksGroups.Count() + aces.outlierAnanlysisChunksList.Count())).ToString() + "%";


//                if (macroAnalysisComplete && individualAnalysisComplete)
  //              {
       //             progBarAnalyze.Visible = false; lblAnalyzeStatus.Visible = false;
    //            }
            }

            if (aces.analysisHistory.Count() > historyLineCountAtLastCheck)
            {
                try
                {
                    historyLineCountAtLastCheck = aces.analysisHistory.Count();
                    textBoxAnalysisHostory.Text = "";
                    for (int i = 0; i < aces.analysisHistory.Count(); i++)
                    {
                        if (aces.analysisHistory[i] == null) { continue; }
                        var fields = aces.analysisHistory[i].Split('\t');
                        if (aces.logLevel >= Convert.ToInt32(fields[0]))
                        {
                            textBoxAnalysisHostory.Text += fields[1] + "\r\n";
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }



        // this picurebox is used to represent all the chunks that represent the whole file's individual app analysis work
        private void pictureBoxCommonErrors_Paint(object sender, PaintEventArgs e)
        {
            if (aces.individualAnanlysisChunksList.Count() > 0)
            {
                Graphics g = e.Graphics;
                Brush penIncomplete = new SolidBrush(Color.Gray);
                Brush penCompleteClean = new SolidBrush(Color.Green);
                Brush penCompleteErrors = new SolidBrush(Color.Red);

                int blockSize = Convert.ToInt32(550 / (aces.individualAnanlysisChunksList.Count()+1)); //number of pixels wide each work block in the bargraph
                if (blockSize < 9) { blockSize = 9; }


                pictureBoxCommonErrors.Width -= (pictureBoxCommonErrors.Width - (aces.individualAnanlysisChunksList.Count() * blockSize));

                for (int i = 0; i < aces.individualAnanlysisChunksList.Count(); i++)
                {
                    if (!aces.individualAnanlysisChunksList[i].complete)
                    {
                        g.FillRectangle(penIncomplete, (i*blockSize), 0, blockSize, pictureBoxCommonErrors.Height);
                    }
                    else
                    {
                        int chunkTotalErrorCount = aces.individualAnanlysisChunksList[i].basevehicleidsErrorsCount + aces.individualAnanlysisChunksList[i].parttypePositionErrorsCount + aces.individualAnanlysisChunksList[i].qdbErrorsCount + aces.individualAnanlysisChunksList[i].vcdbCodesErrorsCount + aces.individualAnanlysisChunksList[i].vcdbConfigurationsErrorsCount;

                        if (chunkTotalErrorCount == 0)
                        {// whole block is green
                            g.FillRectangle(penCompleteClean, (i*blockSize), 0, blockSize, pictureBoxCommonErrors.Height);
                        }
                        else
                        {// mix of red/green in this block
                            double goodPixels=(-15)*Math.Log10(((double)(aces.individualAnanlysisChunksList[i].vcdbCodesErrorsCount+ 
                                aces.individualAnanlysisChunksList[i].vcdbConfigurationsErrorsCount+ 
                                aces.individualAnanlysisChunksList[i].parttypePositionErrorsCount+
                                aces.individualAnanlysisChunksList[i].qdbErrorsCount+
                                aces.individualAnanlysisChunksList[i].basevehicleidsErrorsCount) /
                                (double)(aces.individualAnanlysisChunksList[i].appsList.Count() * 5)));

                            float badPixels = (float)pictureBoxCommonErrors.Height - (float)goodPixels;
                            g.FillRectangle(penCompleteClean, (i * blockSize), 0, blockSize,  (float)goodPixels);
                            g.FillRectangle(penCompleteErrors, (i * blockSize), (float)goodPixels, blockSize, badPixels);
                        }
                    }
                }
                g.DrawRectangle(Pens.Black, 0, 0, pictureBoxCommonErrors.Width - 1, pictureBoxCommonErrors.Height - 1);
            }

        }

        private void pictureBoxLogicProblems_Paint(object sender, PaintEventArgs e)
        {
            if (aces.fitmentAnalysisChunksGroups.Count() > 0)
            {
                Graphics g = e.Graphics;
                Brush penIncomplete = new SolidBrush(Color.Gray);
                Brush penCompleteClean = new SolidBrush(Color.Green);
                Brush penCompleteWarnings = new SolidBrush(Color.Yellow);
                Brush penCompleteErrors = new SolidBrush(Color.Red);
                int blockSize = Convert.ToInt32(550 / (aces.fitmentAnalysisChunksGroups.Count() + aces.outlierAnanlysisChunksList.Count() + 1)); //number of pixels wide each work block in the bargraph
                if (blockSize < 9) { blockSize = 9; }


                int blockOffset = 0;
                pictureBoxLogicProblems.Width -= (pictureBoxLogicProblems.Width - ((aces.fitmentAnalysisChunksGroups.Count()+ aces.outlierAnanlysisChunksList.Count())  * blockSize));


                for (int i = 0; i < aces.outlierAnanlysisChunksList.Count(); i++)
                {
                    if (!aces.outlierAnanlysisChunksList[i].complete)
                    {
                        g.FillRectangle(penIncomplete, ((i + blockOffset) * blockSize), 0, blockSize, pictureBoxLogicProblems.Width);
                    }
                    else
                    {// this block (chunkGroup) is complete
                        if ((aces.outlierAnanlysisChunksList[i].parttypeDisagreementErrorsCount + aces.outlierAnanlysisChunksList[i].qtyOutlierCount) == 0)
                        {// no warnings
                            g.FillRectangle(penCompleteClean, ((i + blockOffset) * blockSize), 0, blockSize, pictureBoxLogicProblems.Width);
                        }
                        else
                        {
                            g.FillRectangle(penCompleteWarnings, ((i + blockOffset) * blockSize), 0, blockSize, pictureBoxLogicProblems.Width);
                        }
                    }
                    blockOffset = i;
                }

                blockOffset++;

                for (int i = 0; i < aces.fitmentAnalysisChunksGroups.Count(); i++)
                {
                    if (!aces.fitmentAnalysisChunksGroups[i].complete)
                    {
                        g.FillRectangle(penIncomplete, ((i + blockOffset) * blockSize), 0, blockSize, pictureBoxLogicProblems.Width);
                    }
                    else
                    {// this block (chunkGroup) is complete
                        if (aces.fitmentAnalysisChunksGroups[i].errorsCount == 0)
                        {// no errors - check for warnings
                            if (aces.fitmentAnalysisChunksGroups[i].warningsCount == 0)
                            {
                                g.FillRectangle(penCompleteClean, ((i + blockOffset) * blockSize), 0, blockSize, pictureBoxLogicProblems.Width);
                            }
                            else
                            {// warnings exist
                                g.FillRectangle(penCompleteWarnings, ((i + blockOffset) * blockSize), 0, blockSize, pictureBoxLogicProblems.Width);
                            }
                        }
                        else
                        {

                            double goodPixels = (-20) * Math.Log10((double)aces.fitmentAnalysisChunksGroups[i].errorsCount / (double)aces.fitmentAnalysisChunksGroups[i].chunkList.Count());

                            float badPixels = (float)pictureBoxLogicProblems.Height - (float)goodPixels;
//                            g.FillRectangle(penCompleteClean, (i * blockSize), 0, blockSize, (float)goodPixels);
  //                          g.FillRectangle(penCompleteErrors, (i * blockSize), (float)goodPixels, blockSize, badPixels);


                            g.FillRectangle(penCompleteClean, ((i + blockOffset) * blockSize), 0, blockSize, (float)goodPixels);
                            g.FillRectangle(penCompleteErrors, ((i + blockOffset) * blockSize), (float)goodPixels, blockSize, badPixels);
                        }
                    }
                }

                g.DrawRectangle(Pens.Black,0 , 0, pictureBoxLogicProblems.Width-1, pictureBoxLogicProblems.Height-1);
            }

        }

        private void numericUpDownThreads_ValueChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            key.SetValue("threadCount", numericUpDownThreads.Value);
        }

        private void btnSelectAssessmentDir_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                key.CreateSubKey("ACESinspector");
                key = key.OpenSubKey("ACESinspector", true);
                if (key.GetValue("assessmentDirectoryPath") != null) { fbd.SelectedPath = key.GetValue("assessmentDirectoryPath").ToString(); }
                DialogResult dialogResult = fbd.ShowDialog();
                if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    key.SetValue("assessmentDirectoryPath", fbd.SelectedPath);
                    lblAssessmentsPath.Text = fbd.SelectedPath;
                    if(lblCachePath.Text=="")
                    {
                        key.SetValue("cacheDirectoryPath", fbd.SelectedPath);
                        lblCachePath.Text = fbd.SelectedPath;
                    }
                }
            }
        }

        private void buttonMySQLconnect_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);

            getAvailableMySQLdatabaseList();

            comboBoxMySQLvcdbVersion.Items.Clear();
            comboBoxMySQLvcdbVersion.Items.AddRange(vcdb.vcdbVersionsOnServerList.ToArray());
            comboBoxMySQLvcdbVersion.SelectedIndex = 0;
            comboBoxMySQLvcdbVersion.Visible = true;
            btnSelectVCdbFile.Visible = false;
            buttonMySQLloadVCdb.Visible = true;

            comboBoxMySQLpcdbVersion.Items.Clear();
            comboBoxMySQLpcdbVersion.Items.AddRange(pcdb.pcdbVersionsOnServerList.ToArray());
            comboBoxMySQLpcdbVersion.SelectedIndex = 0;
            comboBoxMySQLpcdbVersion.Visible = true;
            buttonMySQLloadPCdb.Visible = true;
            btnSelectPCdbFile.Visible = false;

            comboBoxMySQLqdbVersion.Items.Clear();
            comboBoxMySQLqdbVersion.Items.AddRange(qdb.qdbVersionsOnServerList.ToArray());
            comboBoxMySQLqdbVersion.SelectedIndex = 0;
            comboBoxMySQLqdbVersion.Visible = true;
            buttonMySQLloadQdb.Visible = true;
            btnSelectQdbFile.Visible = false;
        }


        private async void buttonMySQLloadVCdb_Click(object sender, EventArgs e)
        {
            buttonMySQLloadVCdb.Enabled = false;
            progBarVCdbload.Visible = true; lblVCdbFilePath.Text = ""; lblVCdbLoadStatus.Text = "Loading VCdb - 0%";
            vcdb.importIsRunning = true;
            vcdb.clear();
            lblVCdbFilePath.Text = "";
            vcdb.MySQLusername = textBoxMySQLuser.Text.Trim();
            vcdb.MySQLpassword = textBoxMySQLpassword.Text.Trim();
            vcdb.MySQLdatabaseName = comboBoxMySQLvcdbVersion.Items[comboBoxMySQLvcdbVersion.SelectedIndex].ToString();
            vcdb.MySQLconnectionString = "SERVER=" + textBoxMySQLhost.Text.Trim() + ";" + "DATABASE=" + vcdb.MySQLdatabaseName + ";" + "UID=" + textBoxMySQLuser.Text.Trim() + ";" + "PASSWORD=" + textBoxMySQLpassword.Text.Trim() + ";";
            vcdb.useRemoteDB = true;
            vcdb.addNewMySQLconnection();  // establish the initial connections used for loading the local data dictionaries. this will get distroyed as soon as the import completes. when actual analysis is run, the required number of connections will be instanced on the fly
            await Task.Run(() => importMySQLvcdb());
            progBarVCdbload.Value = 0; progBarVCdbload.Visible = false; vcdb.importIsRunning = false; lblVCdbLoadStatus.Text = "";

            if (vcdb.importSuccess)
            {
                buttonMySQLConnect.Enabled = false;
                lblVCdbFilePath.Text = "Loaded version " + vcdb.version + " from remote database";
                if (pcdb.version != "" && qdb.version != "" && aces.successfulImport) { btnAnalyze.Enabled = true; }
            }
            buttonMySQLloadVCdb.Enabled = true;
        }

  
        private void radioButtonDataSourceMySQL_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if (radioButtonDataSourceMySQL.Checked)
            {
                key.SetValue("datasource", "mysql");
                getAvailableMySQLdatabaseList();
                comboBoxMySQLvcdbVersion.Items.Clear();

                if (vcdb.vcdbVersionsOnServerList.Count() > 0)
                {
                    comboBoxMySQLvcdbVersion.Items.AddRange(vcdb.vcdbVersionsOnServerList.ToArray());
                    comboBoxMySQLvcdbVersion.SelectedIndex = 0;
                    comboBoxMySQLvcdbVersion.Visible = true;
                    buttonMySQLloadVCdb.Visible = true;
                }
                btnSelectVCdbFile.Visible = false;

                comboBoxMySQLpcdbVersion.Items.Clear();
                if (vcdb.vcdbVersionsOnServerList.Count() > 0)
                {
                    comboBoxMySQLpcdbVersion.Items.AddRange(pcdb.pcdbVersionsOnServerList.ToArray());
                    comboBoxMySQLpcdbVersion.SelectedIndex = 0;
                    comboBoxMySQLpcdbVersion.Visible = true;
                    buttonMySQLloadPCdb.Visible = true;
                }
                btnSelectPCdbFile.Visible = false;

                comboBoxMySQLqdbVersion.Items.Clear();
                if (qdb.qdbVersionsOnServerList.Count() > 0)
                {
                    comboBoxMySQLqdbVersion.Items.AddRange(qdb.qdbVersionsOnServerList.ToArray());
                    comboBoxMySQLqdbVersion.SelectedIndex = 0;
                    comboBoxMySQLqdbVersion.Visible = true;
                    buttonMySQLloadQdb.Visible = true;
                }
                btnSelectQdbFile.Visible = false;
                buttonMySQLConnect.Enabled = false;
            }
        }

        private void radioButtonDataSourceAccess_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            if (radioButtonDataSourceAccess.Checked)
            {
                key.SetValue("datasource", "oledbAccess");
                btnSelectVCdbFile.Visible = true;
                vcdb.clear();
                lblVCdbFilePath.Text = "";
                vcdb.useRemoteDB = false;

                btnSelectPCdbFile.Visible = true;
                pcdb.clear();
                lblPCdbFilePath.Text = "";
                pcdb.useRemoteDB = false;
                btnSelectVCdbFile.Focus();

                btnSelectQdbFile.Visible = true;
                qdb.clear();
                lblQdbFilePath.Text = "";
                qdb.useRemoteDB = false;
                btnSelectQdbFile.Focus();
            }
        }

        public int importNoteTranslation(string translationFile)
        {
            using (var reader = new StreamReader(translationFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fields = line.Split('\t');
                    if (fields.Count() == 2 && fields[0].Trim().Length > 0)
                    {//text must be exactly 2 columns seperated by tab
                        if(noteTranslationDictionary.ContainsKey(fields[0].Trim()))
                        {
                            noteTranslationDictionary[fields[0].Trim()] = fields[1].Trim();
                        }
                        else
                        {
                            noteTranslationDictionary.Add(fields[0].Trim(), fields[1].Trim());
                        }
                    }
                }
            }
            return noteTranslationDictionary.Count;
        }

        private async void buttonMySQLloadPCdb_Click(object sender, EventArgs e)
        {
            buttonMySQLloadPCdb.Enabled = false;
            pcdb.clear();
            lblPCdbFilePath.Text = "";
            pcdb.MySQLusername = textBoxMySQLuser.Text.Trim();
            pcdb.MySQLpassword = textBoxMySQLpassword.Text.Trim();
            pcdb.MySQLdatabaseName = comboBoxMySQLpcdbVersion.Items[comboBoxMySQLpcdbVersion.SelectedIndex].ToString();
            pcdb.MySQLconnectionString = "SERVER=" + textBoxMySQLhost.Text.Trim() + ";" + "DATABASE=" + pcdb.MySQLdatabaseName + ";" + "UID=" + textBoxMySQLuser.Text.Trim() + ";" + "PASSWORD=" + textBoxMySQLpassword.Text.Trim() + ";";
            pcdb.useRemoteDB = true;
            pcdb.connectMySQL();
            await Task.Run(() => importMySQLpcdb());

            if (pcdb.importSuccess)
            {
                lblPCdbFilePath.Text = "Loaded version " + pcdb.version + " from remote database";
                if (qdb.version != "" && aces.successfulImport) { btnAnalyze.Enabled = true; }
            }
            buttonMySQLloadPCdb.Enabled = true;
        }

        private async void buttonMySQLloadQdb_Click(object sender, EventArgs e)
        {
            buttonMySQLloadQdb.Enabled = false;
            qdb.clear();
            lblQdbFilePath.Text = "";
            qdb.MySQLusername = textBoxMySQLuser.Text.Trim();
            qdb.MySQLpassword = textBoxMySQLpassword.Text.Trim();
            qdb.MySQLdatabaseName = comboBoxMySQLqdbVersion.Items[comboBoxMySQLqdbVersion.SelectedIndex].ToString();
            qdb.MySQLconnectionString = "SERVER=" + textBoxMySQLhost.Text.Trim() + ";" + "DATABASE=" + qdb.MySQLdatabaseName + ";" + "UID=" + textBoxMySQLuser.Text.Trim() + ";" + "PASSWORD=" + textBoxMySQLpassword.Text.Trim() + ";";
            qdb.useRemoteDB = true;
            qdb.connectMySQL();
            await Task.Run(() => importMySQLqdb());

            if (qdb.importSuccess)
            {
                lblQdbFilePath.Text = "Loaded version " + qdb.version + " from remote database";
                if (pcdb.version != "" && aces.successfulImport) { btnAnalyze.Enabled = true; }
            }
            buttonMySQLloadQdb.Enabled = true;
        }

        private void getAvailableMySQLdatabaseList()
        {
            vcdb.vcdbVersionsOnServerList.Clear();
            pcdb.pcdbVersionsOnServerList.Clear();
            qdb.qdbVersionsOnServerList.Clear();
            // "list" is the assumed name of the db on the server. it must contain a table called "versions" with two colums :name, type. like 'vcdb20180228','vcdb'. this tells the client what vcdb,qdb and pcb databases exist (by name) on the server. when it comes time to actually comsume the reference databases, new connections will be made to those specific databases that the user selects - or that the imported ACES file claims
            string connectionString = "SERVER=" + textBoxMySQLhost.Text.Trim() + ";" + "DATABASE=list" + ";" + "UID=" + textBoxMySQLuser.Text.Trim() + ";" + "PASSWORD=" + textBoxMySQLpassword.Text.Trim() + ";POOLING=FALSE";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand("select `name`,`type` from versions order by `name` desc", connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader["type"].ToString() == "vcdb")
                    {
                        vcdb.vcdbVersionsOnServerList.Add(dataReader["name"].ToString());
                    }

                    if (dataReader["type"].ToString() == "pcdb")
                    {
                        pcdb.pcdbVersionsOnServerList.Add(dataReader["name"].ToString());
                    }

                    if (dataReader["type"].ToString() == "qdb")
                    {
                        qdb.qdbVersionsOnServerList.Add(dataReader["name"].ToString());
                    }
                }
                dataReader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0: MessageBox.Show("Cannot connect to server."); break;
                    case 1045: MessageBox.Show("Invalid username/password"); break;
                    default: MessageBox.Show("MySQL error (1): " + ex.Message); break;
                }
            }
        }

        private void textBoxMySQLhost_Leave(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            key.SetValue("MySQLhost", textBoxMySQLhost.Text.Trim());
        }

        private void textBoxMySQLpassword_Leave(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            key.SetValue("MySQLpassword", textBoxMySQLpassword.Text.Trim());
        }

        private void textBoxMySQLuser_Leave(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key.CreateSubKey("ACESinspector");
            key = key.OpenSubKey("ACESinspector", true);
            key.SetValue("MySQLuser", textBoxMySQLuser.Text.Trim());
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnDistinctVCdbExportSave_Click(object sender, EventArgs e)
        {
            string result = "";
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult dialogResult = fbd.ShowDialog();
                if (dialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                    key.CreateSubKey("ACESinspector");
                    key = key.OpenSubKey("ACESinspector", true);
                    key.SetValue("lastHolesDirectoryPath", fbd.SelectedPath);
                    string VCdbCodeStatsFilename = fbd.SelectedPath + "\\" + Path.GetFileNameWithoutExtension(aces.filePath) + "_VCdbCodeStats.txt";
                    if (aces.vcdbUsageStatsFileList.Count>1){VCdbCodeStatsFilename = fbd.SelectedPath + "\\VCdbCodeStats.txt";}
                    result = aces.exportVCdbUsageReport(vcdb, VCdbCodeStatsFilename);
                    MessageBox.Show(result);
                }
            }
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


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            aces.writeFitmentPermutationMiningCache(lblCachePath.Text + @"\ACESinspector-fitment permutations.txt");

            // clean up any cache files that were written
            foreach(string cachefile in cacheFilesToDeleteOnExit)
            {
                try{File.Delete(cachefile);}catch(Exception ex) { }
            }

        }

    }
}
