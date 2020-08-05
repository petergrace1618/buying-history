# buying-history  
## Phase 0
As a teenager I had a small collection of cassette tapes, but I sold them when I was 20 at a turning point in my life. Ten years later in 2006, I felt compelled for various reasons to rekindle my youthful past-time. As it was now the 21st Century and cassette tapes were nearly a forgotten relic, I started collecting on eBay and Amazon.

## Phase 1
I kept track of my purchases in a [text file][2] using a format of my own devising. Then in 2008, while I was going to PCC for Computer Science, I wrote a one-off Java program to convert my precious gobbledygook into XML. I even wrote the program in pseudocode first in proper academic fashion. Unfortunately, I have neither the Java nor the pseudocode anymore, but the program worked beautifully. 

In an Introduction to Unix class, I discovered `sed`, the text processing utility, and I immediately fell in love with its arcane syntax. So as a programming exercise I wrote [a sed script][3] to convert my original text file to [XML][4]. To test my conversion script, I wrote several more smaller scripts ([newbuyers.sed](legacy_files/newbuyers.sed), [oldbuyers.sed](legacy_files/oldbuyers.sed), [newtitles.sed](legacy_files/newtitles.sed), [oldtitles.sed](legacy_files/oldtitles.sed)) which extracted corresponding data from the old and new files. I then compared these two sets of data using `diff` on the command line. Later, I decided to use attributes for some of the data and had to write [another sed script][5] to convert it to the [new format][6].

To avoid the potential risks of having to manually enter raw XML in a text editor, I continued tracking my purchases in my original format and used my conversion script to update the XML after every purchase. I even had a [Makefile][7] to automate the process. All the while, I had a feeling that my XML schema may not have been as robust as it could be, but I didn't give it much thought as I was too consumed with the vagaries and vicissitudes of everyday life. Then one day something unexpected happened and I was confronted with a stark new reality.

On January 9 2010, I broke my schema.

You see, some tapes are sold as a *lot* meaning multiple tapes sold as one item. To account for this, I used a `price` attribute on the `<album>` element for tapes sold individually and a `subtotal` attribute on the `<sale>` element for tapes sold as a lot. This had been adequate until one day when I decided to buy a single tape ***and*** a lot from the same seller.  And that was that.   
My Weltanschauung went kaput.

It had to refactor my XML.

## Phase 2
It was really a simple solution, melodrama notwithstanding.   
- I wrapped each individual album and lot in an `<item>` tag. 
- I moved `price` from an attribute of `<album>` to a child element of `<item>` thus removing the redundant subtotal attribute. 
- And I dispensed with using attributes altogether and moved everything into its own element to facilitate future processing.

And my all-new [bh.xml][8] was reborn.

Shortly after this, I bought a [book about XSLT][1] which allowed me to render my XML as HTML. With a web hosting account, an FTP client, and a little PHP [my obsession][9] was online for the whole world to see. My ultimate goal, however, was to create a web application backed by a database to update and display my buying history. 

## Phase 3

In July of 2020 as a student at the Tech Academy, I had the opportunity to create a console application in C# using Entity Framework Code First. 

## Phase 4? 

Create a web application using MVC.


[1]: <https://www.amazon.com/XSLT-Working-Khun-Yee-Fung/dp/0201711036/> "XSLT: Working with XML and HTML by Khun Yee Fung" 
[2]: <legacy_files/Buyinghistory.txt> "Buyinghistory.txt"
[3]: <legacy_files/convertbh.sed1> "converbh.sed1"
[4]: <legacy_files/bh1.xml> "bh1.xml"
[5]: <legacy_files/convertbh.sed2> "converbh.sed2"
[6]: <legacy_files/bh.xml> "Buyinghistory in XML w/ attributes"
[7]: <legacy_files/Makefile> "Makefile"
[8]: <Buyinghistory.xml> "bh.xml"
[9]: <https://petergrace.site/buying-history/> "Peter Grace's Tape Buying History"
[10]: <legacy_files/albumsby> "Print albums by band"
