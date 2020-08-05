# buying-history  
As a teenager I had a small collection of Heavy Metal cassette tapes, but I sold them when I was 19 because I thought I had outgrown them. Ten years later, I started collecting cassettes again. This time on eBay and Amazon.

## Phase 1
For several years I kept track of my purchases in a [text file][2] using a format of my own devising. Then in 2008, while I was going to PCC for Computer Science, I wrote a one-off Java program to convert my precious gobbledygook into XML. I even wrote the program in pseudocode first in proper academic fashion. Unfortunately, I have neither the Java nor the pseudocode anymore, but the program worked beautifully. 

Then I discovered the text processing utility `sed` in an Introduction to Unix class. I immediately fell in love with its arcane syntax, so as a programming exercise I wrote [a sed script][3] to convert my original text file to [XML][4]. I also wrote a series of scripts to test my conversion script. ([newbuyers.sed](legacy_files/newbuyers.sed), [oldbuyers.sed](legacy_files/oldbuyers.sed), [newtitles.sed](legacy_files/newtitles.sed), [oldtitles.sed](legacy_files/oldtitles.sed))

I put all the data into elements, but then decided that using attributes looked way cooler with syntax highlighting, so I wrote [another sed script][5]. Here is [the result][6]. It looked great but was 

I had a [Makefile][7] to simplify the process. 



And a few ancillary scripts like this one:
 
- [maketotals.sed](legacy_files/maketotals.sed)
- [printtotals.awk](legacy_files/printtotals.awk)

Which produced such glorious output as this: 

```
$albumsby Black Sabbath
Black Sabbath:
	Black Sabbath
	Black Sabbath (Import)
	Paranoid
	Live At Last (Import)
	Sabbath, Bloody, Sabbath
	Sabotage
	We Sold Our Souls For Rock And Roll
	Technical Ecstasy
	Never Say Die
	Heaven And Hell
	Mob Rules
	Born Again
	Born Again
	Headless Cross
```

This was all done on the command line using Cygwin (a Linux emulator for Windows) and the vi text editor. 

In the back of my mind, though, I had a feeling that my XML schema may not be as robust as it could be, but I didn't give it much thought as I was too consumed with the vagaries and vicissitudes of everyday life. Then one day something unexpected happened and I was confronted with a stark new reality.

On January 9 2010, I broke my schema.

You see, some tapes are sold as a *lot* meaning multiple tapes sold as one item. To account for this, I used a `price` attribute on the `<album>` element for tapes sold individually and a `subtotal` attribute on the `<sale>` element for tapes sold as a lot. This had been adequate until one crisp January afternoon when I decided to buy a single tape ***and*** a lot from the same seller at the same time. And that was that.  
My Weltanschauung went kaput.

It was time to refactor my XML.

## Phase 2

It was really a simple solution, melodrama notwithstanding.   
- I wrapped each individual album and lot in an `<item>` tag. 
- I dispensed with using attributes altogether and moved everything into its own element to make it easier to process with XSLT.
- I moved `price` from an attribute of `<album>` to a child element of `<item>` thus ridding myself of the redundant subtotal attribute. 
- And finally, influenced by the terseness of Linux commands and willfully flaunting XML's inherent verbosity, I abbreviated the name of the root node. (Admittedly, a manifestation of my antinomian tendencies.)

And the all new bh.xml was born.

```xml
<bh>
  <sale>
    <store>eBay</store>
    <seller>_gnasher670</seller>
    <date>2007-01-03</date>
    <total>7.27</total>
    <item>
      <price>2.37</price>
      <album>
        <band>Testament</band>
        <title>Practice What You Preach</title>
        <format>CASSETTE</format>
      </album>
      <album>
        <band>Testament</band>
        <title>The New Order</title>
        <format>CASSETTE</format>
      </album>
    </item>
  </sale>
  ...
</bh>
```
With my Weltanschauung now restored, I moved on to my next goal which was to bring my fetish to the light of day. 

I had the good fortune of happening upon a [book][1] about XSLT at a second hand store which opened up a myriad of possiblities. I got a web hosting account, FileZilla FTP client

[Peter Grace's Tape Buying History](https://petergrace.site/buying-history/)

## Phase 3

In July of 2020 as a student at the Tech Academy, I had the opportunity to create a console application in C# using Entity Framework Code First. It should come as no surprise that I took the opportunity to bring my years-long goal closer to fruition.

Output of `$bh xml`, `$bh print`, and `$bh add`.

![BuyingHistory console app screenshot](buyinghistory-console-scrshot-2.png)

![BuyingHistory console app screenshot](buyinghistory-console-scrshot-3.png)

![BuyingHistory console app screenshot](buyinghistory-console-scrshot-1.png)

## Phase 4? 

I discovered/realized a few things while writing this README. I 
- I want to change the `<store>`, `<seller>`, `<total>`, `<date>`, and `<price>` elements back to being attributes. It looks more sleek, and it's also more colorful with syntax highlighting.
- The console app needs error checking to test for empty strings on input.

- Create a web application using MVC.

I realize that it's purely a vanity piece of software but it's a labor of love.

[1]: <https://www.amazon.com/XSLT-Working-Khun-Yee-Fung/dp/0201711036/> "XSLT: Working with XML and HTML by Khun Yee Fung" 
[2]: <legacy_files/Buyinghistory.txt> "Buyinghistory.txt"
[3]: <legacy_files/convertbh.sed1> "converbh.sed1"
[4]: <legacy_files/bh1.xml> "bh1.xml"
[5]: <legacy_files/convertbh.sed2> "converbh.sed2"
[6]: <legacy_files/bh.xml> "Buyinghistory in XML w/ attributes"
[7]: <legacy_files/Makefile> "Makefile"
