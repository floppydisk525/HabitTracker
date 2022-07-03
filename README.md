# HabitTracker
HabitTracker project from https://www.thecsharpacademy.com/ and on youtube here:
https://www.youtube.com/watch?v=d1JIJdDVFjs

Couple of changes from the video:  
1. I used Visual Studio 2022 on Windows while the video project was shown with VSCode on a Mac.
2. I created a console application from the command line like the video, but my console application was in .Net 6 and had the new Program.cs file format.  After reading up on it, I simply wrote in the code for the namespace and "static void Main(string[] args)" method like the old template.  That worked fine.  
	- This solved an issue when the video started adding private and internal static classes to the 'new' console application format where  Visual Studio didn't like them.  Once I added the new Main method, everything worked as it should.  
3.  I used DB Browser for SQLite:  https://sqlitebrowser.org/  In the video, Pablo goes to a VSCode add-in SQLite Explorer to view the database, but Visual Studio didn't have that so I used the DB Browser.  Worked great and learned something, too.  
4.  I put the DrinkingWater class in it's own file DrinkingWater.cs.  
5.  I used the US version of the date MM-dd-yy vs. the European dd-MM-yy version.  
