# ACESinspector
C# Windows Forms app for validating automotive catalog ACES xml file content. It will tell you that your ACES Content is not as clean as you thought - guaranteed.

You can download the latest stable compiled version at
autopartsource.com/ACESinspector or on github as a "release"


# Changelog

1.2.0.60 (8/23/2024)
 - figured out how to code-sign with my new HSM from Sectigo - What a struggle! 0 stars, would not recommend. Post-HSM signing is not for the faint-of-heart. On top of that, I spent $600 for the privilege.

1.2.0.59 (3/13/2024)
 - Fixed MakeID / ModelID mix-up bug on Make/Model/Year-range app expansion - Thanks, Eric Snow (NTN) for finding this one.

1.2.0.58 (2/22/2024)
 - Added VCdb caching to AiFragments directory as 50 tab-delimited text files containing exactly the query recordsets that result from our queries of the Access file. This is to lay the groundwork for consuming the VCdb from cache and avoid the costly OLEDB overhead. Initial testing implied a 4x speed-up reading the cache.

1.2.0.57 (2/5/2024)
 - Added allowance for a year-range app that specifies MakeID but not ModelID (yes, I’ve actually seen someone do it).

1.2.0.56 (1/30/2024)
 - added qdb code listing tab to the assessment output spreadsheet

1.2.0.55 (1/16/2024)
 - Fixed a crash problem in branch analysis that was introduced by allowing multiple instances of the same QdbID on an app. Dave Logan (AutoCare) identified this one.

1.2.0.54 (12/27/2023)
 - Added ability to gracefull consume an app with a missing parttype so that a XSD error could be thrown. Joe Thomas helped identify the need for this fix.

1.2.0.53 (12/18/2023)
 - Added allowance and detection of duplicate Qdb IDs on same app. Dups were being dropped on import before this version. This is somewhat controversial in the community.

1.2.0.52 (10/24/2023)
 - Added "reference" column to qty outliers grid

1.2.0.51 (9/14/2023)
 - Updated the internal version of the 4.2 xsd to "rev 2 11-19-2021"
 - fix spelling of a variable: basevehilceid -> basevehicleid

1.2.0.50 (7/27/2023)
 - Fixed support for App-level brand and subbrand codes. This is to allow for branding to solve overlaps. They are now properties of the "app" object

1.2.0.49 (7/20/2023)
 - Added support for App-level brand and subbrand codes. They get prefixed onto the partnumber like ABCD_EFGH_PARTNUMBER. This is to allow for branding to solve overlaps

1.2.0.48 (2/27/2023)
 - Completely back-tracked on the automation endevor - it was a fool's erand. The multi-threaded nature of the existing was pushing me down a road of hacks and compromises that were just too far.
 - Began dev on a simpler (non-GUI) version called ACESinspectorCLI

1.2.0.47 (2/25/2023)
 - moved analysis actions from button event into "analyze" function so it can be called through automation
 - changed importXML .load() to uese read-only streamreader (set as read-only) instead direct reading of the file. ACES file was remaining locked which was a problem for automation's ability to delete the consumed file

1.2.0.46 (2/24/2023)
 - compile and publish "release" package 

1.2.0.45 (2/17/2023)
 - Updated my IDE to VS2022 and reconnected to github
 - removed junk files from project

1.2.0.45 (10/31/2022)
 - Added error catching on Qdb import to allow a null in the QdbTypeId field (ACA added a "0" record for referencial reasons with a null typeid in the 10/2022 version)

1.2.0.44 (10/12/2022)
 - Fixed version number in AssemblyInfo.cs to 1.2.0.44

1.2.0.43 (10/1/2022)
 - Began groundwork for automated operation
 - Added EngineBlockID support

1.2.0.42 (9/3/2021)
 - Added "Asset Item Order" to flattened export (at the request of Ben B. at Dr1v)
 - Added the imporation of Action="Deleted" apps (at the request of Ben B. at Dr1v) so that they could be exported to the flattened output
 - Added missing Qdb qualifiers in Net-Change export
 - Added XSD schema 4.2 (RC2) to the list
 - Added a feature to ignore non-fitment type Qdbs when evaluating CNC overlaps (at the request of Ben B. at Dr1v). Avoid looking at a "replaced by" or "use until" qualifiers in such a way that a CNC is generated
 - Fixed progres bar hide/un-hide on exports tab
 - Fixed the ignoring of the "qty outliers" checkbox on the settings tab


1.2.0.35 (released 10/21/2020)
- Added "Coded-Values" format option to the "Export Simplified (flat) apps" function. This was to meet an internal need of my own for import of ACES files from trading partners into my PIM system (sandPIM)
- fixed "Treat Assets as Fitment" checkbox to be sticky in registry
- added ACES 4.1 xsd data to schemas
- added basevid to the holes report (requested by Joe @ Autology)

1.2.0.34 (released 1/25/2020)
- Added 2 more levels of depth to logic diagram 

1.2.0.33 (released 12/12/2019)
- Added "Reference" field to VCdb code problems tab (and spreadsheet output).
- Added "BAse Vehicel id" field to VCdb code problems tab (and spreadsheet output).
- Added "Reference" field to flattened export.
- Added AssetName, AssetItemOrder to exported ACES xml file
- Added "Export Asset List" feature to "Exports" tab
- Added logic to respect the checkbox "Treat Assets as Fitment". It was permenantly true before now
- Fixed false positive "Asset Problems"

1.2.0.32 (released 9/12/2019)
- Added new format option to assessment file layout in response to Tenneco's suggestion for CSV-style content that can be consumed into a BI system.
- Fixed "Part adds/drops list is too large to show here" message was showing up in error when vehicle diff list was above threshold
- Added results row to the stats tab of the assessment spreadsheet.
- Fixed net-changed ACES xml export. Had been broken since 1.0.2.1 due to changing from arrays of apps to lists of apps.
- Fixed differential vehilces datagrid - it was not showing any results if results count was above "limit".
- Added automated processing features.
- Added clickability to the fitment tree diagram in the logic problems tab. Clicking a fitment element draws a red line through it and addds it to a hitlist of things to go back and remove from the source PIM dataset.
- Removed brake wear-sensors from the list of typical parttype/qty analysis.
- Fixed loading of Access files was not updating version lables properly - Calvin @ Dr1ve identified this issue
- Upgraded included MySQL client DLLs to latest generally-available version (8.0.17.0).
-switched to from 4.7.1 to 4.8.0 .NET Framework

1.2.0.31 (released 9/13/2018)
- Upgraded included MySQL client DLLs to latest generally-available version (8.0.12.0). Had to add "SslMode=none" to the connect string as a result.
- Started catching bogus years that are implied by year-range-style apps that go beyond (earlier or later) than the valid basevehicles defined in the VCdb.
- tweaked the Qdb utilization score to avoid divide-by-zero.

1.2.0.30 (released 9/7/2018)
- Added row to assessment spreadsheet stats tab to show Qdb utilization score.
- Added decoded Qdb-transformed notes to the distinct notes list in assessment spreasheet.
- Fixed header row to be Frozen and bold/gray bg in assessment spreadsheet notes tab.
- Fix fullFitmentString() function to properly delimit consecutive Qdb elements with a semi-colon.

1.2.0.29 (released 9/5/2018)
- Added distinct note-tag reporting in the output spreadsheet.
- Added Qdb-Transform feature to the note translation mechanism.
- Added Qdb utilization stats on home tab.
- Added "Ignore NA items" control and (back-end execution) to settings tab.
- Moved controls around (cosmetic) on the settings tab.

1.2.0.28 (released 8/20/2018)
- Fixed blank lblAssessmentsPath.Text and lblCachePath.Text logic on first run

1.2.0.27 (released 8/19/2018)
- Fixed creation of AiFragments directory on path selection

1.2.0.26 (released 8/16/2018)
- switched to from 4.5.2 to 4.7.1 .NET Framework

1.2.0.25 (released 8/15/2018)
- cosmetic fix - "Select VCdb" button was tweaked out of place 

1.2.0.24 (released 8/10/2018)
- Fixed asset analysis to correctly look for assets matched to apps. Also limited DNS-based version publishing to 9.0.0.0 as highest version

1.2.0.23 (released 7/31/2018)
- Added a mechanism to lookup the current official released version from public DNS in order to tell the use when a newer version is avail. The 4-octec (like 1.2.0.23) version number is published in DNS at aiversion.autopaersource.com. The 32bit value is not treated as an IP address and never communicated with.
- The value is compared to the local version and the background color of the version label in the top-right corner of the UI is turned red if a higher value is returned from the DNS query.


1.2.0.22 (released 7/21/2018)
- Added basic changelog support via MySQL import. EngineBase history is currently the only data consumed from the log. Deleted (orphaned) EngineBaseID's are lookuped up in the changelog to see what they did translate to in the past.


1.2.0.21 (released 6/28/2018)
- Removed the "Questionable Note" count from the total of individual app errors. Changed the header on Add/Drop vehicles and parts datagrids to say "Add/Drop" instead of "Action".
- Added logic in to show the "too big to display here" message to add/drop parts and vehicles.


1.2.0.20 (released 6/15/2018)
Added "Questionable Note" detection (only outputs to assessment file - not UI). Made user-selected folder for assessment files and cache files apply to both on one being empty


1.2.0.19 (released 6/12/2018)
Fixed a MySQL-specific VCdb-import problem where the reverse-lookup dictionary for converting year-range style apps to legit basevehicle apps was not being populated. This was causing 
year-range apps to not work when MySQL reference source was in use.


1.2.0.18 (released 5/25/2018)
Re-worked the layout of the VCdb Usage stats export


1.2.0.17 (released 5/24/2018)
Added format selection to "flat" export at the request of Joe at Tenneco.


1.2.0.15 (released 5/23/2018)
Added header line to VCdb code stats export file. Fixed cosmetic stuff around progress bar for ACES files loading.


1.2.0.14 (released 5/23/2018)
Added export of VCdb code stats feature


1.2.0.13 (released 5/18/2018)
Added "Problem Description" and "Reference" attribute to dataGrid in FitmentLogicProblems


1.2.0.12 (released 5/17/2018)
Added PowerOutput support (yes - it was missing since day one)

Write registry values for MySQL server upon "leave" events on host, user, password


1.2.0.10 (released 4/28/2018)
Added multi-thread support
Added mysql remote database support with baked-in MySQL client DLL
Added support for year-range-style apps

1.0.2.1 (released 12/5/2017)
Re-worked aces.apps from fixed array of App types to <List<App>>
Changed CNCoverlap detection from simple blank-vs-nonblank fitment to "deep CNC inspection". In other words, all apps within a mmy/parttype/position/mfrlabel have to all have the same 
named VCdb attributes - If enginebase, for example, is used on one app, it must be use across the whole group.
Added "Settings" tab
Added controls to settings tab to select deep or simple CNC logic
Added controls to settings tab to control thresholds for qtyOutlier detection
Added controls to settings tab to auto-analyze uppon successful import

1.0.1.28 (released 11/25/2017)
XML import switched to xmlreader and a custom axiom function (StreamAppElement()) to use less memory and allow for analysis for larger files - we were throwing "out of memory" exceptions 
at about 500k app befor. Now we are able to handle 1M+ app files
Added App Quantity analysis to look for outlier apps with unusual parttype/position/quantity combinations

1.0.1.27 (released 11/24/2017)
Re-worked the way tabs are handled. Only tabs containing meaningful content are shown.
Began the work of including PIES support.

1.0.1.18 (released 5/24/2017)
Fixed assessment output function that produced a corrupt excel file when multiple positions per part are in play. The string containing a list of position id’s was being delimited by comma and subsequently split by tab. Transitioned over to tab.

1.0.1.17 (released 5/18/2017)
Added IComparable method to VCdbAttributes class for sorting attributes within an app to conform to the XSD-mandated sequence. This fixed the exported net-changes ACES xml file to pass xsd validation.

1.0.1.16 ((released 5/16/2017)
Introduced 2-file differential calculation
Added Adds/Drops Parts & Adds/Drops Vehicles tabs
Added export of net-changes 
Introduced registry-resident validation history storage
Included invalid parttypes and positions in Parttypes-Positions tab

1.0.1.15 (released 5/10/2017)
Defaulted position to 0 on import and excluded 0 and 1 (N/A) from parttype/position audit
Changed “mfrid” to “enginemfrid” in the attributeWhereClause() function
Changed “mfrid” to “transmissionmfrid” in the attributeWhereClause() function
