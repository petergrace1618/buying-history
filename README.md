# buying-history

## Phase 0
As a teenager I had a small collection of cassette tapes, but I sold them when I was 20 at a turning point in my life. Ten years later in 2006, I felt compelled for various reasons to rekindle my youthful pastime. As it was now the 21st Century and cassette tapes were nearly a forgotten relic, I started collecting on eBay and Amazon.

## Phase 1
I kept track of my purchases in a text file using a format of my own devising. Then in 2008, while I was going to PCC for Computer Science, I wrote a one-off Java program to convert [my precious gobbledygook][2] into XML. I even wrote the program in pseudocode first in proper academic fashion. Unfortunately, I have neither the Java nor the pseudocode anymore, but the program worked beautifully. 

In an Introduction to Unix class, I discovered `sed`, the text processing utility, and I immediately fell in love with its arcane syntax. So as a programming exercise I wrote [a sed script][3] to convert my original text file to [XML][4]. To test my conversion script, I wrote several more smaller scripts ([newbuyers.sed](legacy_files/newbuyers.sed), [oldbuyers.sed](legacy_files/oldbuyers.sed), [newtitles.sed](legacy_files/newtitles.sed), [oldtitles.sed](legacy_files/oldtitles.sed)) which extracted corresponding data from the old and new files. I then compared these two sets of data using `diff` on the command line. Later, I decided to use attributes for some of the data and wrote [another sed script][5] to convert it to the [new format][6].

To avoid the potential risks of having to manually enter raw XML in a text editor, I continued tracking my purchases in my original format and used my conversion script to update the XML after every purchase. I even had a [Makefile][7] to automate the process. All the while, I had a feeling that my XML schema may not have been as robust as it could be, but I didn't give it much thought as I was too consumed with the vagaries and vicissitudes of everyday life. Then one day something unexpected happened and I was confronted with a stark new reality.

On January 9 2010, I broke my schema.

Explanation: some tapes are sold as a *lot* meaning multiple tapes sold as one item. To account for this, I used a `price` attribute on the `<album>` element for tapes sold individually and a `subtotal` attribute on the `<sale>` element for tapes sold as a lot. This had been adequate until the day I decided to buy a single tape ***and*** a lot from the same seller.  And that was that.   
My Weltanschauung went kaput.

It was time to refactor my XML.

## Phase 2
It was really a simple solution, melodrama notwithstanding.   
- I wrapped each individual album and lot in an `<item>` tag. 
- I moved `price` from an attribute of `<album>` to a child element of `<item>` thus removing the redundant subtotal attribute. 
- And I dispensed with using attributes altogether and moved everything into its own element to facilitate future processing.

And my [buying history][8] was reborn.

Shortly after this, I bought a [book about XSLT][10] which allowed me to render my XML as HTML. With a web hosting account, an FTP client, and a little PHP, [my obsession][9] was online for the whole world to see. 

My ultimate goal, however, was to create a web application backed by a database to update and display my buying history. 

## Phase 3
In July of 2020 as a student at the Tech Academy, I had the opportunity to create a console application in C# using Entity Framework Code First which I used to fulfill another step towards my years-long dream: to give my buying history a proper home in a SQL Server database. 

The [console app][11] has three features: [add a sale][12] to the database, print the database in [XML][14] or in [human-friendly][13] form, and save the database as an XML file. My favorite part, though, is the Seed() function in [Configuration.cs][15] where I use the `System.Xml` package to populate the database with my buying history file. Glorious! 

User input is obtained through several functions that validate the data and enforce the `NOT NULL` constraint on the models. The `Sale.Seller` field, however, is nullable so I added an optional parameter in `GetString()` that allows an empty string to be returned. In `TryGetDate()` and `TryGetDecimal()` I used a try-catch block in a while loop to ensure that input is in the proper format. I also added data annotations on the model to prevent duplicate records from being added. 

## The Next Phase
The next step is of course to
- Create a web application using MVC Database First,
- Add an admin portal with CRUD functionality, and
- Add user authentication. 

For the sake of simplicity, security, and time, however, I think I can dispense with the admin page and authentication, and instead install my console app on my virtual private server and login over SSH to update the database. Then I can focus on further improvements such as:
- Overhaul the page styling and make it responsive.
- Add album art. For example, add a thumbnail which displays a modal when clicked.
- Link each album and band to its corresponding page on [discogs.com][16].
- Link each seller to their corresponding storefront on eBay or Amazon.
- Add a view to show all albums by a particular band.
- Add a view to show purchase activity and quantity over time or over a given span of time.

In the end, I know it's just a vanity piece of software, but it's my pride and joy.

[2]: <legacy_files/Buyinghistory.txt> "Buyinghistory.txt"
[3]: <legacy_files/convertbh.sed1> "converbh.sed1"
[4]: <legacy_files/bh1.xml> "bh1.xml"
[5]: <legacy_files/convertbh.sed2> "converbh.sed2"
[6]: <legacy_files/bh.xml> "Buyinghistory in XML w/ attributes"
[7]: <legacy_files/Makefile> "Makefile"
[8]: <bh.xml> "bh.xml"
[9]: <https://petergrace.site/buying-history/> "Peter Grace's Tape Buying History"
[10]: <https://www.amazon.com/XSLT-Working-Khun-Yee-Fung/dp/0201711036/> "XSLT: Working with XML and HTML by Khun Yee Fung" 
[11]: <Program.cs> "BuyingHistory/Program.cs"
[12]: <README_files/buyinghistory-scrshot-add.png>
[13]: <README_files/buyinghistory-scrshot-print.png>
[14]: <README_files/buyinghistory-scrshot-print-xml.png>
[15]: <Migrations/Configuration.cs> "Configuration.cs"
[16]: <https://www.discogs.com/> "Discogs - Music Database and Marketplace"
