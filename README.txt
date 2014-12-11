FAT - File Analyzer Tool
-----------------------------------------------------------------

Analyzes files and folder structure used in the European Newspaper project (http://www.europeana-newspapers.eu/).

INSTALLATION:
- Compile program and start using 'FAT.exe'
- Make sure a masterlist "Masterlist.xlsx" is in the same folder
	The original masterlist is not included in the opensource package as it contains confidential information
	An example of a masterlist (Masterlist.xlsx.example) is included in this package however
	Two columns of the masterlist are used in the program:
		UID: determines the valid values of newspaper foldernames
		TELPresentation: if either set to '1' or '2' viewing files must be present in a subfolder called "viewing_files"
- If the application does not start you maybe have to download and install the Office 2007 system drivers: http://www.microsoft.com/en-us/download/confirmation.aspx?id=23734

USAGE:
- 1st step: folder structure check: specify the input folder, ie the folder with the library name, by clicking on the '...' button;
	If you ever want to reload the current folder press 'reload directory' (however this also clears all your current progress!)
	At this stage the directory structure is parsed and validated: if an error occurs you will see corresponding error messages in the error log tab
	and you won't be able to start filechecking or editing metadata
	If your folder structure does not contain folder on year level, uncheck the 'has year level' checkbox!
- 2nd step: filechecking: start the filecheck process by clicking 'start file check'
	If an error occurs at this stage it appears in the error log tab. You can start and stop the filechecking process at any time without loosing
	your current progress.
	If all files are checked with no error a success message appears beside 'File check status'
- 3rd step: editing metadata: by selecting an item in the 'folder structure' window you can edit its metadata in the 'Metadata Editor' tab
	Any information applied at a higher directory level will be applied to all its subdirectories
	Use button 'Set metadata' if you just want to apply the selected metadata to the selected element
	Use button 'Set metadata and populate down' if you want to apply the metadata to the selected element and populate it down in the list on this hierarchy level
	If an issue folder contains metadata it appears green, if it contains some metadata it appeary yellow, if it contains no metadata it appears red
	If all issue folders contain metadata a green success message appears beside 'Metadata status'
- 4th step: if both, filecheck and metadata, is completed successfully, save the XML file by clicking 'Save XML file...'
- At any point after the 1st step you can load/save your results using the load/save xml file buttons
- Loading an XML additionally performs a sync on issue level: files that were removed from the issue folder (eg because they were invalid) are are not loaded into the tool
  and new image files are added to the tool if they were not there before

DELIVERY:
- Library input folder and(!) the saved XML file!


