# DirectoryCompressor
Small and simple app to compress subdirectories in given directory using [7zip][1] (for now... Opened for other compressors too). Needed such for backups, can be added to scheduler for time scheduled usage. You must have 7Zip installed to use the app.    

**Example of usage:**    
You have logs in the directory *d:\Logs* that you want to compress and remove the files/folders, that are older then the value set in '*lastmodifieddays*' (see *app.config*). For that you just need to call the program with arguments: `path-to-exe\DirectoryCompressor.exe -s d:\Logs -a 1` ; where -s means "source directory", -a "auto delete" = 1 (enabled). By default the archived files will be saved in the source directory.    
![image](https://user-images.githubusercontent.com/1378866/151960175-1421c1e3-cc06-424e-b577-18a6a58433ec.png)

[1]:http://www.7-zip.org
