namespace ACESinspector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnSelectACESfile = new System.Windows.Forms.Button();
            this.lblACESfilePath = new System.Windows.Forms.Label();
            this.btnSelectVCdbFile = new System.Windows.Forms.Button();
            this.lblVCdbFilePath = new System.Windows.Forms.Label();
            this.btnSelectPCdbFile = new System.Windows.Forms.Button();
            this.lblPCdbFilePath = new System.Windows.Forms.Label();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.dgParts = new System.Windows.Forms.DataGridView();
            this.dgPartsPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgPartsAppCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgPartsParttypes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgPartsPositions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageStats = new System.Windows.Forms.TabPage();
            this.textBoxAnalysisHostory = new System.Windows.Forms.TextBox();
            this.lblStatsProcessingTime = new System.Windows.Forms.Label();
            this.lblProcessTimeTitle = new System.Windows.Forms.Label();
            this.pictureBoxCommonErrors = new System.Windows.Forms.PictureBox();
            this.lblIndividualErrors = new System.Windows.Forms.Label();
            this.lblIndividualErrorsTitle = new System.Windows.Forms.Label();
            this.lblDifferentialsSummary = new System.Windows.Forms.Label();
            this.lblDifferentialsLabel = new System.Windows.Forms.Label();
            this.progressBarDifferentials = new System.Windows.Forms.ProgressBar();
            this.pictureBoxLogicProblems = new System.Windows.Forms.PictureBox();
            this.lblMacroProblems = new System.Windows.Forms.Label();
            this.lblMacroProblemsTitle = new System.Windows.Forms.Label();
            this.lblStatsPartsCount = new System.Windows.Forms.Label();
            this.lblStatsAppsCount = new System.Windows.Forms.Label();
            this.lblStatsQdbVersion = new System.Windows.Forms.Label();
            this.lblStatsPCdbVersion = new System.Windows.Forms.Label();
            this.lblStatsVCdbVersion = new System.Windows.Forms.Label();
            this.lblStatsACESversion = new System.Windows.Forms.Label();
            this.lblStatsTitle = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.groupBoxRemoteVCdb = new System.Windows.Forms.GroupBox();
            this.radioButtonDataSourceMySQL = new System.Windows.Forms.RadioButton();
            this.radioButtonDataSourceAccess = new System.Windows.Forms.RadioButton();
            this.buttonMySQLConnect = new System.Windows.Forms.Button();
            this.textBoxMySQLpassword = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxMySQLuser = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxMySQLhost = new System.Windows.Forms.TextBox();
            this.checkBoxAutoloadLocalDatabases = new System.Windows.Forms.CheckBox();
            this.lblAssessmentsPath = new System.Windows.Forms.Label();
            this.btnSelectAssessmentDir = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownThreads = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxUKgrace = new System.Windows.Forms.CheckBox();
            this.checkBoxLimitDataGridRows = new System.Windows.Forms.CheckBox();
            this.groupBoxValidateTagOptions = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.checkBoxRespectValidateTag = new System.Windows.Forms.CheckBox();
            this.groupBoxQuantityOutlierSettings = new System.Windows.Forms.GroupBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownQtyOutliersThreshold = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownQtyOutliersSample = new System.Windows.Forms.NumericUpDown();
            this.checkBoxQtyOutliers = new System.Windows.Forms.CheckBox();
            this.lblCachePath = new System.Windows.Forms.Label();
            this.btnSelectCacheDir = new System.Windows.Forms.Button();
            this.checkBoxAssetsAsFitment = new System.Windows.Forms.CheckBox();
            this.checkBoxExplodeNotes = new System.Windows.Forms.CheckBox();
            this.groupBoxFitmentLogicSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxReportAllAppsInProblemGroup = new System.Windows.Forms.CheckBox();
            this.checkBoxConcernForDisparate = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.numericUpDownTreeConfigLimit = new System.Windows.Forms.NumericUpDown();
            this.tabPageExports = new System.Windows.Forms.TabPage();
            this.progBarExportRelatedParts = new System.Windows.Forms.ProgressBar();
            this.progBarExportFlatApps = new System.Windows.Forms.ProgressBar();
            this.progBarExportBuyersGuide = new System.Windows.Forms.ProgressBar();
            this.checkBoxAnonymizeErrorsACES = new System.Windows.Forms.CheckBox();
            this.btnExportConfigerrorsACES = new System.Windows.Forms.Button();
            this.checkBoxEncipherExport = new System.Windows.Forms.CheckBox();
            this.btnExportPrimaryACES = new System.Windows.Forms.Button();
            this.btnNetChangeExportSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnHolesExportSave = new System.Windows.Forms.Button();
            this.btnBgExportSave = new System.Windows.Forms.Button();
            this.comboBoxExportDelimiter = new System.Windows.Forms.ComboBox();
            this.btnAppExportSave = new System.Windows.Forms.Button();
            this.checkBoxRelatedPartsUseNotes = new System.Windows.Forms.CheckBox();
            this.checkBoxRelatedPartsUseAttributes = new System.Windows.Forms.CheckBox();
            this.checkBoxRelatedPartsUsePosition = new System.Windows.Forms.CheckBox();
            this.label22 = new System.Windows.Forms.Label();
            this.comboBoxRelatedTypesRight = new System.Windows.Forms.ComboBox();
            this.comboBoxRelatedTypesLeft = new System.Windows.Forms.ComboBox();
            this.btnExportRelatedParts = new System.Windows.Forms.Button();
            this.tabPageParts = new System.Windows.Forms.TabPage();
            this.lblPartsTabRedirect = new System.Windows.Forms.Label();
            this.tabPagePartsMultiTypes = new System.Windows.Forms.TabPage();
            this.dgParttypeDisagreement = new System.Windows.Forms.DataGridView();
            this.dgParttypeDisagreementPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgParttypeDisagreementParttypes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageParttypePosition = new System.Windows.Forms.TabPage();
            this.lblParttypePositionRedirect = new System.Windows.Forms.Label();
            this.dgParttypePosition = new System.Windows.Forms.DataGridView();
            this.dataGridViewParttypePositionError = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionAppId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionBasevid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionMake = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionParttype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewParttypePositionQualifiers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageQdbErrors = new System.Windows.Forms.TabPage();
            this.lblQdbErrorsRedirect = new System.Windows.Forms.Label();
            this.dgQdbErrors = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumnError = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxBasevehicleid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageInvalidBasevids = new System.Windows.Forms.TabPage();
            this.lblInvalidBasevehiclesRedirect = new System.Windows.Forms.Label();
            this.dgBasevids = new System.Windows.Forms.DataGridView();
            this.dgBasevidsApplicationid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgBasevidsBasevid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgBasevidsParttype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgBasevidsPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgBasevidsQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgBasevidsPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgBasevidsQualifiers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageInvalidVCdbCodes = new System.Windows.Forms.TabPage();
            this.lblInvalidVCdbCodesRedirect = new System.Windows.Forms.Label();
            this.dgVCdbCodes = new System.Windows.Forms.DataGridView();
            this.dgVCdbCodesApplicationid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesMake = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesParttype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesQualifiers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbCodesNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageInvalidConfigs = new System.Windows.Forms.TabPage();
            this.lblVCdbConfigErrorRedirect = new System.Windows.Forms.Label();
            this.dgVCdbConfigs = new System.Windows.Forms.DataGridView();
            this.dgVCdbConfigsApplicationid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsBasevehicleid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsMake = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsParttype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsVCdbAttributes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsQdbQualifiers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgVCdbConfigsNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageAddsDropsParts = new System.Windows.Forms.TabPage();
            this.dgAddsDropsParts = new System.Windows.Forms.DataGridView();
            this.dgAddsDropsPartsAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageAddsDropsVehicles = new System.Windows.Forms.TabPage();
            this.dgAddsDropsVehicles = new System.Windows.Forms.DataGridView();
            this.dgAddsDropsVehiclesAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgAddsDropsVehiclesBaseVid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgAddsDropsVehiclesMake = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgAddsDropsVehiclesModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgAddsDropsVehiclesYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgAddsDropsVehiclesParttype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgAddsDropsVehiclesPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgAddsDropsVehiclesQualifiers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgAddsDropsVehiclesMfrLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageQuantityWarnings = new System.Windows.Forms.TabPage();
            this.lblQtyWarningsRedirect = new System.Windows.Forms.Label();
            this.dgQuantityWarnings = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn201 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageFitmentLogic = new System.Windows.Forms.TabPage();
            this.splitContainerFitmentLogic = new System.Windows.Forms.SplitContainer();
            this.lblFitmentLogicProblemsTabRedirect = new System.Windows.Forms.Label();
            this.dgFitmentLogicProblems = new System.Windows.Forms.DataGridView();
            this.listBoxFitmentLogicElements = new System.Windows.Forms.ListBox();
            this.pictureBoxFitmentTree = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblAppVersion = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnSelectReferenceACESfile = new System.Windows.Forms.Button();
            this.lblReferenceACESfilePath = new System.Windows.Forms.Label();
            this.btnSelectPartInterchange = new System.Windows.Forms.Button();
            this.lblinterchangefilePath = new System.Windows.Forms.Label();
            this.btnSelectQdbFile = new System.Windows.Forms.Button();
            this.lblQdbFilePath = new System.Windows.Forms.Label();
            this.btnSelectNoteTranslationFile = new System.Windows.Forms.Button();
            this.lblNoteTranslationfilePath = new System.Windows.Forms.Label();
            this.timerHistoryUpdate = new System.Windows.Forms.Timer(this.components);
            this.comboBoxMySQLvcdbVersion = new System.Windows.Forms.ComboBox();
            this.buttonMySQLloadVCdb = new System.Windows.Forms.Button();
            this.progBarVCdbload = new System.Windows.Forms.ProgressBar();
            this.comboBoxMySQLpcdbVersion = new System.Windows.Forms.ComboBox();
            this.buttonMySQLloadPCdb = new System.Windows.Forms.Button();
            this.comboBoxMySQLqdbVersion = new System.Windows.Forms.ComboBox();
            this.buttonMySQLloadQdb = new System.Windows.Forms.Button();
            this.lblVCdbLoadStatus = new System.Windows.Forms.Label();
            this.progBarPrimeACESload = new System.Windows.Forms.ProgressBar();
            this.progBarRefACESload = new System.Windows.Forms.ProgressBar();
            this.lblPrimeACESLoadStatus = new System.Windows.Forms.Label();
            this.lblRefACESLoadStatus = new System.Windows.Forms.Label();
            this.dgLogicProblemsDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsAppId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsReference = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsBaseVehicleId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsMake = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsPartType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsPart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgLogicProblemsFitment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgParts)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCommonErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogicProblems)).BeginInit();
            this.tabPageSettings.SuspendLayout();
            this.groupBoxRemoteVCdb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreads)).BeginInit();
            this.groupBoxValidateTagOptions.SuspendLayout();
            this.groupBoxQuantityOutlierSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQtyOutliersThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQtyOutliersSample)).BeginInit();
            this.groupBoxFitmentLogicSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTreeConfigLimit)).BeginInit();
            this.tabPageExports.SuspendLayout();
            this.tabPageParts.SuspendLayout();
            this.tabPagePartsMultiTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgParttypeDisagreement)).BeginInit();
            this.tabPageParttypePosition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgParttypePosition)).BeginInit();
            this.tabPageQdbErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgQdbErrors)).BeginInit();
            this.tabPageInvalidBasevids.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBasevids)).BeginInit();
            this.tabPageInvalidVCdbCodes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgVCdbCodes)).BeginInit();
            this.tabPageInvalidConfigs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgVCdbConfigs)).BeginInit();
            this.tabPageAddsDropsParts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAddsDropsParts)).BeginInit();
            this.tabPageAddsDropsVehicles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgAddsDropsVehicles)).BeginInit();
            this.tabPageQuantityWarnings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgQuantityWarnings)).BeginInit();
            this.tabPageFitmentLogic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFitmentLogic)).BeginInit();
            this.splitContainerFitmentLogic.Panel1.SuspendLayout();
            this.splitContainerFitmentLogic.Panel2.SuspendLayout();
            this.splitContainerFitmentLogic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFitmentLogicProblems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFitmentTree)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectACESfile
            // 
            this.btnSelectACESfile.Location = new System.Drawing.Point(12, 12);
            this.btnSelectACESfile.Name = "btnSelectACESfile";
            this.btnSelectACESfile.Size = new System.Drawing.Size(139, 22);
            this.btnSelectACESfile.TabIndex = 2;
            this.btnSelectACESfile.Text = "Primary ACES file";
            this.btnSelectACESfile.UseVisualStyleBackColor = true;
            this.btnSelectACESfile.Click += new System.EventHandler(this.btnSelectACESfile_Click);
            // 
            // lblACESfilePath
            // 
            this.lblACESfilePath.AutoSize = true;
            this.lblACESfilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblACESfilePath.Location = new System.Drawing.Point(352, 15);
            this.lblACESfilePath.Name = "lblACESfilePath";
            this.lblACESfilePath.Size = new System.Drawing.Size(45, 16);
            this.lblACESfilePath.TabIndex = 3;
            this.lblACESfilePath.Text = "label1";
            // 
            // btnSelectVCdbFile
            // 
            this.btnSelectVCdbFile.Location = new System.Drawing.Point(12, 125);
            this.btnSelectVCdbFile.Name = "btnSelectVCdbFile";
            this.btnSelectVCdbFile.Size = new System.Drawing.Size(139, 23);
            this.btnSelectVCdbFile.TabIndex = 4;
            this.btnSelectVCdbFile.Text = "Select VCdb file";
            this.btnSelectVCdbFile.UseVisualStyleBackColor = true;
            this.btnSelectVCdbFile.Click += new System.EventHandler(this.btnSelectVCdbFile_Click);
            // 
            // lblVCdbFilePath
            // 
            this.lblVCdbFilePath.AutoSize = true;
            this.lblVCdbFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVCdbFilePath.Location = new System.Drawing.Point(152, 129);
            this.lblVCdbFilePath.Name = "lblVCdbFilePath";
            this.lblVCdbFilePath.Size = new System.Drawing.Size(45, 16);
            this.lblVCdbFilePath.TabIndex = 5;
            this.lblVCdbFilePath.Text = "label1";
            // 
            // btnSelectPCdbFile
            // 
            this.btnSelectPCdbFile.Location = new System.Drawing.Point(12, 153);
            this.btnSelectPCdbFile.Name = "btnSelectPCdbFile";
            this.btnSelectPCdbFile.Size = new System.Drawing.Size(139, 21);
            this.btnSelectPCdbFile.TabIndex = 6;
            this.btnSelectPCdbFile.Text = "Select PCdb file";
            this.btnSelectPCdbFile.UseVisualStyleBackColor = true;
            this.btnSelectPCdbFile.Click += new System.EventHandler(this.btnSelectPCdbFile_Click);
            // 
            // lblPCdbFilePath
            // 
            this.lblPCdbFilePath.AutoSize = true;
            this.lblPCdbFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPCdbFilePath.Location = new System.Drawing.Point(152, 157);
            this.lblPCdbFilePath.Name = "lblPCdbFilePath";
            this.lblPCdbFilePath.Size = new System.Drawing.Size(45, 16);
            this.lblPCdbFilePath.TabIndex = 7;
            this.lblPCdbFilePath.Text = "label1";
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Location = new System.Drawing.Point(14, 212);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(137, 29);
            this.btnAnalyze.TabIndex = 11;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // dgParts
            // 
            this.dgParts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgParts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgPartsPart,
            this.dgPartsAppCount,
            this.dgPartsParttypes,
            this.dgPartsPositions});
            this.dgParts.Location = new System.Drawing.Point(3, 4);
            this.dgParts.Name = "dgParts";
            this.dgParts.Size = new System.Drawing.Size(1228, 479);
            this.dgParts.TabIndex = 0;
            // 
            // dgPartsPart
            // 
            this.dgPartsPart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgPartsPart.HeaderText = "Part";
            this.dgPartsPart.Name = "dgPartsPart";
            this.dgPartsPart.ReadOnly = true;
            this.dgPartsPart.Width = 51;
            // 
            // dgPartsAppCount
            // 
            this.dgPartsAppCount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgPartsAppCount.HeaderText = "Application Count";
            this.dgPartsAppCount.Name = "dgPartsAppCount";
            this.dgPartsAppCount.ReadOnly = true;
            this.dgPartsAppCount.Width = 105;
            // 
            // dgPartsParttypes
            // 
            this.dgPartsParttypes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgPartsParttypes.HeaderText = "Part Types";
            this.dgPartsParttypes.Name = "dgPartsParttypes";
            this.dgPartsParttypes.ReadOnly = true;
            this.dgPartsParttypes.Width = 77;
            // 
            // dgPartsPositions
            // 
            this.dgPartsPositions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgPartsPositions.HeaderText = "Positions";
            this.dgPartsPositions.Name = "dgPartsPositions";
            this.dgPartsPositions.ReadOnly = true;
            this.dgPartsPositions.Width = 74;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageStats);
            this.tabControl1.Controls.Add(this.tabPageSettings);
            this.tabControl1.Controls.Add(this.tabPageExports);
            this.tabControl1.Controls.Add(this.tabPageParts);
            this.tabControl1.Controls.Add(this.tabPagePartsMultiTypes);
            this.tabControl1.Controls.Add(this.tabPageParttypePosition);
            this.tabControl1.Controls.Add(this.tabPageQdbErrors);
            this.tabControl1.Controls.Add(this.tabPageInvalidBasevids);
            this.tabControl1.Controls.Add(this.tabPageInvalidVCdbCodes);
            this.tabControl1.Controls.Add(this.tabPageInvalidConfigs);
            this.tabControl1.Controls.Add(this.tabPageAddsDropsParts);
            this.tabControl1.Controls.Add(this.tabPageAddsDropsVehicles);
            this.tabControl1.Controls.Add(this.tabPageQuantityWarnings);
            this.tabControl1.Controls.Add(this.tabPageFitmentLogic);
            this.tabControl1.Location = new System.Drawing.Point(2, 247);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1242, 462);
            this.tabControl1.TabIndex = 12;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPageStats
            // 
            this.tabPageStats.Controls.Add(this.textBoxAnalysisHostory);
            this.tabPageStats.Controls.Add(this.lblStatsProcessingTime);
            this.tabPageStats.Controls.Add(this.lblProcessTimeTitle);
            this.tabPageStats.Controls.Add(this.pictureBoxCommonErrors);
            this.tabPageStats.Controls.Add(this.lblIndividualErrors);
            this.tabPageStats.Controls.Add(this.lblIndividualErrorsTitle);
            this.tabPageStats.Controls.Add(this.lblDifferentialsSummary);
            this.tabPageStats.Controls.Add(this.lblDifferentialsLabel);
            this.tabPageStats.Controls.Add(this.progressBarDifferentials);
            this.tabPageStats.Controls.Add(this.pictureBoxLogicProblems);
            this.tabPageStats.Controls.Add(this.lblMacroProblems);
            this.tabPageStats.Controls.Add(this.lblMacroProblemsTitle);
            this.tabPageStats.Controls.Add(this.lblStatsPartsCount);
            this.tabPageStats.Controls.Add(this.lblStatsAppsCount);
            this.tabPageStats.Controls.Add(this.lblStatsQdbVersion);
            this.tabPageStats.Controls.Add(this.lblStatsPCdbVersion);
            this.tabPageStats.Controls.Add(this.lblStatsVCdbVersion);
            this.tabPageStats.Controls.Add(this.lblStatsACESversion);
            this.tabPageStats.Controls.Add(this.lblStatsTitle);
            this.tabPageStats.Controls.Add(this.label8);
            this.tabPageStats.Controls.Add(this.label7);
            this.tabPageStats.Controls.Add(this.label6);
            this.tabPageStats.Controls.Add(this.label5);
            this.tabPageStats.Controls.Add(this.label4);
            this.tabPageStats.Controls.Add(this.label3);
            this.tabPageStats.Controls.Add(this.label2);
            this.tabPageStats.Location = new System.Drawing.Point(4, 22);
            this.tabPageStats.Name = "tabPageStats";
            this.tabPageStats.Size = new System.Drawing.Size(1234, 436);
            this.tabPageStats.TabIndex = 9;
            this.tabPageStats.Text = "Statistics";
            this.tabPageStats.UseVisualStyleBackColor = true;
            // 
            // textBoxAnalysisHostory
            // 
            this.textBoxAnalysisHostory.BackColor = System.Drawing.SystemColors.Menu;
            this.textBoxAnalysisHostory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAnalysisHostory.Location = new System.Drawing.Point(583, 4);
            this.textBoxAnalysisHostory.Multiline = true;
            this.textBoxAnalysisHostory.Name = "textBoxAnalysisHostory";
            this.textBoxAnalysisHostory.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxAnalysisHostory.Size = new System.Drawing.Size(648, 429);
            this.textBoxAnalysisHostory.TabIndex = 64;
            this.textBoxAnalysisHostory.WordWrap = false;
            // 
            // lblStatsProcessingTime
            // 
            this.lblStatsProcessingTime.AutoSize = true;
            this.lblStatsProcessingTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatsProcessingTime.Location = new System.Drawing.Point(144, 145);
            this.lblStatsProcessingTime.Name = "lblStatsProcessingTime";
            this.lblStatsProcessingTime.Size = new System.Drawing.Size(42, 20);
            this.lblStatsProcessingTime.TabIndex = 63;
            this.lblStatsProcessingTime.Text = "label";
            // 
            // lblProcessTimeTitle
            // 
            this.lblProcessTimeTitle.AutoSize = true;
            this.lblProcessTimeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessTimeTitle.Location = new System.Drawing.Point(6, 145);
            this.lblProcessTimeTitle.Name = "lblProcessTimeTitle";
            this.lblProcessTimeTitle.Size = new System.Drawing.Size(125, 20);
            this.lblProcessTimeTitle.TabIndex = 62;
            this.lblProcessTimeTitle.Text = "Processing Time";
            // 
            // pictureBoxCommonErrors
            // 
            this.pictureBoxCommonErrors.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBoxCommonErrors.Location = new System.Drawing.Point(10, 203);
            this.pictureBoxCommonErrors.Name = "pictureBoxCommonErrors";
            this.pictureBoxCommonErrors.Size = new System.Drawing.Size(550, 75);
            this.pictureBoxCommonErrors.TabIndex = 60;
            this.pictureBoxCommonErrors.TabStop = false;
            this.pictureBoxCommonErrors.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxCommonErrors_Paint);
            // 
            // lblIndividualErrors
            // 
            this.lblIndividualErrors.AutoSize = true;
            this.lblIndividualErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIndividualErrors.Location = new System.Drawing.Point(144, 180);
            this.lblIndividualErrors.Name = "lblIndividualErrors";
            this.lblIndividualErrors.Size = new System.Drawing.Size(42, 20);
            this.lblIndividualErrors.TabIndex = 59;
            this.lblIndividualErrors.Text = "label";
            // 
            // lblIndividualErrorsTitle
            // 
            this.lblIndividualErrorsTitle.AutoSize = true;
            this.lblIndividualErrorsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIndividualErrorsTitle.Location = new System.Drawing.Point(6, 180);
            this.lblIndividualErrorsTitle.Name = "lblIndividualErrorsTitle";
            this.lblIndividualErrorsTitle.Size = new System.Drawing.Size(114, 20);
            this.lblIndividualErrorsTitle.TabIndex = 58;
            this.lblIndividualErrorsTitle.Text = "Individual apps";
            // 
            // lblDifferentialsSummary
            // 
            this.lblDifferentialsSummary.AutoSize = true;
            this.lblDifferentialsSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDifferentialsSummary.Location = new System.Drawing.Point(183, 391);
            this.lblDifferentialsSummary.Name = "lblDifferentialsSummary";
            this.lblDifferentialsSummary.Size = new System.Drawing.Size(42, 20);
            this.lblDifferentialsSummary.TabIndex = 53;
            this.lblDifferentialsSummary.Text = "label";
            // 
            // lblDifferentialsLabel
            // 
            this.lblDifferentialsLabel.AutoSize = true;
            this.lblDifferentialsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDifferentialsLabel.Location = new System.Drawing.Point(8, 391);
            this.lblDifferentialsLabel.Name = "lblDifferentialsLabel";
            this.lblDifferentialsLabel.Size = new System.Drawing.Size(169, 20);
            this.lblDifferentialsLabel.TabIndex = 52;
            this.lblDifferentialsLabel.Text = "Delta (prime minus ref)";
            // 
            // progressBarDifferentials
            // 
            this.progressBarDifferentials.Location = new System.Drawing.Point(12, 414);
            this.progressBarDifferentials.Name = "progressBarDifferentials";
            this.progressBarDifferentials.Size = new System.Drawing.Size(535, 16);
            this.progressBarDifferentials.TabIndex = 51;
            // 
            // pictureBoxLogicProblems
            // 
            this.pictureBoxLogicProblems.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBoxLogicProblems.Location = new System.Drawing.Point(10, 304);
            this.pictureBoxLogicProblems.Name = "pictureBoxLogicProblems";
            this.pictureBoxLogicProblems.Size = new System.Drawing.Size(550, 75);
            this.pictureBoxLogicProblems.TabIndex = 39;
            this.pictureBoxLogicProblems.TabStop = false;
            this.pictureBoxLogicProblems.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxLogicProblems_Paint);
            // 
            // lblMacroProblems
            // 
            this.lblMacroProblems.AutoSize = true;
            this.lblMacroProblems.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMacroProblems.Location = new System.Drawing.Point(144, 281);
            this.lblMacroProblems.Name = "lblMacroProblems";
            this.lblMacroProblems.Size = new System.Drawing.Size(42, 20);
            this.lblMacroProblems.TabIndex = 23;
            this.lblMacroProblems.Text = "label";
            // 
            // lblMacroProblemsTitle
            // 
            this.lblMacroProblemsTitle.AutoSize = true;
            this.lblMacroProblemsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMacroProblemsTitle.Location = new System.Drawing.Point(7, 281);
            this.lblMacroProblemsTitle.Name = "lblMacroProblemsTitle";
            this.lblMacroProblemsTitle.Size = new System.Drawing.Size(124, 20);
            this.lblMacroProblemsTitle.TabIndex = 22;
            this.lblMacroProblemsTitle.Text = "Groups Analysis";
            // 
            // lblStatsPartsCount
            // 
            this.lblStatsPartsCount.AutoSize = true;
            this.lblStatsPartsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatsPartsCount.Location = new System.Drawing.Point(144, 125);
            this.lblStatsPartsCount.Name = "lblStatsPartsCount";
            this.lblStatsPartsCount.Size = new System.Drawing.Size(42, 20);
            this.lblStatsPartsCount.TabIndex = 15;
            this.lblStatsPartsCount.Text = "label";
            // 
            // lblStatsAppsCount
            // 
            this.lblStatsAppsCount.AutoSize = true;
            this.lblStatsAppsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatsAppsCount.Location = new System.Drawing.Point(144, 105);
            this.lblStatsAppsCount.Name = "lblStatsAppsCount";
            this.lblStatsAppsCount.Size = new System.Drawing.Size(42, 20);
            this.lblStatsAppsCount.TabIndex = 14;
            this.lblStatsAppsCount.Text = "label";
            // 
            // lblStatsQdbVersion
            // 
            this.lblStatsQdbVersion.AutoSize = true;
            this.lblStatsQdbVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatsQdbVersion.Location = new System.Drawing.Point(144, 85);
            this.lblStatsQdbVersion.Name = "lblStatsQdbVersion";
            this.lblStatsQdbVersion.Size = new System.Drawing.Size(42, 20);
            this.lblStatsQdbVersion.TabIndex = 13;
            this.lblStatsQdbVersion.Text = "label";
            // 
            // lblStatsPCdbVersion
            // 
            this.lblStatsPCdbVersion.AutoSize = true;
            this.lblStatsPCdbVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatsPCdbVersion.Location = new System.Drawing.Point(144, 65);
            this.lblStatsPCdbVersion.Name = "lblStatsPCdbVersion";
            this.lblStatsPCdbVersion.Size = new System.Drawing.Size(42, 20);
            this.lblStatsPCdbVersion.TabIndex = 12;
            this.lblStatsPCdbVersion.Text = "label";
            // 
            // lblStatsVCdbVersion
            // 
            this.lblStatsVCdbVersion.AutoSize = true;
            this.lblStatsVCdbVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatsVCdbVersion.Location = new System.Drawing.Point(144, 45);
            this.lblStatsVCdbVersion.Name = "lblStatsVCdbVersion";
            this.lblStatsVCdbVersion.Size = new System.Drawing.Size(42, 20);
            this.lblStatsVCdbVersion.TabIndex = 11;
            this.lblStatsVCdbVersion.Text = "label";
            // 
            // lblStatsACESversion
            // 
            this.lblStatsACESversion.AutoSize = true;
            this.lblStatsACESversion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatsACESversion.Location = new System.Drawing.Point(144, 24);
            this.lblStatsACESversion.Name = "lblStatsACESversion";
            this.lblStatsACESversion.Size = new System.Drawing.Size(42, 20);
            this.lblStatsACESversion.TabIndex = 10;
            this.lblStatsACESversion.Text = "label";
            // 
            // lblStatsTitle
            // 
            this.lblStatsTitle.AutoSize = true;
            this.lblStatsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatsTitle.Location = new System.Drawing.Point(144, 5);
            this.lblStatsTitle.Name = "lblStatsTitle";
            this.lblStatsTitle.Size = new System.Drawing.Size(42, 20);
            this.lblStatsTitle.TabIndex = 9;
            this.lblStatsTitle.Text = "label";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 125);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 20);
            this.label8.TabIndex = 6;
            this.label8.Text = "Part Count";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(134, 20);
            this.label7.TabIndex = 5;
            this.label7.Text = "Application Count";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 20);
            this.label6.TabIndex = 4;
            this.label6.Text = "Qdb Version";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "PCdb Version";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "VCdb Version";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "ACES Version";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Title";
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.groupBoxRemoteVCdb);
            this.tabPageSettings.Controls.Add(this.lblAssessmentsPath);
            this.tabPageSettings.Controls.Add(this.btnSelectAssessmentDir);
            this.tabPageSettings.Controls.Add(this.label10);
            this.tabPageSettings.Controls.Add(this.numericUpDownThreads);
            this.tabPageSettings.Controls.Add(this.label9);
            this.tabPageSettings.Controls.Add(this.checkBoxUKgrace);
            this.tabPageSettings.Controls.Add(this.checkBoxLimitDataGridRows);
            this.tabPageSettings.Controls.Add(this.groupBoxValidateTagOptions);
            this.tabPageSettings.Controls.Add(this.groupBoxQuantityOutlierSettings);
            this.tabPageSettings.Controls.Add(this.lblCachePath);
            this.tabPageSettings.Controls.Add(this.btnSelectCacheDir);
            this.tabPageSettings.Controls.Add(this.checkBoxAssetsAsFitment);
            this.tabPageSettings.Controls.Add(this.checkBoxExplodeNotes);
            this.tabPageSettings.Controls.Add(this.groupBoxFitmentLogicSettings);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Size = new System.Drawing.Size(1234, 436);
            this.tabPageSettings.TabIndex = 17;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxRemoteVCdb
            // 
            this.groupBoxRemoteVCdb.Controls.Add(this.radioButtonDataSourceMySQL);
            this.groupBoxRemoteVCdb.Controls.Add(this.radioButtonDataSourceAccess);
            this.groupBoxRemoteVCdb.Controls.Add(this.buttonMySQLConnect);
            this.groupBoxRemoteVCdb.Controls.Add(this.textBoxMySQLpassword);
            this.groupBoxRemoteVCdb.Controls.Add(this.label15);
            this.groupBoxRemoteVCdb.Controls.Add(this.textBoxMySQLuser);
            this.groupBoxRemoteVCdb.Controls.Add(this.label14);
            this.groupBoxRemoteVCdb.Controls.Add(this.textBoxMySQLhost);
            this.groupBoxRemoteVCdb.Controls.Add(this.checkBoxAutoloadLocalDatabases);
            this.groupBoxRemoteVCdb.Location = new System.Drawing.Point(9, 70);
            this.groupBoxRemoteVCdb.Name = "groupBoxRemoteVCdb";
            this.groupBoxRemoteVCdb.Size = new System.Drawing.Size(333, 99);
            this.groupBoxRemoteVCdb.TabIndex = 21;
            this.groupBoxRemoteVCdb.TabStop = false;
            this.groupBoxRemoteVCdb.Text = "Reference DataSources";
            // 
            // radioButtonDataSourceMySQL
            // 
            this.radioButtonDataSourceMySQL.AutoSize = true;
            this.radioButtonDataSourceMySQL.Location = new System.Drawing.Point(12, 42);
            this.radioButtonDataSourceMySQL.Name = "radioButtonDataSourceMySQL";
            this.radioButtonDataSourceMySQL.Size = new System.Drawing.Size(138, 17);
            this.radioButtonDataSourceMySQL.TabIndex = 10;
            this.radioButtonDataSourceMySQL.TabStop = true;
            this.radioButtonDataSourceMySQL.Text = "Remote server (MySQL)";
            this.radioButtonDataSourceMySQL.UseVisualStyleBackColor = true;
            this.radioButtonDataSourceMySQL.CheckedChanged += new System.EventHandler(this.radioButtonDataSourceMySQL_CheckedChanged);
            // 
            // radioButtonDataSourceAccess
            // 
            this.radioButtonDataSourceAccess.AutoSize = true;
            this.radioButtonDataSourceAccess.Location = new System.Drawing.Point(12, 19);
            this.radioButtonDataSourceAccess.Name = "radioButtonDataSourceAccess";
            this.radioButtonDataSourceAccess.Size = new System.Drawing.Size(111, 17);
            this.radioButtonDataSourceAccess.TabIndex = 9;
            this.radioButtonDataSourceAccess.TabStop = true;
            this.radioButtonDataSourceAccess.Text = "Local (Access file)";
            this.radioButtonDataSourceAccess.UseVisualStyleBackColor = true;
            this.radioButtonDataSourceAccess.CheckedChanged += new System.EventHandler(this.radioButtonDataSourceAccess_CheckedChanged);
            // 
            // buttonMySQLConnect
            // 
            this.buttonMySQLConnect.Location = new System.Drawing.Point(268, 67);
            this.buttonMySQLConnect.Name = "buttonMySQLConnect";
            this.buttonMySQLConnect.Size = new System.Drawing.Size(57, 20);
            this.buttonMySQLConnect.TabIndex = 7;
            this.buttonMySQLConnect.Text = "Connect";
            this.buttonMySQLConnect.UseVisualStyleBackColor = true;
            this.buttonMySQLConnect.Click += new System.EventHandler(this.buttonMySQLconnect_Click);
            // 
            // textBoxMySQLpassword
            // 
            this.textBoxMySQLpassword.Location = new System.Drawing.Point(153, 67);
            this.textBoxMySQLpassword.Name = "textBoxMySQLpassword";
            this.textBoxMySQLpassword.Size = new System.Drawing.Size(110, 20);
            this.textBoxMySQLpassword.TabIndex = 6;
            this.textBoxMySQLpassword.Leave += new System.EventHandler(this.textBoxMySQLpassword_Leave);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(118, 70);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(29, 13);
            this.label15.TabIndex = 5;
            this.label15.Text = "pass";
            // 
            // textBoxMySQLuser
            // 
            this.textBoxMySQLuser.Location = new System.Drawing.Point(37, 67);
            this.textBoxMySQLuser.Name = "textBoxMySQLuser";
            this.textBoxMySQLuser.Size = new System.Drawing.Size(75, 20);
            this.textBoxMySQLuser.TabIndex = 4;
            this.textBoxMySQLuser.Leave += new System.EventHandler(this.textBoxMySQLuser_Leave);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(4, 70);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(27, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "user";
            // 
            // textBoxMySQLhost
            // 
            this.textBoxMySQLhost.Location = new System.Drawing.Point(153, 41);
            this.textBoxMySQLhost.Name = "textBoxMySQLhost";
            this.textBoxMySQLhost.Size = new System.Drawing.Size(172, 20);
            this.textBoxMySQLhost.TabIndex = 1;
            this.textBoxMySQLhost.Leave += new System.EventHandler(this.textBoxMySQLhost_Leave);
            // 
            // checkBoxAutoloadLocalDatabases
            // 
            this.checkBoxAutoloadLocalDatabases.AutoSize = true;
            this.checkBoxAutoloadLocalDatabases.Location = new System.Drawing.Point(126, 19);
            this.checkBoxAutoloadLocalDatabases.Name = "checkBoxAutoloadLocalDatabases";
            this.checkBoxAutoloadLocalDatabases.Size = new System.Drawing.Size(199, 17);
            this.checkBoxAutoloadLocalDatabases.TabIndex = 14;
            this.checkBoxAutoloadLocalDatabases.Text = "Autoload last local VCdb, PCdb, Qdb";
            this.checkBoxAutoloadLocalDatabases.UseVisualStyleBackColor = true;
            this.checkBoxAutoloadLocalDatabases.CheckedChanged += new System.EventHandler(this.checkBoxAutoloadDatabases_CheckedChanged);
            // 
            // lblAssessmentsPath
            // 
            this.lblAssessmentsPath.AutoSize = true;
            this.lblAssessmentsPath.Location = new System.Drawing.Point(168, 46);
            this.lblAssessmentsPath.Name = "lblAssessmentsPath";
            this.lblAssessmentsPath.Size = new System.Drawing.Size(16, 13);
            this.lblAssessmentsPath.TabIndex = 20;
            this.lblAssessmentsPath.Text = "---";
            // 
            // btnSelectAssessmentDir
            // 
            this.btnSelectAssessmentDir.Location = new System.Drawing.Point(9, 41);
            this.btnSelectAssessmentDir.Name = "btnSelectAssessmentDir";
            this.btnSelectAssessmentDir.Size = new System.Drawing.Size(153, 23);
            this.btnSelectAssessmentDir.TabIndex = 19;
            this.btnSelectAssessmentDir.Text = "Select Assessments Folder";
            this.btnSelectAssessmentDir.UseVisualStyleBackColor = true;
            this.btnSelectAssessmentDir.Click += new System.EventHandler(this.btnSelectAssessmentDir_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(151, 290);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "threads";
            // 
            // numericUpDownThreads
            // 
            this.numericUpDownThreads.Location = new System.Drawing.Point(104, 288);
            this.numericUpDownThreads.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownThreads.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThreads.Name = "numericUpDownThreads";
            this.numericUpDownThreads.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownThreads.TabIndex = 17;
            this.numericUpDownThreads.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThreads.ValueChanged += new System.EventHandler(this.numericUpDownThreads_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 290);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Divide work across";
            // 
            // checkBoxUKgrace
            // 
            this.checkBoxUKgrace.AutoSize = true;
            this.checkBoxUKgrace.Location = new System.Drawing.Point(8, 265);
            this.checkBoxUKgrace.Name = "checkBoxUKgrace";
            this.checkBoxUKgrace.Size = new System.Drawing.Size(208, 17);
            this.checkBoxUKgrace.TabIndex = 15;
            this.checkBoxUKgrace.Text = "Treat VCdb \"U/K\" values as wildcards";
            this.checkBoxUKgrace.UseVisualStyleBackColor = true;
            this.checkBoxUKgrace.CheckedChanged += new System.EventHandler(this.checkBoxUKgrace_CheckedChanged);
            // 
            // checkBoxLimitDataGridRows
            // 
            this.checkBoxLimitDataGridRows.AutoSize = true;
            this.checkBoxLimitDataGridRows.Location = new System.Drawing.Point(8, 242);
            this.checkBoxLimitDataGridRows.Name = "checkBoxLimitDataGridRows";
            this.checkBoxLimitDataGridRows.Size = new System.Drawing.Size(188, 17);
            this.checkBoxLimitDataGridRows.TabIndex = 13;
            this.checkBoxLimitDataGridRows.Text = "Limit datagrid displays to xxxx rows";
            this.checkBoxLimitDataGridRows.UseVisualStyleBackColor = true;
            this.checkBoxLimitDataGridRows.CheckedChanged += new System.EventHandler(this.checkBoxLimitDataGridRows_CheckedChanged);
            // 
            // groupBoxValidateTagOptions
            // 
            this.groupBoxValidateTagOptions.Controls.Add(this.radioButton2);
            this.groupBoxValidateTagOptions.Controls.Add(this.radioButton1);
            this.groupBoxValidateTagOptions.Controls.Add(this.checkBoxRespectValidateTag);
            this.groupBoxValidateTagOptions.Enabled = false;
            this.groupBoxValidateTagOptions.Location = new System.Drawing.Point(348, 70);
            this.groupBoxValidateTagOptions.Name = "groupBoxValidateTagOptions";
            this.groupBoxValidateTagOptions.Size = new System.Drawing.Size(253, 99);
            this.groupBoxValidateTagOptions.TabIndex = 12;
            this.groupBoxValidateTagOptions.TabStop = false;
            this.groupBoxValidateTagOptions.Text = "Handling of validate=no";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(17, 65);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(177, 17);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Ignore VCdb configuration errors";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(17, 42);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(160, 17);
            this.radioButton1.TabIndex = 6;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Ignore all aspects of the App";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // checkBoxRespectValidateTag
            // 
            this.checkBoxRespectValidateTag.AutoSize = true;
            this.checkBoxRespectValidateTag.Location = new System.Drawing.Point(6, 19);
            this.checkBoxRespectValidateTag.Name = "checkBoxRespectValidateTag";
            this.checkBoxRespectValidateTag.Size = new System.Drawing.Size(159, 17);
            this.checkBoxRespectValidateTag.TabIndex = 5;
            this.checkBoxRespectValidateTag.Text = "Respect XML \"validate\" tag";
            this.checkBoxRespectValidateTag.UseVisualStyleBackColor = true;
            this.checkBoxRespectValidateTag.CheckedChanged += new System.EventHandler(this.checkBoxRespectValidateTag_CheckedChanged);
            // 
            // groupBoxQuantityOutlierSettings
            // 
            this.groupBoxQuantityOutlierSettings.Controls.Add(this.label23);
            this.groupBoxQuantityOutlierSettings.Controls.Add(this.label21);
            this.groupBoxQuantityOutlierSettings.Controls.Add(this.label13);
            this.groupBoxQuantityOutlierSettings.Controls.Add(this.label12);
            this.groupBoxQuantityOutlierSettings.Controls.Add(this.numericUpDownQtyOutliersThreshold);
            this.groupBoxQuantityOutlierSettings.Controls.Add(this.numericUpDownQtyOutliersSample);
            this.groupBoxQuantityOutlierSettings.Controls.Add(this.checkBoxQtyOutliers);
            this.groupBoxQuantityOutlierSettings.Location = new System.Drawing.Point(222, 175);
            this.groupBoxQuantityOutlierSettings.Name = "groupBoxQuantityOutlierSettings";
            this.groupBoxQuantityOutlierSettings.Size = new System.Drawing.Size(379, 114);
            this.groupBoxQuantityOutlierSettings.TabIndex = 11;
            this.groupBoxQuantityOutlierSettings.TabStop = false;
            this.groupBoxQuantityOutlierSettings.Text = "Unusual \"Qty\" analysis";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(338, 73);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(33, 13);
            this.label23.TabIndex = 15;
            this.label23.Text = "apps.";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(14, 73);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(248, 13);
            this.label21.TabIndex = 14;
            this.label21.Text = "part-type/position group and the group has at least ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(250, 46);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(121, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "% of the total within their";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 46);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(176, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Find apps that account for less than";
            // 
            // numericUpDownQtyOutliersThreshold
            // 
            this.numericUpDownQtyOutliersThreshold.DecimalPlaces = 1;
            this.numericUpDownQtyOutliersThreshold.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownQtyOutliersThreshold.Location = new System.Drawing.Point(196, 44);
            this.numericUpDownQtyOutliersThreshold.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numericUpDownQtyOutliersThreshold.Name = "numericUpDownQtyOutliersThreshold";
            this.numericUpDownQtyOutliersThreshold.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownQtyOutliersThreshold.TabIndex = 11;
            this.numericUpDownQtyOutliersThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownQtyOutliersThreshold.ValueChanged += new System.EventHandler(this.numericUpDownQtyOutliersThreshold_ValueChanged);
            // 
            // numericUpDownQtyOutliersSample
            // 
            this.numericUpDownQtyOutliersSample.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownQtyOutliersSample.Location = new System.Drawing.Point(268, 71);
            this.numericUpDownQtyOutliersSample.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownQtyOutliersSample.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownQtyOutliersSample.Name = "numericUpDownQtyOutliersSample";
            this.numericUpDownQtyOutliersSample.Size = new System.Drawing.Size(62, 20);
            this.numericUpDownQtyOutliersSample.TabIndex = 10;
            this.numericUpDownQtyOutliersSample.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownQtyOutliersSample.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownQtyOutliersSample.ValueChanged += new System.EventHandler(this.numericUpDownQtyOutliersSample_ValueChanged);
            // 
            // checkBoxQtyOutliers
            // 
            this.checkBoxQtyOutliers.AutoSize = true;
            this.checkBoxQtyOutliers.Location = new System.Drawing.Point(17, 19);
            this.checkBoxQtyOutliers.Name = "checkBoxQtyOutliers";
            this.checkBoxQtyOutliers.Size = new System.Drawing.Size(141, 17);
            this.checkBoxQtyOutliers.TabIndex = 8;
            this.checkBoxQtyOutliers.Text = "Check for unusual \"Qty\"";
            this.checkBoxQtyOutliers.UseVisualStyleBackColor = true;
            this.checkBoxQtyOutliers.CheckedChanged += new System.EventHandler(this.checkBoxQtyOutliers_CheckedChanged);
            // 
            // lblCachePath
            // 
            this.lblCachePath.AutoSize = true;
            this.lblCachePath.Location = new System.Drawing.Point(168, 17);
            this.lblCachePath.Name = "lblCachePath";
            this.lblCachePath.Size = new System.Drawing.Size(16, 13);
            this.lblCachePath.TabIndex = 10;
            this.lblCachePath.Text = "---";
            // 
            // btnSelectCacheDir
            // 
            this.btnSelectCacheDir.Location = new System.Drawing.Point(9, 12);
            this.btnSelectCacheDir.Name = "btnSelectCacheDir";
            this.btnSelectCacheDir.Size = new System.Drawing.Size(153, 23);
            this.btnSelectCacheDir.TabIndex = 9;
            this.btnSelectCacheDir.Text = "Select Cache Folder";
            this.btnSelectCacheDir.UseVisualStyleBackColor = true;
            this.btnSelectCacheDir.Click += new System.EventHandler(this.btnSelectCacheDir_Click);
            // 
            // checkBoxAssetsAsFitment
            // 
            this.checkBoxAssetsAsFitment.AutoSize = true;
            this.checkBoxAssetsAsFitment.Location = new System.Drawing.Point(8, 219);
            this.checkBoxAssetsAsFitment.Name = "checkBoxAssetsAsFitment";
            this.checkBoxAssetsAsFitment.Size = new System.Drawing.Size(181, 17);
            this.checkBoxAssetsAsFitment.TabIndex = 7;
            this.checkBoxAssetsAsFitment.Text = "Treat Assets as fitment  elements";
            this.checkBoxAssetsAsFitment.UseVisualStyleBackColor = true;
            // 
            // checkBoxExplodeNotes
            // 
            this.checkBoxExplodeNotes.AutoSize = true;
            this.checkBoxExplodeNotes.Location = new System.Drawing.Point(8, 196);
            this.checkBoxExplodeNotes.Name = "checkBoxExplodeNotes";
            this.checkBoxExplodeNotes.Size = new System.Drawing.Size(180, 17);
            this.checkBoxExplodeNotes.TabIndex = 6;
            this.checkBoxExplodeNotes.Text = "Explode Note tags by semi-colon";
            this.checkBoxExplodeNotes.UseVisualStyleBackColor = true;
            this.checkBoxExplodeNotes.CheckedChanged += new System.EventHandler(this.checkBoxExplodeNotes_CheckedChanged);
            // 
            // groupBoxFitmentLogicSettings
            // 
            this.groupBoxFitmentLogicSettings.Controls.Add(this.checkBoxReportAllAppsInProblemGroup);
            this.groupBoxFitmentLogicSettings.Controls.Add(this.checkBoxConcernForDisparate);
            this.groupBoxFitmentLogicSettings.Controls.Add(this.label24);
            this.groupBoxFitmentLogicSettings.Controls.Add(this.numericUpDownTreeConfigLimit);
            this.groupBoxFitmentLogicSettings.Location = new System.Drawing.Point(8, 314);
            this.groupBoxFitmentLogicSettings.Name = "groupBoxFitmentLogicSettings";
            this.groupBoxFitmentLogicSettings.Size = new System.Drawing.Size(247, 92);
            this.groupBoxFitmentLogicSettings.TabIndex = 4;
            this.groupBoxFitmentLogicSettings.TabStop = false;
            this.groupBoxFitmentLogicSettings.Text = "App grouping analysis";
            // 
            // checkBoxReportAllAppsInProblemGroup
            // 
            this.checkBoxReportAllAppsInProblemGroup.AutoSize = true;
            this.checkBoxReportAllAppsInProblemGroup.Location = new System.Drawing.Point(6, 41);
            this.checkBoxReportAllAppsInProblemGroup.Name = "checkBoxReportAllAppsInProblemGroup";
            this.checkBoxReportAllAppsInProblemGroup.Size = new System.Drawing.Size(178, 17);
            this.checkBoxReportAllAppsInProblemGroup.TabIndex = 9;
            this.checkBoxReportAllAppsInProblemGroup.Text = "Report all apps in problem group";
            this.checkBoxReportAllAppsInProblemGroup.UseVisualStyleBackColor = true;
            this.checkBoxReportAllAppsInProblemGroup.CheckedChanged += new System.EventHandler(this.checkBoxReportAllAppsInProblemGroup_CheckedChanged);
            // 
            // checkBoxConcernForDisparate
            // 
            this.checkBoxConcernForDisparate.AutoSize = true;
            this.checkBoxConcernForDisparate.Location = new System.Drawing.Point(6, 18);
            this.checkBoxConcernForDisparate.Name = "checkBoxConcernForDisparate";
            this.checkBoxConcernForDisparate.Size = new System.Drawing.Size(154, 17);
            this.checkBoxConcernForDisparate.TabIndex = 4;
            this.checkBoxConcernForDisparate.Text = "Detect disparate branching";
            this.checkBoxConcernForDisparate.UseVisualStyleBackColor = true;
            this.checkBoxConcernForDisparate.CheckedChanged += new System.EventHandler(this.checkBoxConcernForVCdbVCdb_CheckedChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 61);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(175, 13);
            this.label24.TabIndex = 2;
            this.label24.Text = "Limit fitment element permutation to ";
            // 
            // numericUpDownTreeConfigLimit
            // 
            this.numericUpDownTreeConfigLimit.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTreeConfigLimit.Location = new System.Drawing.Point(179, 59);
            this.numericUpDownTreeConfigLimit.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownTreeConfigLimit.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTreeConfigLimit.Name = "numericUpDownTreeConfigLimit";
            this.numericUpDownTreeConfigLimit.Size = new System.Drawing.Size(58, 20);
            this.numericUpDownTreeConfigLimit.TabIndex = 1;
            this.numericUpDownTreeConfigLimit.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTreeConfigLimit.ValueChanged += new System.EventHandler(this.numericUpDownTreeConfigLimit_ValueChanged);
            // 
            // tabPageExports
            // 
            this.tabPageExports.Controls.Add(this.progBarExportRelatedParts);
            this.tabPageExports.Controls.Add(this.progBarExportFlatApps);
            this.tabPageExports.Controls.Add(this.progBarExportBuyersGuide);
            this.tabPageExports.Controls.Add(this.checkBoxAnonymizeErrorsACES);
            this.tabPageExports.Controls.Add(this.btnExportConfigerrorsACES);
            this.tabPageExports.Controls.Add(this.checkBoxEncipherExport);
            this.tabPageExports.Controls.Add(this.btnExportPrimaryACES);
            this.tabPageExports.Controls.Add(this.btnNetChangeExportSave);
            this.tabPageExports.Controls.Add(this.label1);
            this.tabPageExports.Controls.Add(this.btnHolesExportSave);
            this.tabPageExports.Controls.Add(this.btnBgExportSave);
            this.tabPageExports.Controls.Add(this.comboBoxExportDelimiter);
            this.tabPageExports.Controls.Add(this.btnAppExportSave);
            this.tabPageExports.Controls.Add(this.checkBoxRelatedPartsUseNotes);
            this.tabPageExports.Controls.Add(this.checkBoxRelatedPartsUseAttributes);
            this.tabPageExports.Controls.Add(this.checkBoxRelatedPartsUsePosition);
            this.tabPageExports.Controls.Add(this.label22);
            this.tabPageExports.Controls.Add(this.comboBoxRelatedTypesRight);
            this.tabPageExports.Controls.Add(this.comboBoxRelatedTypesLeft);
            this.tabPageExports.Controls.Add(this.btnExportRelatedParts);
            this.tabPageExports.Location = new System.Drawing.Point(4, 22);
            this.tabPageExports.Name = "tabPageExports";
            this.tabPageExports.Size = new System.Drawing.Size(1234, 436);
            this.tabPageExports.TabIndex = 5;
            this.tabPageExports.Text = "Exports";
            this.tabPageExports.UseVisualStyleBackColor = true;
            // 
            // progBarExportRelatedParts
            // 
            this.progBarExportRelatedParts.Location = new System.Drawing.Point(879, 117);
            this.progBarExportRelatedParts.Name = "progBarExportRelatedParts";
            this.progBarExportRelatedParts.Size = new System.Drawing.Size(144, 18);
            this.progBarExportRelatedParts.TabIndex = 56;
            // 
            // progBarExportFlatApps
            // 
            this.progBarExportFlatApps.Location = new System.Drawing.Point(287, 10);
            this.progBarExportFlatApps.Name = "progBarExportFlatApps";
            this.progBarExportFlatApps.Size = new System.Drawing.Size(144, 18);
            this.progBarExportFlatApps.TabIndex = 55;
            // 
            // progBarExportBuyersGuide
            // 
            this.progBarExportBuyersGuide.Location = new System.Drawing.Point(167, 63);
            this.progBarExportBuyersGuide.Name = "progBarExportBuyersGuide";
            this.progBarExportBuyersGuide.Size = new System.Drawing.Size(144, 18);
            this.progBarExportBuyersGuide.TabIndex = 54;
            // 
            // checkBoxAnonymizeErrorsACES
            // 
            this.checkBoxAnonymizeErrorsACES.AutoSize = true;
            this.checkBoxAnonymizeErrorsACES.Location = new System.Drawing.Point(164, 171);
            this.checkBoxAnonymizeErrorsACES.Name = "checkBoxAnonymizeErrorsACES";
            this.checkBoxAnonymizeErrorsACES.Size = new System.Drawing.Size(110, 17);
            this.checkBoxAnonymizeErrorsACES.TabIndex = 21;
            this.checkBoxAnonymizeErrorsACES.Text = "Anonymize output";
            this.checkBoxAnonymizeErrorsACES.UseVisualStyleBackColor = true;
            // 
            // btnExportConfigerrorsACES
            // 
            this.btnExportConfigerrorsACES.Location = new System.Drawing.Point(8, 168);
            this.btnExportConfigerrorsACES.Name = "btnExportConfigerrorsACES";
            this.btnExportConfigerrorsACES.Size = new System.Drawing.Size(139, 21);
            this.btnExportConfigerrorsACES.TabIndex = 20;
            this.btnExportConfigerrorsACES.Text = "Export errors ACES";
            this.btnExportConfigerrorsACES.UseVisualStyleBackColor = true;
            this.btnExportConfigerrorsACES.Click += new System.EventHandler(this.btnExportConfigerrorsACES_Click);
            // 
            // checkBoxEncipherExport
            // 
            this.checkBoxEncipherExport.AutoSize = true;
            this.checkBoxEncipherExport.Location = new System.Drawing.Point(164, 144);
            this.checkBoxEncipherExport.Name = "checkBoxEncipherExport";
            this.checkBoxEncipherExport.Size = new System.Drawing.Size(132, 17);
            this.checkBoxEncipherExport.TabIndex = 19;
            this.checkBoxEncipherExport.Text = "Encipher part numbers";
            this.checkBoxEncipherExport.UseVisualStyleBackColor = true;
            // 
            // btnExportPrimaryACES
            // 
            this.btnExportPrimaryACES.Location = new System.Drawing.Point(8, 141);
            this.btnExportPrimaryACES.Name = "btnExportPrimaryACES";
            this.btnExportPrimaryACES.Size = new System.Drawing.Size(139, 21);
            this.btnExportPrimaryACES.TabIndex = 18;
            this.btnExportPrimaryACES.Text = "Export Primary ACES";
            this.btnExportPrimaryACES.UseVisualStyleBackColor = true;
            this.btnExportPrimaryACES.Click += new System.EventHandler(this.btnExportPrimaryACES_Click);
            // 
            // btnNetChangeExportSave
            // 
            this.btnNetChangeExportSave.Location = new System.Drawing.Point(7, 34);
            this.btnNetChangeExportSave.Name = "btnNetChangeExportSave";
            this.btnNetChangeExportSave.Size = new System.Drawing.Size(140, 21);
            this.btnNetChangeExportSave.TabIndex = 2;
            this.btnNetChangeExportSave.Text = "Export net change ACES";
            this.btnNetChangeExportSave.UseVisualStyleBackColor = true;
            this.btnNetChangeExportSave.Click += new System.EventHandler(this.btnNetChangeExportSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(161, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Delimiter";
            // 
            // btnHolesExportSave
            // 
            this.btnHolesExportSave.Location = new System.Drawing.Point(7, 87);
            this.btnHolesExportSave.Name = "btnHolesExportSave";
            this.btnHolesExportSave.Size = new System.Drawing.Size(140, 21);
            this.btnHolesExportSave.TabIndex = 0;
            this.btnHolesExportSave.Text = "Export vehicle holes";
            this.btnHolesExportSave.UseVisualStyleBackColor = true;
            this.btnHolesExportSave.Click += new System.EventHandler(this.btnHolesExportSave_Click);
            // 
            // btnBgExportSave
            // 
            this.btnBgExportSave.Location = new System.Drawing.Point(7, 60);
            this.btnBgExportSave.Name = "btnBgExportSave";
            this.btnBgExportSave.Size = new System.Drawing.Size(140, 21);
            this.btnBgExportSave.TabIndex = 1;
            this.btnBgExportSave.Text = "Export buyer\'s guide";
            this.btnBgExportSave.UseVisualStyleBackColor = true;
            this.btnBgExportSave.Click += new System.EventHandler(this.btnBGExportSave_Click);
            // 
            // comboBoxExportDelimiter
            // 
            this.comboBoxExportDelimiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxExportDelimiter.FormattingEnabled = true;
            this.comboBoxExportDelimiter.Items.AddRange(new object[] {
            "Tab",
            "Pipe (|)",
            "Tilde (~)"});
            this.comboBoxExportDelimiter.Location = new System.Drawing.Point(214, 8);
            this.comboBoxExportDelimiter.Name = "comboBoxExportDelimiter";
            this.comboBoxExportDelimiter.Size = new System.Drawing.Size(67, 21);
            this.comboBoxExportDelimiter.TabIndex = 16;
            // 
            // btnAppExportSave
            // 
            this.btnAppExportSave.Location = new System.Drawing.Point(7, 7);
            this.btnAppExportSave.Name = "btnAppExportSave";
            this.btnAppExportSave.Size = new System.Drawing.Size(140, 21);
            this.btnAppExportSave.TabIndex = 15;
            this.btnAppExportSave.Text = "Export simplified (flat) apps";
            this.btnAppExportSave.UseVisualStyleBackColor = true;
            this.btnAppExportSave.Click += new System.EventHandler(this.btnAppExportSave_Click);
            // 
            // checkBoxRelatedPartsUseNotes
            // 
            this.checkBoxRelatedPartsUseNotes.AutoSize = true;
            this.checkBoxRelatedPartsUseNotes.Location = new System.Drawing.Point(776, 119);
            this.checkBoxRelatedPartsUseNotes.Name = "checkBoxRelatedPartsUseNotes";
            this.checkBoxRelatedPartsUseNotes.Size = new System.Drawing.Size(97, 17);
            this.checkBoxRelatedPartsUseNotes.TabIndex = 14;
            this.checkBoxRelatedPartsUseNotes.Text = "Respect Notes";
            this.checkBoxRelatedPartsUseNotes.UseVisualStyleBackColor = true;
            // 
            // checkBoxRelatedPartsUseAttributes
            // 
            this.checkBoxRelatedPartsUseAttributes.AutoSize = true;
            this.checkBoxRelatedPartsUseAttributes.Location = new System.Drawing.Point(628, 119);
            this.checkBoxRelatedPartsUseAttributes.Name = "checkBoxRelatedPartsUseAttributes";
            this.checkBoxRelatedPartsUseAttributes.Size = new System.Drawing.Size(142, 17);
            this.checkBoxRelatedPartsUseAttributes.TabIndex = 13;
            this.checkBoxRelatedPartsUseAttributes.Text = "Respect VCdb Attributes";
            this.checkBoxRelatedPartsUseAttributes.UseVisualStyleBackColor = true;
            // 
            // checkBoxRelatedPartsUsePosition
            // 
            this.checkBoxRelatedPartsUsePosition.AutoSize = true;
            this.checkBoxRelatedPartsUsePosition.Location = new System.Drawing.Point(516, 119);
            this.checkBoxRelatedPartsUsePosition.Name = "checkBoxRelatedPartsUsePosition";
            this.checkBoxRelatedPartsUsePosition.Size = new System.Drawing.Size(106, 17);
            this.checkBoxRelatedPartsUsePosition.TabIndex = 12;
            this.checkBoxRelatedPartsUsePosition.Text = "Respect Position";
            this.checkBoxRelatedPartsUsePosition.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(317, 118);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(44, 13);
            this.label22.TabIndex = 11;
            this.label22.Text = "Related";
            // 
            // comboBoxRelatedTypesRight
            // 
            this.comboBoxRelatedTypesRight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRelatedTypesRight.FormattingEnabled = true;
            this.comboBoxRelatedTypesRight.Location = new System.Drawing.Point(367, 115);
            this.comboBoxRelatedTypesRight.Name = "comboBoxRelatedTypesRight";
            this.comboBoxRelatedTypesRight.Size = new System.Drawing.Size(143, 21);
            this.comboBoxRelatedTypesRight.TabIndex = 9;
            // 
            // comboBoxRelatedTypesLeft
            // 
            this.comboBoxRelatedTypesLeft.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRelatedTypesLeft.FormattingEnabled = true;
            this.comboBoxRelatedTypesLeft.Location = new System.Drawing.Point(167, 115);
            this.comboBoxRelatedTypesLeft.Name = "comboBoxRelatedTypesLeft";
            this.comboBoxRelatedTypesLeft.Size = new System.Drawing.Size(144, 21);
            this.comboBoxRelatedTypesLeft.TabIndex = 8;
            // 
            // btnExportRelatedParts
            // 
            this.btnExportRelatedParts.Location = new System.Drawing.Point(7, 114);
            this.btnExportRelatedParts.Name = "btnExportRelatedParts";
            this.btnExportRelatedParts.Size = new System.Drawing.Size(141, 21);
            this.btnExportRelatedParts.TabIndex = 7;
            this.btnExportRelatedParts.Text = "Export cross-type pairings";
            this.btnExportRelatedParts.UseVisualStyleBackColor = true;
            this.btnExportRelatedParts.Click += new System.EventHandler(this.btnExportRelatedParts_Click);
            // 
            // tabPageParts
            // 
            this.tabPageParts.Controls.Add(this.lblPartsTabRedirect);
            this.tabPageParts.Controls.Add(this.dgParts);
            this.tabPageParts.Location = new System.Drawing.Point(4, 22);
            this.tabPageParts.Name = "tabPageParts";
            this.tabPageParts.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageParts.Size = new System.Drawing.Size(1234, 436);
            this.tabPageParts.TabIndex = 1;
            this.tabPageParts.Text = "Parts";
            this.tabPageParts.UseVisualStyleBackColor = true;
            // 
            // lblPartsTabRedirect
            // 
            this.lblPartsTabRedirect.AutoSize = true;
            this.lblPartsTabRedirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPartsTabRedirect.Location = new System.Drawing.Point(43, 38);
            this.lblPartsTabRedirect.Name = "lblPartsTabRedirect";
            this.lblPartsTabRedirect.Size = new System.Drawing.Size(242, 20);
            this.lblPartsTabRedirect.TabIndex = 1;
            this.lblPartsTabRedirect.Text = "Parts list it too large to show here";
            // 
            // tabPagePartsMultiTypes
            // 
            this.tabPagePartsMultiTypes.Controls.Add(this.dgParttypeDisagreement);
            this.tabPagePartsMultiTypes.Location = new System.Drawing.Point(4, 22);
            this.tabPagePartsMultiTypes.Name = "tabPagePartsMultiTypes";
            this.tabPagePartsMultiTypes.Size = new System.Drawing.Size(1234, 436);
            this.tabPagePartsMultiTypes.TabIndex = 11;
            this.tabPagePartsMultiTypes.Text = "Parttype Disagreement";
            this.tabPagePartsMultiTypes.UseVisualStyleBackColor = true;
            // 
            // dgParttypeDisagreement
            // 
            this.dgParttypeDisagreement.AllowUserToAddRows = false;
            this.dgParttypeDisagreement.AllowUserToDeleteRows = false;
            this.dgParttypeDisagreement.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgParttypeDisagreement.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgParttypeDisagreementPart,
            this.dgParttypeDisagreementParttypes});
            this.dgParttypeDisagreement.Location = new System.Drawing.Point(3, 3);
            this.dgParttypeDisagreement.Name = "dgParttypeDisagreement";
            this.dgParttypeDisagreement.Size = new System.Drawing.Size(1228, 480);
            this.dgParttypeDisagreement.TabIndex = 1;
            // 
            // dgParttypeDisagreementPart
            // 
            this.dgParttypeDisagreementPart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgParttypeDisagreementPart.HeaderText = "Part";
            this.dgParttypeDisagreementPart.Name = "dgParttypeDisagreementPart";
            this.dgParttypeDisagreementPart.ReadOnly = true;
            this.dgParttypeDisagreementPart.Width = 51;
            // 
            // dgParttypeDisagreementParttypes
            // 
            this.dgParttypeDisagreementParttypes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgParttypeDisagreementParttypes.HeaderText = "Part Types";
            this.dgParttypeDisagreementParttypes.Name = "dgParttypeDisagreementParttypes";
            this.dgParttypeDisagreementParttypes.ReadOnly = true;
            this.dgParttypeDisagreementParttypes.Width = 83;
            // 
            // tabPageParttypePosition
            // 
            this.tabPageParttypePosition.Controls.Add(this.lblParttypePositionRedirect);
            this.tabPageParttypePosition.Controls.Add(this.dgParttypePosition);
            this.tabPageParttypePosition.Location = new System.Drawing.Point(4, 22);
            this.tabPageParttypePosition.Name = "tabPageParttypePosition";
            this.tabPageParttypePosition.Size = new System.Drawing.Size(1234, 436);
            this.tabPageParttypePosition.TabIndex = 10;
            this.tabPageParttypePosition.Text = "Parttypes/Positions";
            this.tabPageParttypePosition.UseVisualStyleBackColor = true;
            // 
            // lblParttypePositionRedirect
            // 
            this.lblParttypePositionRedirect.AutoSize = true;
            this.lblParttypePositionRedirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParttypePositionRedirect.Location = new System.Drawing.Point(41, 35);
            this.lblParttypePositionRedirect.Name = "lblParttypePositionRedirect";
            this.lblParttypePositionRedirect.Size = new System.Drawing.Size(372, 20);
            this.lblParttypePositionRedirect.TabIndex = 5;
            this.lblParttypePositionRedirect.Text = "Parttype/Position errors list is too large to show here";
            // 
            // dgParttypePosition
            // 
            this.dgParttypePosition.AllowUserToAddRows = false;
            this.dgParttypePosition.AllowUserToDeleteRows = false;
            this.dgParttypePosition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgParttypePosition.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewParttypePositionError,
            this.dataGridViewParttypePositionAppId,
            this.dataGridViewParttypePositionBasevid,
            this.dataGridViewParttypePositionMake,
            this.dataGridViewParttypePositionModel,
            this.dataGridViewParttypePositionYear,
            this.dataGridViewParttypePositionParttype,
            this.dataGridViewParttypePositionPosition,
            this.dataGridViewParttypePositionQty,
            this.dataGridViewParttypePositionPart,
            this.dataGridViewParttypePositionQualifiers});
            this.dgParttypePosition.Location = new System.Drawing.Point(3, 3);
            this.dgParttypePosition.Name = "dgParttypePosition";
            this.dgParttypePosition.Size = new System.Drawing.Size(1228, 480);
            this.dgParttypePosition.TabIndex = 4;
            this.dgParttypePosition.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgParttypePosition_SortCompare);
            // 
            // dataGridViewParttypePositionError
            // 
            this.dataGridViewParttypePositionError.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionError.HeaderText = "Error";
            this.dataGridViewParttypePositionError.Name = "dataGridViewParttypePositionError";
            this.dataGridViewParttypePositionError.ReadOnly = true;
            this.dataGridViewParttypePositionError.Width = 54;
            // 
            // dataGridViewParttypePositionAppId
            // 
            this.dataGridViewParttypePositionAppId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionAppId.HeaderText = "App id";
            this.dataGridViewParttypePositionAppId.Name = "dataGridViewParttypePositionAppId";
            this.dataGridViewParttypePositionAppId.ReadOnly = true;
            this.dataGridViewParttypePositionAppId.Width = 51;
            // 
            // dataGridViewParttypePositionBasevid
            // 
            this.dataGridViewParttypePositionBasevid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionBasevid.HeaderText = "BaseVehicle Id";
            this.dataGridViewParttypePositionBasevid.Name = "dataGridViewParttypePositionBasevid";
            this.dataGridViewParttypePositionBasevid.ReadOnly = true;
            this.dataGridViewParttypePositionBasevid.Width = 95;
            // 
            // dataGridViewParttypePositionMake
            // 
            this.dataGridViewParttypePositionMake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionMake.HeaderText = "Make";
            this.dataGridViewParttypePositionMake.Name = "dataGridViewParttypePositionMake";
            this.dataGridViewParttypePositionMake.ReadOnly = true;
            this.dataGridViewParttypePositionMake.Width = 59;
            // 
            // dataGridViewParttypePositionModel
            // 
            this.dataGridViewParttypePositionModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionModel.HeaderText = "Model";
            this.dataGridViewParttypePositionModel.Name = "dataGridViewParttypePositionModel";
            this.dataGridViewParttypePositionModel.ReadOnly = true;
            this.dataGridViewParttypePositionModel.Width = 61;
            // 
            // dataGridViewParttypePositionYear
            // 
            this.dataGridViewParttypePositionYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionYear.HeaderText = "Year";
            this.dataGridViewParttypePositionYear.Name = "dataGridViewParttypePositionYear";
            this.dataGridViewParttypePositionYear.ReadOnly = true;
            this.dataGridViewParttypePositionYear.Width = 54;
            // 
            // dataGridViewParttypePositionParttype
            // 
            this.dataGridViewParttypePositionParttype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionParttype.HeaderText = "Part Type";
            this.dataGridViewParttypePositionParttype.Name = "dataGridViewParttypePositionParttype";
            this.dataGridViewParttypePositionParttype.ReadOnly = true;
            this.dataGridViewParttypePositionParttype.Width = 72;
            // 
            // dataGridViewParttypePositionPosition
            // 
            this.dataGridViewParttypePositionPosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionPosition.HeaderText = "Position";
            this.dataGridViewParttypePositionPosition.Name = "dataGridViewParttypePositionPosition";
            this.dataGridViewParttypePositionPosition.ReadOnly = true;
            this.dataGridViewParttypePositionPosition.Width = 69;
            // 
            // dataGridViewParttypePositionQty
            // 
            this.dataGridViewParttypePositionQty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionQty.HeaderText = "Qty";
            this.dataGridViewParttypePositionQty.Name = "dataGridViewParttypePositionQty";
            this.dataGridViewParttypePositionQty.ReadOnly = true;
            this.dataGridViewParttypePositionQty.Width = 48;
            // 
            // dataGridViewParttypePositionPart
            // 
            this.dataGridViewParttypePositionPart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionPart.HeaderText = "Part";
            this.dataGridViewParttypePositionPart.Name = "dataGridViewParttypePositionPart";
            this.dataGridViewParttypePositionPart.ReadOnly = true;
            this.dataGridViewParttypePositionPart.Width = 51;
            // 
            // dataGridViewParttypePositionQualifiers
            // 
            this.dataGridViewParttypePositionQualifiers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewParttypePositionQualifiers.HeaderText = "Fitment";
            this.dataGridViewParttypePositionQualifiers.Name = "dataGridViewParttypePositionQualifiers";
            this.dataGridViewParttypePositionQualifiers.ReadOnly = true;
            this.dataGridViewParttypePositionQualifiers.Width = 66;
            // 
            // tabPageQdbErrors
            // 
            this.tabPageQdbErrors.Controls.Add(this.lblQdbErrorsRedirect);
            this.tabPageQdbErrors.Controls.Add(this.dgQdbErrors);
            this.tabPageQdbErrors.Location = new System.Drawing.Point(4, 22);
            this.tabPageQdbErrors.Name = "tabPageQdbErrors";
            this.tabPageQdbErrors.Size = new System.Drawing.Size(1234, 436);
            this.tabPageQdbErrors.TabIndex = 15;
            this.tabPageQdbErrors.Text = "Qdb Errors";
            this.tabPageQdbErrors.UseVisualStyleBackColor = true;
            // 
            // lblQdbErrorsRedirect
            // 
            this.lblQdbErrorsRedirect.AutoSize = true;
            this.lblQdbErrorsRedirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQdbErrorsRedirect.Location = new System.Drawing.Point(41, 37);
            this.lblQdbErrorsRedirect.Name = "lblQdbErrorsRedirect";
            this.lblQdbErrorsRedirect.Size = new System.Drawing.Size(218, 20);
            this.lblQdbErrorsRedirect.TabIndex = 4;
            this.lblQdbErrorsRedirect.Text = "Too many errors to show here";
            // 
            // dgQdbErrors
            // 
            this.dgQdbErrors.AllowUserToAddRows = false;
            this.dgQdbErrors.AllowUserToDeleteRows = false;
            this.dgQdbErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgQdbErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumnError,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxBasevehicleid,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10});
            this.dgQdbErrors.Location = new System.Drawing.Point(3, 3);
            this.dgQdbErrors.Name = "dgQdbErrors";
            this.dgQdbErrors.Size = new System.Drawing.Size(1228, 480);
            this.dgQdbErrors.TabIndex = 3;
            // 
            // dataGridViewTextBoxColumnError
            // 
            this.dataGridViewTextBoxColumnError.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumnError.HeaderText = "Error Type";
            this.dataGridViewTextBoxColumnError.Name = "dataGridViewTextBoxColumnError";
            this.dataGridViewTextBoxColumnError.Width = 75;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn1.HeaderText = "App id";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 51;
            // 
            // dataGridViewTextBoxBasevehicleid
            // 
            this.dataGridViewTextBoxBasevehicleid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxBasevehicleid.HeaderText = "Base Vehicle id";
            this.dataGridViewTextBoxBasevehicleid.Name = "dataGridViewTextBoxBasevehicleid";
            this.dataGridViewTextBoxBasevehicleid.Width = 87;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn2.HeaderText = "Make";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 59;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn3.HeaderText = "Model";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 61;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn4.HeaderText = "Year";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 54;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn5.HeaderText = "Part Type";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Width = 72;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn6.HeaderText = "Position";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.Width = 69;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn7.HeaderText = "Qty";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 48;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn8.HeaderText = "Part";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            this.dataGridViewTextBoxColumn8.Width = 51;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn9.HeaderText = "VCdb Coded Attributes";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            this.dataGridViewTextBoxColumn9.Width = 127;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn10.HeaderText = "Notes";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            this.dataGridViewTextBoxColumn10.Width = 60;
            // 
            // tabPageInvalidBasevids
            // 
            this.tabPageInvalidBasevids.Controls.Add(this.lblInvalidBasevehiclesRedirect);
            this.tabPageInvalidBasevids.Controls.Add(this.dgBasevids);
            this.tabPageInvalidBasevids.Location = new System.Drawing.Point(4, 22);
            this.tabPageInvalidBasevids.Name = "tabPageInvalidBasevids";
            this.tabPageInvalidBasevids.Size = new System.Drawing.Size(1234, 436);
            this.tabPageInvalidBasevids.TabIndex = 6;
            this.tabPageInvalidBasevids.Text = "Invalid Basevehicles";
            this.tabPageInvalidBasevids.UseVisualStyleBackColor = true;
            // 
            // lblInvalidBasevehiclesRedirect
            // 
            this.lblInvalidBasevehiclesRedirect.AutoSize = true;
            this.lblInvalidBasevehiclesRedirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInvalidBasevehiclesRedirect.Location = new System.Drawing.Point(47, 40);
            this.lblInvalidBasevehiclesRedirect.Name = "lblInvalidBasevehiclesRedirect";
            this.lblInvalidBasevehiclesRedirect.Size = new System.Drawing.Size(242, 20);
            this.lblInvalidBasevehiclesRedirect.TabIndex = 3;
            this.lblInvalidBasevehiclesRedirect.Text = "Too many problems to show here";
            // 
            // dgBasevids
            // 
            this.dgBasevids.AllowUserToAddRows = false;
            this.dgBasevids.AllowUserToDeleteRows = false;
            this.dgBasevids.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgBasevids.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgBasevidsApplicationid,
            this.dgBasevidsBasevid,
            this.dgBasevidsParttype,
            this.dgBasevidsPosition,
            this.dgBasevidsQty,
            this.dgBasevidsPart,
            this.dgBasevidsQualifiers});
            this.dgBasevids.Location = new System.Drawing.Point(3, 3);
            this.dgBasevids.Name = "dgBasevids";
            this.dgBasevids.Size = new System.Drawing.Size(1228, 480);
            this.dgBasevids.TabIndex = 2;
            // 
            // dgBasevidsApplicationid
            // 
            this.dgBasevidsApplicationid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgBasevidsApplicationid.HeaderText = "App id";
            this.dgBasevidsApplicationid.Name = "dgBasevidsApplicationid";
            this.dgBasevidsApplicationid.ReadOnly = true;
            this.dgBasevidsApplicationid.Width = 51;
            // 
            // dgBasevidsBasevid
            // 
            this.dgBasevidsBasevid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgBasevidsBasevid.HeaderText = "Base Vehicle id";
            this.dgBasevidsBasevid.Name = "dgBasevidsBasevid";
            this.dgBasevidsBasevid.ReadOnly = true;
            this.dgBasevidsBasevid.Width = 87;
            // 
            // dgBasevidsParttype
            // 
            this.dgBasevidsParttype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgBasevidsParttype.HeaderText = "Part Type";
            this.dgBasevidsParttype.Name = "dgBasevidsParttype";
            this.dgBasevidsParttype.ReadOnly = true;
            this.dgBasevidsParttype.Width = 72;
            // 
            // dgBasevidsPosition
            // 
            this.dgBasevidsPosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgBasevidsPosition.HeaderText = "Position";
            this.dgBasevidsPosition.Name = "dgBasevidsPosition";
            this.dgBasevidsPosition.ReadOnly = true;
            this.dgBasevidsPosition.Width = 69;
            // 
            // dgBasevidsQty
            // 
            this.dgBasevidsQty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgBasevidsQty.HeaderText = "Qty";
            this.dgBasevidsQty.Name = "dgBasevidsQty";
            this.dgBasevidsQty.ReadOnly = true;
            this.dgBasevidsQty.Width = 48;
            // 
            // dgBasevidsPart
            // 
            this.dgBasevidsPart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgBasevidsPart.HeaderText = "Part";
            this.dgBasevidsPart.Name = "dgBasevidsPart";
            this.dgBasevidsPart.ReadOnly = true;
            this.dgBasevidsPart.Width = 51;
            // 
            // dgBasevidsQualifiers
            // 
            this.dgBasevidsQualifiers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgBasevidsQualifiers.HeaderText = "Fitment";
            this.dgBasevidsQualifiers.Name = "dgBasevidsQualifiers";
            this.dgBasevidsQualifiers.ReadOnly = true;
            this.dgBasevidsQualifiers.Width = 66;
            // 
            // tabPageInvalidVCdbCodes
            // 
            this.tabPageInvalidVCdbCodes.Controls.Add(this.lblInvalidVCdbCodesRedirect);
            this.tabPageInvalidVCdbCodes.Controls.Add(this.dgVCdbCodes);
            this.tabPageInvalidVCdbCodes.Location = new System.Drawing.Point(4, 22);
            this.tabPageInvalidVCdbCodes.Name = "tabPageInvalidVCdbCodes";
            this.tabPageInvalidVCdbCodes.Size = new System.Drawing.Size(1234, 436);
            this.tabPageInvalidVCdbCodes.TabIndex = 7;
            this.tabPageInvalidVCdbCodes.Text = "Invalid VCdb codes";
            this.tabPageInvalidVCdbCodes.UseVisualStyleBackColor = true;
            // 
            // lblInvalidVCdbCodesRedirect
            // 
            this.lblInvalidVCdbCodesRedirect.AutoSize = true;
            this.lblInvalidVCdbCodesRedirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInvalidVCdbCodesRedirect.Location = new System.Drawing.Point(47, 40);
            this.lblInvalidVCdbCodesRedirect.Name = "lblInvalidVCdbCodesRedirect";
            this.lblInvalidVCdbCodesRedirect.Size = new System.Drawing.Size(242, 20);
            this.lblInvalidVCdbCodesRedirect.TabIndex = 3;
            this.lblInvalidVCdbCodesRedirect.Text = "Too many problems to show here";
            // 
            // dgVCdbCodes
            // 
            this.dgVCdbCodes.AllowUserToAddRows = false;
            this.dgVCdbCodes.AllowUserToDeleteRows = false;
            this.dgVCdbCodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgVCdbCodes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgVCdbCodesApplicationid,
            this.dgVCdbCodesMake,
            this.dgVCdbCodesModel,
            this.dgVCdbCodesYear,
            this.dgVCdbCodesParttype,
            this.dgVCdbCodesPosition,
            this.dgVCdbCodesQty,
            this.dgVCdbCodesPart,
            this.dgVCdbCodesQualifiers,
            this.dgVCdbCodesNotes});
            this.dgVCdbCodes.Location = new System.Drawing.Point(3, 3);
            this.dgVCdbCodes.Name = "dgVCdbCodes";
            this.dgVCdbCodes.Size = new System.Drawing.Size(1228, 480);
            this.dgVCdbCodes.TabIndex = 2;
            // 
            // dgVCdbCodesApplicationid
            // 
            this.dgVCdbCodesApplicationid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesApplicationid.HeaderText = "App id";
            this.dgVCdbCodesApplicationid.Name = "dgVCdbCodesApplicationid";
            this.dgVCdbCodesApplicationid.ReadOnly = true;
            this.dgVCdbCodesApplicationid.Width = 51;
            // 
            // dgVCdbCodesMake
            // 
            this.dgVCdbCodesMake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesMake.HeaderText = "Make";
            this.dgVCdbCodesMake.Name = "dgVCdbCodesMake";
            this.dgVCdbCodesMake.ReadOnly = true;
            this.dgVCdbCodesMake.Width = 59;
            // 
            // dgVCdbCodesModel
            // 
            this.dgVCdbCodesModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesModel.HeaderText = "Model";
            this.dgVCdbCodesModel.Name = "dgVCdbCodesModel";
            this.dgVCdbCodesModel.ReadOnly = true;
            this.dgVCdbCodesModel.Width = 61;
            // 
            // dgVCdbCodesYear
            // 
            this.dgVCdbCodesYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesYear.HeaderText = "Year";
            this.dgVCdbCodesYear.Name = "dgVCdbCodesYear";
            this.dgVCdbCodesYear.ReadOnly = true;
            this.dgVCdbCodesYear.Width = 54;
            // 
            // dgVCdbCodesParttype
            // 
            this.dgVCdbCodesParttype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesParttype.HeaderText = "Part Type";
            this.dgVCdbCodesParttype.Name = "dgVCdbCodesParttype";
            this.dgVCdbCodesParttype.ReadOnly = true;
            this.dgVCdbCodesParttype.Width = 72;
            // 
            // dgVCdbCodesPosition
            // 
            this.dgVCdbCodesPosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesPosition.HeaderText = "Position";
            this.dgVCdbCodesPosition.Name = "dgVCdbCodesPosition";
            this.dgVCdbCodesPosition.ReadOnly = true;
            this.dgVCdbCodesPosition.Width = 69;
            // 
            // dgVCdbCodesQty
            // 
            this.dgVCdbCodesQty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesQty.HeaderText = "Qty";
            this.dgVCdbCodesQty.Name = "dgVCdbCodesQty";
            this.dgVCdbCodesQty.ReadOnly = true;
            this.dgVCdbCodesQty.Width = 48;
            // 
            // dgVCdbCodesPart
            // 
            this.dgVCdbCodesPart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesPart.HeaderText = "Part";
            this.dgVCdbCodesPart.Name = "dgVCdbCodesPart";
            this.dgVCdbCodesPart.ReadOnly = true;
            this.dgVCdbCodesPart.Width = 51;
            // 
            // dgVCdbCodesQualifiers
            // 
            this.dgVCdbCodesQualifiers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesQualifiers.HeaderText = "VCdb Coded Attributes";
            this.dgVCdbCodesQualifiers.Name = "dgVCdbCodesQualifiers";
            this.dgVCdbCodesQualifiers.ReadOnly = true;
            this.dgVCdbCodesQualifiers.Width = 127;
            // 
            // dgVCdbCodesNotes
            // 
            this.dgVCdbCodesNotes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbCodesNotes.HeaderText = "Notes";
            this.dgVCdbCodesNotes.Name = "dgVCdbCodesNotes";
            this.dgVCdbCodesNotes.ReadOnly = true;
            this.dgVCdbCodesNotes.Width = 60;
            // 
            // tabPageInvalidConfigs
            // 
            this.tabPageInvalidConfigs.Controls.Add(this.lblVCdbConfigErrorRedirect);
            this.tabPageInvalidConfigs.Controls.Add(this.dgVCdbConfigs);
            this.tabPageInvalidConfigs.Location = new System.Drawing.Point(4, 22);
            this.tabPageInvalidConfigs.Name = "tabPageInvalidConfigs";
            this.tabPageInvalidConfigs.Size = new System.Drawing.Size(1234, 436);
            this.tabPageInvalidConfigs.TabIndex = 8;
            this.tabPageInvalidConfigs.Text = "Invalid Configurations";
            this.tabPageInvalidConfigs.UseVisualStyleBackColor = true;
            // 
            // lblVCdbConfigErrorRedirect
            // 
            this.lblVCdbConfigErrorRedirect.AutoSize = true;
            this.lblVCdbConfigErrorRedirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVCdbConfigErrorRedirect.Location = new System.Drawing.Point(46, 38);
            this.lblVCdbConfigErrorRedirect.Name = "lblVCdbConfigErrorRedirect";
            this.lblVCdbConfigErrorRedirect.Size = new System.Drawing.Size(265, 20);
            this.lblVCdbConfigErrorRedirect.TabIndex = 4;
            this.lblVCdbConfigErrorRedirect.Text = "Too many config errors to show here";
            // 
            // dgVCdbConfigs
            // 
            this.dgVCdbConfigs.AllowUserToAddRows = false;
            this.dgVCdbConfigs.AllowUserToDeleteRows = false;
            this.dgVCdbConfigs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgVCdbConfigs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgVCdbConfigsApplicationid,
            this.dgVCdbConfigsBasevehicleid,
            this.dgVCdbConfigsMake,
            this.dgVCdbConfigsModel,
            this.dgVCdbConfigsYear,
            this.dgVCdbConfigsParttype,
            this.dgVCdbConfigsPosition,
            this.dgVCdbConfigsQty,
            this.dgVCdbConfigsPart,
            this.dgVCdbConfigsVCdbAttributes,
            this.dgVCdbConfigsQdbQualifiers,
            this.dgVCdbConfigsNotes});
            this.dgVCdbConfigs.Location = new System.Drawing.Point(3, 3);
            this.dgVCdbConfigs.Name = "dgVCdbConfigs";
            this.dgVCdbConfigs.Size = new System.Drawing.Size(1228, 480);
            this.dgVCdbConfigs.TabIndex = 3;
            this.dgVCdbConfigs.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgVCdbConfigs_SortCompare);
            // 
            // dgVCdbConfigsApplicationid
            // 
            this.dgVCdbConfigsApplicationid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsApplicationid.HeaderText = "App id";
            this.dgVCdbConfigsApplicationid.Name = "dgVCdbConfigsApplicationid";
            this.dgVCdbConfigsApplicationid.ReadOnly = true;
            this.dgVCdbConfigsApplicationid.Width = 51;
            // 
            // dgVCdbConfigsBasevehicleid
            // 
            this.dgVCdbConfigsBasevehicleid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsBasevehicleid.HeaderText = "Base Vehicle id";
            this.dgVCdbConfigsBasevehicleid.Name = "dgVCdbConfigsBasevehicleid";
            this.dgVCdbConfigsBasevehicleid.ReadOnly = true;
            this.dgVCdbConfigsBasevehicleid.Width = 87;
            // 
            // dgVCdbConfigsMake
            // 
            this.dgVCdbConfigsMake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsMake.HeaderText = "Make";
            this.dgVCdbConfigsMake.Name = "dgVCdbConfigsMake";
            this.dgVCdbConfigsMake.ReadOnly = true;
            this.dgVCdbConfigsMake.Width = 59;
            // 
            // dgVCdbConfigsModel
            // 
            this.dgVCdbConfigsModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsModel.HeaderText = "Model";
            this.dgVCdbConfigsModel.Name = "dgVCdbConfigsModel";
            this.dgVCdbConfigsModel.ReadOnly = true;
            this.dgVCdbConfigsModel.Width = 61;
            // 
            // dgVCdbConfigsYear
            // 
            this.dgVCdbConfigsYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsYear.HeaderText = "Year";
            this.dgVCdbConfigsYear.Name = "dgVCdbConfigsYear";
            this.dgVCdbConfigsYear.ReadOnly = true;
            this.dgVCdbConfigsYear.Width = 54;
            // 
            // dgVCdbConfigsParttype
            // 
            this.dgVCdbConfigsParttype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsParttype.HeaderText = "Part Type";
            this.dgVCdbConfigsParttype.Name = "dgVCdbConfigsParttype";
            this.dgVCdbConfigsParttype.ReadOnly = true;
            this.dgVCdbConfigsParttype.Width = 72;
            // 
            // dgVCdbConfigsPosition
            // 
            this.dgVCdbConfigsPosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsPosition.HeaderText = "Position";
            this.dgVCdbConfigsPosition.Name = "dgVCdbConfigsPosition";
            this.dgVCdbConfigsPosition.ReadOnly = true;
            this.dgVCdbConfigsPosition.Width = 69;
            // 
            // dgVCdbConfigsQty
            // 
            this.dgVCdbConfigsQty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsQty.HeaderText = "Qty";
            this.dgVCdbConfigsQty.Name = "dgVCdbConfigsQty";
            this.dgVCdbConfigsQty.ReadOnly = true;
            this.dgVCdbConfigsQty.Width = 48;
            // 
            // dgVCdbConfigsPart
            // 
            this.dgVCdbConfigsPart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsPart.HeaderText = "Part";
            this.dgVCdbConfigsPart.Name = "dgVCdbConfigsPart";
            this.dgVCdbConfigsPart.ReadOnly = true;
            this.dgVCdbConfigsPart.Width = 51;
            // 
            // dgVCdbConfigsVCdbAttributes
            // 
            this.dgVCdbConfigsVCdbAttributes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsVCdbAttributes.HeaderText = "VCdb Coded Attributes";
            this.dgVCdbConfigsVCdbAttributes.Name = "dgVCdbConfigsVCdbAttributes";
            this.dgVCdbConfigsVCdbAttributes.ReadOnly = true;
            this.dgVCdbConfigsVCdbAttributes.Width = 127;
            // 
            // dgVCdbConfigsQdbQualifiers
            // 
            this.dgVCdbConfigsQdbQualifiers.HeaderText = "Qdb-coded Qualifiers";
            this.dgVCdbConfigsQdbQualifiers.Name = "dgVCdbConfigsQdbQualifiers";
            // 
            // dgVCdbConfigsNotes
            // 
            this.dgVCdbConfigsNotes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgVCdbConfigsNotes.HeaderText = "Notes";
            this.dgVCdbConfigsNotes.Name = "dgVCdbConfigsNotes";
            this.dgVCdbConfigsNotes.ReadOnly = true;
            this.dgVCdbConfigsNotes.Width = 60;
            // 
            // tabPageAddsDropsParts
            // 
            this.tabPageAddsDropsParts.Controls.Add(this.dgAddsDropsParts);
            this.tabPageAddsDropsParts.Location = new System.Drawing.Point(4, 22);
            this.tabPageAddsDropsParts.Name = "tabPageAddsDropsParts";
            this.tabPageAddsDropsParts.Size = new System.Drawing.Size(1234, 436);
            this.tabPageAddsDropsParts.TabIndex = 12;
            this.tabPageAddsDropsParts.Text = "Adds/Drops Parts";
            this.tabPageAddsDropsParts.UseVisualStyleBackColor = true;
            // 
            // dgAddsDropsParts
            // 
            this.dgAddsDropsParts.AllowUserToAddRows = false;
            this.dgAddsDropsParts.AllowUserToDeleteRows = false;
            this.dgAddsDropsParts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAddsDropsParts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgAddsDropsPartsAction,
            this.dataGridViewTextBoxPart});
            this.dgAddsDropsParts.Location = new System.Drawing.Point(3, 3);
            this.dgAddsDropsParts.Name = "dgAddsDropsParts";
            this.dgAddsDropsParts.Size = new System.Drawing.Size(1228, 480);
            this.dgAddsDropsParts.TabIndex = 2;
            // 
            // dgAddsDropsPartsAction
            // 
            this.dgAddsDropsPartsAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsPartsAction.HeaderText = "Action";
            this.dgAddsDropsPartsAction.Name = "dgAddsDropsPartsAction";
            this.dgAddsDropsPartsAction.ReadOnly = true;
            this.dgAddsDropsPartsAction.Width = 62;
            // 
            // dataGridViewTextBoxPart
            // 
            this.dataGridViewTextBoxPart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxPart.HeaderText = "Part";
            this.dataGridViewTextBoxPart.Name = "dataGridViewTextBoxPart";
            this.dataGridViewTextBoxPart.ReadOnly = true;
            this.dataGridViewTextBoxPart.Width = 51;
            // 
            // tabPageAddsDropsVehicles
            // 
            this.tabPageAddsDropsVehicles.Controls.Add(this.dgAddsDropsVehicles);
            this.tabPageAddsDropsVehicles.Location = new System.Drawing.Point(4, 22);
            this.tabPageAddsDropsVehicles.Name = "tabPageAddsDropsVehicles";
            this.tabPageAddsDropsVehicles.Size = new System.Drawing.Size(1234, 436);
            this.tabPageAddsDropsVehicles.TabIndex = 13;
            this.tabPageAddsDropsVehicles.Text = "Adds/Drops Vehicles";
            this.tabPageAddsDropsVehicles.UseVisualStyleBackColor = true;
            // 
            // dgAddsDropsVehicles
            // 
            this.dgAddsDropsVehicles.AllowUserToAddRows = false;
            this.dgAddsDropsVehicles.AllowUserToDeleteRows = false;
            this.dgAddsDropsVehicles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgAddsDropsVehicles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgAddsDropsVehiclesAction,
            this.dgAddsDropsVehiclesBaseVid,
            this.dgAddsDropsVehiclesMake,
            this.dgAddsDropsVehiclesModel,
            this.dgAddsDropsVehiclesYear,
            this.dgAddsDropsVehiclesParttype,
            this.dgAddsDropsVehiclesPosition,
            this.dgAddsDropsVehiclesQualifiers,
            this.dgAddsDropsVehiclesMfrLabel});
            this.dgAddsDropsVehicles.Location = new System.Drawing.Point(3, 3);
            this.dgAddsDropsVehicles.Name = "dgAddsDropsVehicles";
            this.dgAddsDropsVehicles.Size = new System.Drawing.Size(1228, 480);
            this.dgAddsDropsVehicles.TabIndex = 3;
            // 
            // dgAddsDropsVehiclesAction
            // 
            this.dgAddsDropsVehiclesAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesAction.HeaderText = "Action";
            this.dgAddsDropsVehiclesAction.Name = "dgAddsDropsVehiclesAction";
            this.dgAddsDropsVehiclesAction.ReadOnly = true;
            this.dgAddsDropsVehiclesAction.Width = 62;
            // 
            // dgAddsDropsVehiclesBaseVid
            // 
            this.dgAddsDropsVehiclesBaseVid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesBaseVid.HeaderText = "BaseVehicle id";
            this.dgAddsDropsVehiclesBaseVid.Name = "dgAddsDropsVehiclesBaseVid";
            this.dgAddsDropsVehiclesBaseVid.ReadOnly = true;
            this.dgAddsDropsVehiclesBaseVid.Width = 102;
            // 
            // dgAddsDropsVehiclesMake
            // 
            this.dgAddsDropsVehiclesMake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesMake.HeaderText = "Make";
            this.dgAddsDropsVehiclesMake.Name = "dgAddsDropsVehiclesMake";
            this.dgAddsDropsVehiclesMake.ReadOnly = true;
            this.dgAddsDropsVehiclesMake.Width = 59;
            // 
            // dgAddsDropsVehiclesModel
            // 
            this.dgAddsDropsVehiclesModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesModel.HeaderText = "Model";
            this.dgAddsDropsVehiclesModel.Name = "dgAddsDropsVehiclesModel";
            this.dgAddsDropsVehiclesModel.ReadOnly = true;
            this.dgAddsDropsVehiclesModel.Width = 61;
            // 
            // dgAddsDropsVehiclesYear
            // 
            this.dgAddsDropsVehiclesYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesYear.HeaderText = "Year";
            this.dgAddsDropsVehiclesYear.Name = "dgAddsDropsVehiclesYear";
            this.dgAddsDropsVehiclesYear.ReadOnly = true;
            this.dgAddsDropsVehiclesYear.Width = 54;
            // 
            // dgAddsDropsVehiclesParttype
            // 
            this.dgAddsDropsVehiclesParttype.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesParttype.HeaderText = "Part Type";
            this.dgAddsDropsVehiclesParttype.Name = "dgAddsDropsVehiclesParttype";
            this.dgAddsDropsVehiclesParttype.ReadOnly = true;
            this.dgAddsDropsVehiclesParttype.Width = 78;
            // 
            // dgAddsDropsVehiclesPosition
            // 
            this.dgAddsDropsVehiclesPosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesPosition.HeaderText = "Position";
            this.dgAddsDropsVehiclesPosition.Name = "dgAddsDropsVehiclesPosition";
            this.dgAddsDropsVehiclesPosition.ReadOnly = true;
            this.dgAddsDropsVehiclesPosition.Width = 69;
            // 
            // dgAddsDropsVehiclesQualifiers
            // 
            this.dgAddsDropsVehiclesQualifiers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesQualifiers.HeaderText = "Fitment";
            this.dgAddsDropsVehiclesQualifiers.Name = "dgAddsDropsVehiclesQualifiers";
            this.dgAddsDropsVehiclesQualifiers.ReadOnly = true;
            this.dgAddsDropsVehiclesQualifiers.Width = 66;
            // 
            // dgAddsDropsVehiclesMfrLabel
            // 
            this.dgAddsDropsVehiclesMfrLabel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgAddsDropsVehiclesMfrLabel.HeaderText = "MfrLabel";
            this.dgAddsDropsVehiclesMfrLabel.Name = "dgAddsDropsVehiclesMfrLabel";
            this.dgAddsDropsVehiclesMfrLabel.ReadOnly = true;
            this.dgAddsDropsVehiclesMfrLabel.Width = 73;
            // 
            // tabPageQuantityWarnings
            // 
            this.tabPageQuantityWarnings.Controls.Add(this.lblQtyWarningsRedirect);
            this.tabPageQuantityWarnings.Controls.Add(this.dgQuantityWarnings);
            this.tabPageQuantityWarnings.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuantityWarnings.Name = "tabPageQuantityWarnings";
            this.tabPageQuantityWarnings.Size = new System.Drawing.Size(1234, 436);
            this.tabPageQuantityWarnings.TabIndex = 16;
            this.tabPageQuantityWarnings.Text = "Quantity Warnings";
            this.tabPageQuantityWarnings.UseVisualStyleBackColor = true;
            // 
            // lblQtyWarningsRedirect
            // 
            this.lblQtyWarningsRedirect.AutoSize = true;
            this.lblQtyWarningsRedirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQtyWarningsRedirect.Location = new System.Drawing.Point(45, 37);
            this.lblQtyWarningsRedirect.Name = "lblQtyWarningsRedirect";
            this.lblQtyWarningsRedirect.Size = new System.Drawing.Size(265, 20);
            this.lblQtyWarningsRedirect.TabIndex = 6;
            this.lblQtyWarningsRedirect.Text = "Too many qty warnings to show here";
            // 
            // dgQuantityWarnings
            // 
            this.dgQuantityWarnings.AllowUserToAddRows = false;
            this.dgQuantityWarnings.AllowUserToDeleteRows = false;
            this.dgQuantityWarnings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgQuantityWarnings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewTextBoxColumn12,
            this.dataGridViewTextBoxColumn13,
            this.dataGridViewTextBoxColumn14,
            this.dataGridViewTextBoxColumn15,
            this.dataGridViewTextBoxColumn16,
            this.dataGridViewTextBoxColumn17,
            this.dataGridViewTextBoxColumn18,
            this.dataGridViewTextBoxColumn19,
            this.dataGridViewTextBoxColumn21,
            this.dataGridViewTextBoxColumn20,
            this.dataGridViewTextBoxColumn201,
            this.dataGridViewTextBoxColumn22});
            this.dgQuantityWarnings.Location = new System.Drawing.Point(3, 3);
            this.dgQuantityWarnings.Name = "dgQuantityWarnings";
            this.dgQuantityWarnings.Size = new System.Drawing.Size(1228, 480);
            this.dgQuantityWarnings.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn11.HeaderText = "Error";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.ReadOnly = true;
            this.dataGridViewTextBoxColumn11.Width = 54;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn12.HeaderText = "App id";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.ReadOnly = true;
            this.dataGridViewTextBoxColumn12.Width = 51;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn13.HeaderText = "BaseVehicle Id";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.ReadOnly = true;
            this.dataGridViewTextBoxColumn13.Width = 95;
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn14.HeaderText = "Make";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.ReadOnly = true;
            this.dataGridViewTextBoxColumn14.Width = 59;
            // 
            // dataGridViewTextBoxColumn15
            // 
            this.dataGridViewTextBoxColumn15.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn15.HeaderText = "Model";
            this.dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
            this.dataGridViewTextBoxColumn15.ReadOnly = true;
            this.dataGridViewTextBoxColumn15.Width = 61;
            // 
            // dataGridViewTextBoxColumn16
            // 
            this.dataGridViewTextBoxColumn16.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn16.HeaderText = "Year";
            this.dataGridViewTextBoxColumn16.Name = "dataGridViewTextBoxColumn16";
            this.dataGridViewTextBoxColumn16.ReadOnly = true;
            this.dataGridViewTextBoxColumn16.Width = 54;
            // 
            // dataGridViewTextBoxColumn17
            // 
            this.dataGridViewTextBoxColumn17.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn17.HeaderText = "Part Type";
            this.dataGridViewTextBoxColumn17.Name = "dataGridViewTextBoxColumn17";
            this.dataGridViewTextBoxColumn17.ReadOnly = true;
            this.dataGridViewTextBoxColumn17.Width = 72;
            // 
            // dataGridViewTextBoxColumn18
            // 
            this.dataGridViewTextBoxColumn18.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn18.HeaderText = "Position";
            this.dataGridViewTextBoxColumn18.Name = "dataGridViewTextBoxColumn18";
            this.dataGridViewTextBoxColumn18.ReadOnly = true;
            this.dataGridViewTextBoxColumn18.Width = 69;
            // 
            // dataGridViewTextBoxColumn19
            // 
            this.dataGridViewTextBoxColumn19.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn19.HeaderText = "Qty";
            this.dataGridViewTextBoxColumn19.Name = "dataGridViewTextBoxColumn19";
            this.dataGridViewTextBoxColumn19.ReadOnly = true;
            this.dataGridViewTextBoxColumn19.Width = 48;
            // 
            // dataGridViewTextBoxColumn21
            // 
            this.dataGridViewTextBoxColumn21.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn21.HeaderText = "Part";
            this.dataGridViewTextBoxColumn21.Name = "dataGridViewTextBoxColumn21";
            this.dataGridViewTextBoxColumn21.ReadOnly = true;
            this.dataGridViewTextBoxColumn21.Width = 51;
            // 
            // dataGridViewTextBoxColumn20
            // 
            this.dataGridViewTextBoxColumn20.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn20.HeaderText = "VCdb-coded attributes";
            this.dataGridViewTextBoxColumn20.Name = "dataGridViewTextBoxColumn20";
            this.dataGridViewTextBoxColumn20.ReadOnly = true;
            this.dataGridViewTextBoxColumn20.Width = 125;
            // 
            // dataGridViewTextBoxColumn201
            // 
            this.dataGridViewTextBoxColumn201.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn201.HeaderText = "Qdb-coded attributes";
            this.dataGridViewTextBoxColumn201.Name = "dataGridViewTextBoxColumn201";
            this.dataGridViewTextBoxColumn201.Width = 120;
            // 
            // dataGridViewTextBoxColumn22
            // 
            this.dataGridViewTextBoxColumn22.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn22.HeaderText = "Notes";
            this.dataGridViewTextBoxColumn22.Name = "dataGridViewTextBoxColumn22";
            this.dataGridViewTextBoxColumn22.Width = 60;
            // 
            // tabPageFitmentLogic
            // 
            this.tabPageFitmentLogic.Controls.Add(this.splitContainerFitmentLogic);
            this.tabPageFitmentLogic.Location = new System.Drawing.Point(4, 22);
            this.tabPageFitmentLogic.Name = "tabPageFitmentLogic";
            this.tabPageFitmentLogic.Size = new System.Drawing.Size(1234, 436);
            this.tabPageFitmentLogic.TabIndex = 18;
            this.tabPageFitmentLogic.Text = "Fitment Logic Problems";
            this.tabPageFitmentLogic.UseVisualStyleBackColor = true;
            // 
            // splitContainerFitmentLogic
            // 
            this.splitContainerFitmentLogic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerFitmentLogic.Location = new System.Drawing.Point(-4, 3);
            this.splitContainerFitmentLogic.Name = "splitContainerFitmentLogic";
            this.splitContainerFitmentLogic.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerFitmentLogic.Panel1
            // 
            this.splitContainerFitmentLogic.Panel1.Controls.Add(this.lblFitmentLogicProblemsTabRedirect);
            this.splitContainerFitmentLogic.Panel1.Controls.Add(this.dgFitmentLogicProblems);
            // 
            // splitContainerFitmentLogic.Panel2
            // 
            this.splitContainerFitmentLogic.Panel2.Controls.Add(this.listBoxFitmentLogicElements);
            this.splitContainerFitmentLogic.Panel2.Controls.Add(this.pictureBoxFitmentTree);
            this.splitContainerFitmentLogic.Size = new System.Drawing.Size(1238, 529);
            this.splitContainerFitmentLogic.SplitterDistance = 263;
            this.splitContainerFitmentLogic.TabIndex = 6;
            this.splitContainerFitmentLogic.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerFitmentLogic_SplitterMoved);
            // 
            // lblFitmentLogicProblemsTabRedirect
            // 
            this.lblFitmentLogicProblemsTabRedirect.AutoSize = true;
            this.lblFitmentLogicProblemsTabRedirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFitmentLogicProblemsTabRedirect.Location = new System.Drawing.Point(49, 33);
            this.lblFitmentLogicProblemsTabRedirect.Name = "lblFitmentLogicProblemsTabRedirect";
            this.lblFitmentLogicProblemsTabRedirect.Size = new System.Drawing.Size(331, 20);
            this.lblFitmentLogicProblemsTabRedirect.TabIndex = 5;
            this.lblFitmentLogicProblemsTabRedirect.Text = "Fitment problems list is too large to show here";
            // 
            // dgFitmentLogicProblems
            // 
            this.dgFitmentLogicProblems.AllowUserToAddRows = false;
            this.dgFitmentLogicProblems.AllowUserToDeleteRows = false;
            this.dgFitmentLogicProblems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFitmentLogicProblems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgLogicProblemsDescription,
            this.dgLogicProblemsGroup,
            this.dgLogicProblemsAppId,
            this.dgLogicProblemsReference,
            this.dgLogicProblemsBaseVehicleId,
            this.dgLogicProblemsMake,
            this.dgLogicProblemsModel,
            this.dgLogicProblemsYear,
            this.dgLogicProblemsPartType,
            this.dgLogicProblemsPosition,
            this.dgLogicProblemsQty,
            this.dgLogicProblemsPart,
            this.dgLogicProblemsFitment});
            this.dgFitmentLogicProblems.Location = new System.Drawing.Point(9, 0);
            this.dgFitmentLogicProblems.Name = "dgFitmentLogicProblems";
            this.dgFitmentLogicProblems.Size = new System.Drawing.Size(1228, 258);
            this.dgFitmentLogicProblems.TabIndex = 4;
            this.dgFitmentLogicProblems.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgFitmentLogicProblems_CellEnter);
            // 
            // listBoxFitmentLogicElements
            // 
            this.listBoxFitmentLogicElements.FormattingEnabled = true;
            this.listBoxFitmentLogicElements.Location = new System.Drawing.Point(1037, 4);
            this.listBoxFitmentLogicElements.Name = "listBoxFitmentLogicElements";
            this.listBoxFitmentLogicElements.Size = new System.Drawing.Size(196, 251);
            this.listBoxFitmentLogicElements.TabIndex = 6;
            this.listBoxFitmentLogicElements.Click += new System.EventHandler(this.listBoxFitmentLogicElements_Click);
            this.listBoxFitmentLogicElements.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBoxFitmentLogicElements_MouseClick);
            // 
            // pictureBoxFitmentTree
            // 
            this.pictureBoxFitmentTree.BackColor = System.Drawing.Color.Silver;
            this.pictureBoxFitmentTree.Location = new System.Drawing.Point(8, 3);
            this.pictureBoxFitmentTree.Name = "pictureBoxFitmentTree";
            this.pictureBoxFitmentTree.Size = new System.Drawing.Size(1023, 252);
            this.pictureBoxFitmentTree.TabIndex = 5;
            this.pictureBoxFitmentTree.TabStop = false;
            this.pictureBoxFitmentTree.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxFitmentTree_Paint);
            this.pictureBoxFitmentTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxFitmentTree_MouseDown);
            this.pictureBoxFitmentTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxFitmentTree_MouseMove);
            this.pictureBoxFitmentTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxFitmentTree_MouseUp);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(157, 216);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(389, 20);
            this.lblStatus.TabIndex = 13;
            this.lblStatus.Text = "l1 label1 label1 label1 label1 label1 label1 label1 label1";
            // 
            // lblAppVersion
            // 
            this.lblAppVersion.AutoSize = true;
            this.lblAppVersion.Location = new System.Drawing.Point(1134, 9);
            this.lblAppVersion.Name = "lblAppVersion";
            this.lblAppVersion.Size = new System.Drawing.Size(16, 13);
            this.lblAppVersion.TabIndex = 33;
            this.lblAppVersion.Text = "...";
            // 
            // toolTip1
            // 
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // btnSelectReferenceACESfile
            // 
            this.btnSelectReferenceACESfile.Location = new System.Drawing.Point(12, 40);
            this.btnSelectReferenceACESfile.Name = "btnSelectReferenceACESfile";
            this.btnSelectReferenceACESfile.Size = new System.Drawing.Size(139, 22);
            this.btnSelectReferenceACESfile.TabIndex = 34;
            this.btnSelectReferenceACESfile.Text = "Reference ACES file";
            this.btnSelectReferenceACESfile.UseVisualStyleBackColor = true;
            this.btnSelectReferenceACESfile.Click += new System.EventHandler(this.btnSelectReferenceACESfile_Click);
            // 
            // lblReferenceACESfilePath
            // 
            this.lblReferenceACESfilePath.AutoSize = true;
            this.lblReferenceACESfilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReferenceACESfilePath.Location = new System.Drawing.Point(352, 43);
            this.lblReferenceACESfilePath.Name = "lblReferenceACESfilePath";
            this.lblReferenceACESfilePath.Size = new System.Drawing.Size(45, 16);
            this.lblReferenceACESfilePath.TabIndex = 35;
            this.lblReferenceACESfilePath.Text = "label1";
            // 
            // btnSelectPartInterchange
            // 
            this.btnSelectPartInterchange.Location = new System.Drawing.Point(12, 68);
            this.btnSelectPartInterchange.Name = "btnSelectPartInterchange";
            this.btnSelectPartInterchange.Size = new System.Drawing.Size(139, 23);
            this.btnSelectPartInterchange.TabIndex = 36;
            this.btnSelectPartInterchange.Text = "Part Translation file";
            this.btnSelectPartInterchange.UseVisualStyleBackColor = true;
            this.btnSelectPartInterchange.Click += new System.EventHandler(this.btnSelectPartInterchange_Click);
            // 
            // lblinterchangefilePath
            // 
            this.lblinterchangefilePath.AutoSize = true;
            this.lblinterchangefilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblinterchangefilePath.Location = new System.Drawing.Point(151, 71);
            this.lblinterchangefilePath.Name = "lblinterchangefilePath";
            this.lblinterchangefilePath.Size = new System.Drawing.Size(45, 16);
            this.lblinterchangefilePath.TabIndex = 37;
            this.lblinterchangefilePath.Text = "label1";
            // 
            // btnSelectQdbFile
            // 
            this.btnSelectQdbFile.Location = new System.Drawing.Point(12, 180);
            this.btnSelectQdbFile.Name = "btnSelectQdbFile";
            this.btnSelectQdbFile.Size = new System.Drawing.Size(139, 21);
            this.btnSelectQdbFile.TabIndex = 38;
            this.btnSelectQdbFile.Text = "Select Qdb file";
            this.btnSelectQdbFile.UseVisualStyleBackColor = true;
            this.btnSelectQdbFile.Click += new System.EventHandler(this.btnSelectQdbFile_Click);
            // 
            // lblQdbFilePath
            // 
            this.lblQdbFilePath.AutoSize = true;
            this.lblQdbFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQdbFilePath.Location = new System.Drawing.Point(152, 184);
            this.lblQdbFilePath.Name = "lblQdbFilePath";
            this.lblQdbFilePath.Size = new System.Drawing.Size(45, 16);
            this.lblQdbFilePath.TabIndex = 39;
            this.lblQdbFilePath.Text = "label1";
            // 
            // btnSelectNoteTranslationFile
            // 
            this.btnSelectNoteTranslationFile.Location = new System.Drawing.Point(12, 97);
            this.btnSelectNoteTranslationFile.Name = "btnSelectNoteTranslationFile";
            this.btnSelectNoteTranslationFile.Size = new System.Drawing.Size(139, 23);
            this.btnSelectNoteTranslationFile.TabIndex = 40;
            this.btnSelectNoteTranslationFile.Text = "Note Translation File";
            this.btnSelectNoteTranslationFile.UseVisualStyleBackColor = true;
            this.btnSelectNoteTranslationFile.Click += new System.EventHandler(this.btnSelectNoteInterchangeFile_Click);
            // 
            // lblNoteTranslationfilePath
            // 
            this.lblNoteTranslationfilePath.AutoSize = true;
            this.lblNoteTranslationfilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoteTranslationfilePath.Location = new System.Drawing.Point(151, 100);
            this.lblNoteTranslationfilePath.Name = "lblNoteTranslationfilePath";
            this.lblNoteTranslationfilePath.Size = new System.Drawing.Size(45, 16);
            this.lblNoteTranslationfilePath.TabIndex = 41;
            this.lblNoteTranslationfilePath.Text = "label1";
            // 
            // timerHistoryUpdate
            // 
            this.timerHistoryUpdate.Enabled = true;
            this.timerHistoryUpdate.Interval = 200;
            this.timerHistoryUpdate.Tick += new System.EventHandler(this.timerHistoryUpdate_Tick);
            // 
            // comboBoxMySQLvcdbVersion
            // 
            this.comboBoxMySQLvcdbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMySQLvcdbVersion.FormattingEnabled = true;
            this.comboBoxMySQLvcdbVersion.Location = new System.Drawing.Point(12, 126);
            this.comboBoxMySQLvcdbVersion.Name = "comboBoxMySQLvcdbVersion";
            this.comboBoxMySQLvcdbVersion.Size = new System.Drawing.Size(97, 21);
            this.comboBoxMySQLvcdbVersion.TabIndex = 42;
            // 
            // buttonMySQLloadVCdb
            // 
            this.buttonMySQLloadVCdb.Location = new System.Drawing.Point(111, 125);
            this.buttonMySQLloadVCdb.Name = "buttonMySQLloadVCdb";
            this.buttonMySQLloadVCdb.Size = new System.Drawing.Size(40, 23);
            this.buttonMySQLloadVCdb.TabIndex = 43;
            this.buttonMySQLloadVCdb.Text = "Load";
            this.buttonMySQLloadVCdb.UseVisualStyleBackColor = true;
            this.buttonMySQLloadVCdb.Click += new System.EventHandler(this.buttonMySQLloadVCdb_Click);
            // 
            // progBarVCdbload
            // 
            this.progBarVCdbload.Location = new System.Drawing.Point(157, 128);
            this.progBarVCdbload.Name = "progBarVCdbload";
            this.progBarVCdbload.Size = new System.Drawing.Size(148, 18);
            this.progBarVCdbload.TabIndex = 44;
            // 
            // comboBoxMySQLpcdbVersion
            // 
            this.comboBoxMySQLpcdbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMySQLpcdbVersion.FormattingEnabled = true;
            this.comboBoxMySQLpcdbVersion.Location = new System.Drawing.Point(12, 153);
            this.comboBoxMySQLpcdbVersion.Name = "comboBoxMySQLpcdbVersion";
            this.comboBoxMySQLpcdbVersion.Size = new System.Drawing.Size(97, 21);
            this.comboBoxMySQLpcdbVersion.TabIndex = 45;
            // 
            // buttonMySQLloadPCdb
            // 
            this.buttonMySQLloadPCdb.Location = new System.Drawing.Point(111, 152);
            this.buttonMySQLloadPCdb.Name = "buttonMySQLloadPCdb";
            this.buttonMySQLloadPCdb.Size = new System.Drawing.Size(40, 23);
            this.buttonMySQLloadPCdb.TabIndex = 46;
            this.buttonMySQLloadPCdb.Text = "Load";
            this.buttonMySQLloadPCdb.UseVisualStyleBackColor = true;
            this.buttonMySQLloadPCdb.Click += new System.EventHandler(this.buttonMySQLloadPCdb_Click);
            // 
            // comboBoxMySQLqdbVersion
            // 
            this.comboBoxMySQLqdbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMySQLqdbVersion.FormattingEnabled = true;
            this.comboBoxMySQLqdbVersion.Location = new System.Drawing.Point(12, 180);
            this.comboBoxMySQLqdbVersion.Name = "comboBoxMySQLqdbVersion";
            this.comboBoxMySQLqdbVersion.Size = new System.Drawing.Size(97, 21);
            this.comboBoxMySQLqdbVersion.TabIndex = 47;
            // 
            // buttonMySQLloadQdb
            // 
            this.buttonMySQLloadQdb.Location = new System.Drawing.Point(111, 179);
            this.buttonMySQLloadQdb.Name = "buttonMySQLloadQdb";
            this.buttonMySQLloadQdb.Size = new System.Drawing.Size(40, 23);
            this.buttonMySQLloadQdb.TabIndex = 48;
            this.buttonMySQLloadQdb.Text = "Load";
            this.buttonMySQLloadQdb.UseVisualStyleBackColor = true;
            this.buttonMySQLloadQdb.Click += new System.EventHandler(this.buttonMySQLloadQdb_Click);
            // 
            // lblVCdbLoadStatus
            // 
            this.lblVCdbLoadStatus.AutoSize = true;
            this.lblVCdbLoadStatus.Location = new System.Drawing.Point(311, 130);
            this.lblVCdbLoadStatus.Name = "lblVCdbLoadStatus";
            this.lblVCdbLoadStatus.Size = new System.Drawing.Size(35, 13);
            this.lblVCdbLoadStatus.TabIndex = 49;
            this.lblVCdbLoadStatus.Text = "label1";
            // 
            // progBarPrimeACESload
            // 
            this.progBarPrimeACESload.Location = new System.Drawing.Point(154, 14);
            this.progBarPrimeACESload.Name = "progBarPrimeACESload";
            this.progBarPrimeACESload.Size = new System.Drawing.Size(150, 18);
            this.progBarPrimeACESload.TabIndex = 50;
            // 
            // progBarRefACESload
            // 
            this.progBarRefACESload.Location = new System.Drawing.Point(154, 42);
            this.progBarRefACESload.Name = "progBarRefACESload";
            this.progBarRefACESload.Size = new System.Drawing.Size(150, 18);
            this.progBarRefACESload.TabIndex = 51;
            // 
            // lblPrimeACESLoadStatus
            // 
            this.lblPrimeACESLoadStatus.AutoSize = true;
            this.lblPrimeACESLoadStatus.Location = new System.Drawing.Point(311, 17);
            this.lblPrimeACESLoadStatus.Name = "lblPrimeACESLoadStatus";
            this.lblPrimeACESLoadStatus.Size = new System.Drawing.Size(35, 13);
            this.lblPrimeACESLoadStatus.TabIndex = 52;
            this.lblPrimeACESLoadStatus.Text = "label1";
            // 
            // lblRefACESLoadStatus
            // 
            this.lblRefACESLoadStatus.AutoSize = true;
            this.lblRefACESLoadStatus.Location = new System.Drawing.Point(311, 45);
            this.lblRefACESLoadStatus.Name = "lblRefACESLoadStatus";
            this.lblRefACESLoadStatus.Size = new System.Drawing.Size(35, 13);
            this.lblRefACESLoadStatus.TabIndex = 53;
            this.lblRefACESLoadStatus.Text = "label1";
            // 
            // dgLogicProblemsDescription
            // 
            this.dgLogicProblemsDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsDescription.HeaderText = "Problem";
            this.dgLogicProblemsDescription.Name = "dgLogicProblemsDescription";
            this.dgLogicProblemsDescription.Width = 70;
            // 
            // dgLogicProblemsGroup
            // 
            this.dgLogicProblemsGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsGroup.HeaderText = "App Group";
            this.dgLogicProblemsGroup.Name = "dgLogicProblemsGroup";
            this.dgLogicProblemsGroup.ReadOnly = true;
            this.dgLogicProblemsGroup.Width = 83;
            // 
            // dgLogicProblemsAppId
            // 
            this.dgLogicProblemsAppId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsAppId.HeaderText = "App id";
            this.dgLogicProblemsAppId.Name = "dgLogicProblemsAppId";
            this.dgLogicProblemsAppId.ReadOnly = true;
            this.dgLogicProblemsAppId.Width = 62;
            // 
            // dgLogicProblemsReference
            // 
            this.dgLogicProblemsReference.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsReference.HeaderText = "Reference";
            this.dgLogicProblemsReference.Name = "dgLogicProblemsReference";
            this.dgLogicProblemsReference.Width = 82;
            // 
            // dgLogicProblemsBaseVehicleId
            // 
            this.dgLogicProblemsBaseVehicleId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsBaseVehicleId.HeaderText = "Base Vehicle id";
            this.dgLogicProblemsBaseVehicleId.Name = "dgLogicProblemsBaseVehicleId";
            this.dgLogicProblemsBaseVehicleId.ReadOnly = true;
            this.dgLogicProblemsBaseVehicleId.Width = 87;
            // 
            // dgLogicProblemsMake
            // 
            this.dgLogicProblemsMake.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsMake.HeaderText = "Make";
            this.dgLogicProblemsMake.Name = "dgLogicProblemsMake";
            this.dgLogicProblemsMake.ReadOnly = true;
            this.dgLogicProblemsMake.Width = 59;
            // 
            // dgLogicProblemsModel
            // 
            this.dgLogicProblemsModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsModel.HeaderText = "Model";
            this.dgLogicProblemsModel.Name = "dgLogicProblemsModel";
            this.dgLogicProblemsModel.ReadOnly = true;
            this.dgLogicProblemsModel.Width = 61;
            // 
            // dgLogicProblemsYear
            // 
            this.dgLogicProblemsYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsYear.HeaderText = "Year";
            this.dgLogicProblemsYear.Name = "dgLogicProblemsYear";
            this.dgLogicProblemsYear.ReadOnly = true;
            this.dgLogicProblemsYear.Width = 54;
            // 
            // dgLogicProblemsPartType
            // 
            this.dgLogicProblemsPartType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsPartType.HeaderText = "Part Type";
            this.dgLogicProblemsPartType.Name = "dgLogicProblemsPartType";
            this.dgLogicProblemsPartType.ReadOnly = true;
            this.dgLogicProblemsPartType.Width = 72;
            // 
            // dgLogicProblemsPosition
            // 
            this.dgLogicProblemsPosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsPosition.HeaderText = "Position";
            this.dgLogicProblemsPosition.Name = "dgLogicProblemsPosition";
            this.dgLogicProblemsPosition.ReadOnly = true;
            this.dgLogicProblemsPosition.Width = 69;
            // 
            // dgLogicProblemsQty
            // 
            this.dgLogicProblemsQty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsQty.HeaderText = "Qty";
            this.dgLogicProblemsQty.Name = "dgLogicProblemsQty";
            this.dgLogicProblemsQty.ReadOnly = true;
            this.dgLogicProblemsQty.Width = 48;
            // 
            // dgLogicProblemsPart
            // 
            this.dgLogicProblemsPart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsPart.HeaderText = "Part";
            this.dgLogicProblemsPart.Name = "dgLogicProblemsPart";
            this.dgLogicProblemsPart.ReadOnly = true;
            this.dgLogicProblemsPart.Width = 51;
            // 
            // dgLogicProblemsFitment
            // 
            this.dgLogicProblemsFitment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgLogicProblemsFitment.HeaderText = "Fitment";
            this.dgLogicProblemsFitment.Name = "dgLogicProblemsFitment";
            this.dgLogicProblemsFitment.ReadOnly = true;
            this.dgLogicProblemsFitment.Width = 66;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1250, 711);
            this.Controls.Add(this.lblRefACESLoadStatus);
            this.Controls.Add(this.lblPrimeACESLoadStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblVCdbLoadStatus);
            this.Controls.Add(this.lblVCdbFilePath);
            this.Controls.Add(this.progBarVCdbload);
            this.Controls.Add(this.btnSelectVCdbFile);
            this.Controls.Add(this.buttonMySQLloadVCdb);
            this.Controls.Add(this.comboBoxMySQLvcdbVersion);
            this.Controls.Add(this.lblNoteTranslationfilePath);
            this.Controls.Add(this.btnSelectNoteTranslationFile);
            this.Controls.Add(this.lblQdbFilePath);
            this.Controls.Add(this.btnSelectQdbFile);
            this.Controls.Add(this.lblinterchangefilePath);
            this.Controls.Add(this.btnSelectPartInterchange);
            this.Controls.Add(this.lblReferenceACESfilePath);
            this.Controls.Add(this.btnSelectReferenceACESfile);
            this.Controls.Add(this.lblAppVersion);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.lblPCdbFilePath);
            this.Controls.Add(this.btnSelectPCdbFile);
            this.Controls.Add(this.lblACESfilePath);
            this.Controls.Add(this.btnSelectACESfile);
            this.Controls.Add(this.comboBoxMySQLpcdbVersion);
            this.Controls.Add(this.buttonMySQLloadPCdb);
            this.Controls.Add(this.comboBoxMySQLqdbVersion);
            this.Controls.Add(this.buttonMySQLloadQdb);
            this.Controls.Add(this.progBarPrimeACESload);
            this.Controls.Add(this.progBarRefACESload);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 680);
            this.Name = "Form1";
            this.Text = " ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgParts)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageStats.ResumeLayout(false);
            this.tabPageStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCommonErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogicProblems)).EndInit();
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            this.groupBoxRemoteVCdb.ResumeLayout(false);
            this.groupBoxRemoteVCdb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreads)).EndInit();
            this.groupBoxValidateTagOptions.ResumeLayout(false);
            this.groupBoxValidateTagOptions.PerformLayout();
            this.groupBoxQuantityOutlierSettings.ResumeLayout(false);
            this.groupBoxQuantityOutlierSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQtyOutliersThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQtyOutliersSample)).EndInit();
            this.groupBoxFitmentLogicSettings.ResumeLayout(false);
            this.groupBoxFitmentLogicSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTreeConfigLimit)).EndInit();
            this.tabPageExports.ResumeLayout(false);
            this.tabPageExports.PerformLayout();
            this.tabPageParts.ResumeLayout(false);
            this.tabPageParts.PerformLayout();
            this.tabPagePartsMultiTypes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgParttypeDisagreement)).EndInit();
            this.tabPageParttypePosition.ResumeLayout(false);
            this.tabPageParttypePosition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgParttypePosition)).EndInit();
            this.tabPageQdbErrors.ResumeLayout(false);
            this.tabPageQdbErrors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgQdbErrors)).EndInit();
            this.tabPageInvalidBasevids.ResumeLayout(false);
            this.tabPageInvalidBasevids.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgBasevids)).EndInit();
            this.tabPageInvalidVCdbCodes.ResumeLayout(false);
            this.tabPageInvalidVCdbCodes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgVCdbCodes)).EndInit();
            this.tabPageInvalidConfigs.ResumeLayout(false);
            this.tabPageInvalidConfigs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgVCdbConfigs)).EndInit();
            this.tabPageAddsDropsParts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAddsDropsParts)).EndInit();
            this.tabPageAddsDropsVehicles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgAddsDropsVehicles)).EndInit();
            this.tabPageQuantityWarnings.ResumeLayout(false);
            this.tabPageQuantityWarnings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgQuantityWarnings)).EndInit();
            this.tabPageFitmentLogic.ResumeLayout(false);
            this.splitContainerFitmentLogic.Panel1.ResumeLayout(false);
            this.splitContainerFitmentLogic.Panel1.PerformLayout();
            this.splitContainerFitmentLogic.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFitmentLogic)).EndInit();
            this.splitContainerFitmentLogic.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFitmentLogicProblems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxFitmentTree)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSelectACESfile;
        private System.Windows.Forms.Label lblACESfilePath;
        private System.Windows.Forms.Button btnSelectVCdbFile;
        private System.Windows.Forms.Label lblVCdbFilePath;
        private System.Windows.Forms.Button btnSelectPCdbFile;
        private System.Windows.Forms.Label lblPCdbFilePath;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.DataGridView dgParts;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageParts;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TabPage tabPageExports;
        private System.Windows.Forms.TabPage tabPageInvalidBasevids;
        private System.Windows.Forms.TabPage tabPageInvalidVCdbCodes;
        private System.Windows.Forms.TabPage tabPageInvalidConfigs;
        private System.Windows.Forms.DataGridView dgBasevids;
        private System.Windows.Forms.TabPage tabPageStats;
        private System.Windows.Forms.Label lblStatsPartsCount;
        private System.Windows.Forms.Label lblStatsAppsCount;
        private System.Windows.Forms.Label lblStatsQdbVersion;
        private System.Windows.Forms.Label lblStatsPCdbVersion;
        private System.Windows.Forms.Label lblStatsVCdbVersion;
        private System.Windows.Forms.Label lblStatsACESversion;
        private System.Windows.Forms.Label lblStatsTitle;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgVCdbCodes;
        private System.Windows.Forms.DataGridView dgVCdbConfigs;
        private System.Windows.Forms.Label lblMacroProblems;
        private System.Windows.Forms.Label lblMacroProblemsTitle;
        private System.Windows.Forms.Label lblAppVersion;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabPageParttypePosition;
        private System.Windows.Forms.DataGridView dgParttypePosition;
        private System.Windows.Forms.PictureBox pictureBoxLogicProblems;
        private System.Windows.Forms.TabPage tabPagePartsMultiTypes;
        private System.Windows.Forms.DataGridView dgParttypeDisagreement;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgPartsPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgPartsAppCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgPartsParttypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgPartsPositions;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgParttypeDisagreementPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgParttypeDisagreementParttypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesApplicationid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesMake;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesParttype;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesQualifiers;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbCodesNotes;
        private System.Windows.Forms.Button btnSelectReferenceACESfile;
        private System.Windows.Forms.Label lblReferenceACESfilePath;
        private System.Windows.Forms.ProgressBar progressBarDifferentials;
        private System.Windows.Forms.Label lblDifferentialsSummary;
        private System.Windows.Forms.Label lblDifferentialsLabel;
        private System.Windows.Forms.TabPage tabPageAddsDropsParts;
        private System.Windows.Forms.DataGridView dgAddsDropsParts;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsPartsAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxPart;
        private System.Windows.Forms.TabPage tabPageAddsDropsVehicles;
        private System.Windows.Forms.DataGridView dgAddsDropsVehicles;
        private System.Windows.Forms.Button btnNetChangeExportSave;
        private System.Windows.Forms.Button btnSelectPartInterchange;
        private System.Windows.Forms.Label lblinterchangefilePath;
        private System.Windows.Forms.Button btnHolesExportSave;
        private System.Windows.Forms.Button btnExportRelatedParts;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox comboBoxRelatedTypesRight;
        private System.Windows.Forms.ComboBox comboBoxRelatedTypesLeft;
        private System.Windows.Forms.CheckBox checkBoxRelatedPartsUseAttributes;
        private System.Windows.Forms.CheckBox checkBoxRelatedPartsUsePosition;
        private System.Windows.Forms.CheckBox checkBoxRelatedPartsUseNotes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxExportDelimiter;
        private System.Windows.Forms.Button btnAppExportSave;
        private System.Windows.Forms.Button btnBgExportSave;
        private System.Windows.Forms.Button btnSelectQdbFile;
        private System.Windows.Forms.Label lblQdbFilePath;
        private System.Windows.Forms.TabPage tabPageQdbErrors;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgBasevidsApplicationid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgBasevidsBasevid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgBasevidsParttype;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgBasevidsPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgBasevidsQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgBasevidsPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgBasevidsQualifiers;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesBaseVid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesMake;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesParttype;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesQualifiers;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgAddsDropsVehiclesMfrLabel;
        private System.Windows.Forms.DataGridView dgQdbErrors;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsApplicationid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsBasevehicleid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsMake;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsParttype;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsVCdbAttributes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsQdbQualifiers;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgVCdbConfigsNotes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumnError;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxBasevehicleid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.Label lblIndividualErrors;
        private System.Windows.Forms.Label lblIndividualErrorsTitle;
        private System.Windows.Forms.TabPage tabPageQuantityWarnings;
        private System.Windows.Forms.DataGridView dgQuantityWarnings;
        private System.Windows.Forms.PictureBox pictureBoxCommonErrors;
        private System.Windows.Forms.Button btnExportPrimaryACES;
        private System.Windows.Forms.CheckBox checkBoxEncipherExport;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn16;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn17;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn18;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn19;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn21;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn20;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn201;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn22;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.GroupBox groupBoxFitmentLogicSettings;
        private System.Windows.Forms.CheckBox checkBoxAnonymizeErrorsACES;
        private System.Windows.Forms.Button btnExportConfigerrorsACES;
        private System.Windows.Forms.CheckBox checkBoxRespectValidateTag;
        private System.Windows.Forms.CheckBox checkBoxQtyOutliers;
        private System.Windows.Forms.CheckBox checkBoxAssetsAsFitment;
        private System.Windows.Forms.CheckBox checkBoxExplodeNotes;
        private System.Windows.Forms.Label lblCachePath;
        private System.Windows.Forms.Button btnSelectCacheDir;
        private System.Windows.Forms.TabPage tabPageFitmentLogic;
        private System.Windows.Forms.DataGridView dgFitmentLogicProblems;
        private System.Windows.Forms.PictureBox pictureBoxFitmentTree;
        private System.Windows.Forms.SplitContainer splitContainerFitmentLogic;
        private System.Windows.Forms.GroupBox groupBoxQuantityOutlierSettings;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown numericUpDownQtyOutliersThreshold;
        private System.Windows.Forms.NumericUpDown numericUpDownQtyOutliersSample;
        private System.Windows.Forms.ListBox listBoxFitmentLogicElements;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.NumericUpDown numericUpDownTreeConfigLimit;
        private System.Windows.Forms.CheckBox checkBoxConcernForDisparate;
        private System.Windows.Forms.GroupBox groupBoxValidateTagOptions;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.CheckBox checkBoxReportAllAppsInProblemGroup;
        private System.Windows.Forms.CheckBox checkBoxLimitDataGridRows;
        private System.Windows.Forms.CheckBox checkBoxAutoloadLocalDatabases;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionError;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionAppId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionBasevid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionMake;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionParttype;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewParttypePositionQualifiers;
        private System.Windows.Forms.Label lblStatsProcessingTime;
        private System.Windows.Forms.Label lblProcessTimeTitle;
        private System.Windows.Forms.Label lblPartsTabRedirect;
        private System.Windows.Forms.Label lblFitmentLogicProblemsTabRedirect;
        private System.Windows.Forms.Label lblParttypePositionRedirect;
        private System.Windows.Forms.Label lblQtyWarningsRedirect;
        private System.Windows.Forms.Label lblVCdbConfigErrorRedirect;
        private System.Windows.Forms.Label lblInvalidVCdbCodesRedirect;
        private System.Windows.Forms.Label lblInvalidBasevehiclesRedirect;
        private System.Windows.Forms.Button btnSelectNoteTranslationFile;
        private System.Windows.Forms.Label lblNoteTranslationfilePath;
        private System.Windows.Forms.Timer timerHistoryUpdate;
        private System.Windows.Forms.TextBox textBoxAnalysisHostory;
        private System.Windows.Forms.Label lblQdbErrorsRedirect;
        private System.Windows.Forms.CheckBox checkBoxUKgrace;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDownThreads;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblAssessmentsPath;
        private System.Windows.Forms.Button btnSelectAssessmentDir;
        private System.Windows.Forms.GroupBox groupBoxRemoteVCdb;
        private System.Windows.Forms.Button buttonMySQLConnect;
        private System.Windows.Forms.TextBox textBoxMySQLpassword;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxMySQLuser;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxMySQLhost;
        private System.Windows.Forms.ComboBox comboBoxMySQLvcdbVersion;
        private System.Windows.Forms.Button buttonMySQLloadVCdb;
        private System.Windows.Forms.RadioButton radioButtonDataSourceMySQL;
        private System.Windows.Forms.RadioButton radioButtonDataSourceAccess;
        private System.Windows.Forms.ProgressBar progBarVCdbload;
        private System.Windows.Forms.ComboBox comboBoxMySQLpcdbVersion;
        private System.Windows.Forms.Button buttonMySQLloadPCdb;
        private System.Windows.Forms.ComboBox comboBoxMySQLqdbVersion;
        private System.Windows.Forms.Button buttonMySQLloadQdb;
        private System.Windows.Forms.Label lblVCdbLoadStatus;
        private System.Windows.Forms.ProgressBar progBarPrimeACESload;
        private System.Windows.Forms.ProgressBar progBarRefACESload;
        private System.Windows.Forms.Label lblPrimeACESLoadStatus;
        private System.Windows.Forms.Label lblRefACESLoadStatus;
        private System.Windows.Forms.ProgressBar progBarExportBuyersGuide;
        private System.Windows.Forms.ProgressBar progBarExportFlatApps;
        private System.Windows.Forms.ProgressBar progBarExportRelatedParts;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsAppId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsReference;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsBaseVehicleId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsMake;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsPartType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsPosition;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsPart;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgLogicProblemsFitment;
    }
}

