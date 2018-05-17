# ACESinspector
C# Windows Forms app for validating automotive catalog ACES xml file content

You can download the latest stable compiled version at
autopartsource.com/ACESinspector



# Changelog

1.2.0.12 (released 5/17/2018)
Added PowerOutput support (yes - it was missing since day one)
Write registry values for MySQL server upon "leave" events on host, user, password

1.2.0.10 (released 4/28/2018)
Added multi-thread support
Added mysql remote database support with baked-in MySQL client DLL
Added support for year-range-style apps

1.0.2.1 (released 12/5?/2017)
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
