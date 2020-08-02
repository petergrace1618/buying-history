# buying-history
I used to have a meager collection of cassette tapes when I was a teenager, but I sold most of them during a turning point in my life. 
I started buying cassette tapes on eBay and Amazon in 2006. I kept track of my purchases in a text file with a format of my own making.
```
_gnasher670:Testament{"Practice What You Preach","The New Order"}:2.37:7.27:1/3/07
millimidian:King Diamond{"Them"}:3.50:6.45:1/16/07
tragedian1:Slayer{"Reign In Blood":7.50},Trouble{"The Skull":2.99},
Xentrix{"Shattered Existence":7.16}::21.65:1/19/07
mmusic:Oingo Boingo{"Best Of Boingo":4.99,"Boingo Alive":5.99},
Sacrilege{"Within The Prophecy":5.99},Holy Moses{"Queen Of Siam":5.99},
Nevermore{"Nevermore":4.99}::33.45:4/24/08
```
Not exactly reader-friendly.  
In 2008 while I was going to PCC I decided to convert my precious gobbledygook into XML. My first idea was to make a sed script to convert it, but then I realized it would take several passes. I happened to be taking a course in Java at the time so I decided to use Java to do it (unfortunately I don't have the source code anymore). I wrote out the program in pseudocode first before I wrote the code. It worked beautifully. 
```xml
<buyinghistory>
  <sale seller="_gnasher670" subtotal="2.37" total="7.27" date="2007-01-03">
    <album>
      <band>Testament</band>
      <title>Practice What You Preach</title>
    </album>
    <album>
      <band>Testament</band>
      <title>The New Order</title>
    </album>
  </sale>
  <sale seller="millimidian" subtotal="3.50" total="6.45" date="2007-01-16">
    <album>
      <band>King Diamond</band>
      <title>Them</title>
    </album>
  </sale>
  <sale seller="tragedian1" subtotal="" total="21.65" date="2007-01-19">
    <album price="7.50">
      <band>Slayer</band>
      <title>Reign In Blood</title>
    </album>
    <album price="2.99">
      <band>Trouble</band>
      <title>The Skull</title>
    </album>
    <album price="7.16">
      <band>Xentrix</band>
      <title>Shattered Existence</title>
    </album>
  </sale>
  <sale seller="mmusic" subtotal="" total="33.45" date="2008-04-24">
    <album price="4.99">
      <band>Oingo Boingo</band>
      <title>Best Of Boingo</title>
    </album>
    <album price="5.99">
      <band>Oingo Boingo</band>
      <title>Boingo Alive</title>
    </album>
    <album price="5.99">
      <band>Sacrilege</band>
      <title>Within The Prophecy</title>
    </album>
    <album price="5.99">
      <band>Holy Moses</band>
      <title>Queen Of Siam</title>
    </album>
    <album price="4.99">
      <band>Nevermore</band>
      <title>Nevermore</title>
    </album>
  </sale>
</buyinghistory>
```
But apparently I wasn't satisfied with that, so I made a couple sed scripts to do the same thing.  
This one.
```sed
# convertbh.sed1 
#
# Usage: sed -f convertbh.sed bh.old
#
# A sed script that converts Buyinghistory.txt into XML. (Before applying
# the script, album titles in the original file were surrounded by double
# quotes using VI to facilitate processing.) All data in the file are saved as
# element content, i.e. no attributes are used.

## BEGIN ##

# First line of file is comment, so have to insert before first line is deleted
1i<buyinghistory>

# delete comments
/^#/d

# Extract initial tags, and put all album info at beginning of line, so later
# when the pattern space is appended to the hold space the unprocessed text
# will be at start of line and can be easily erased.
s!\([^:]*\):\(.*\):\([^:]*\):\([^:]*\):\([^:]*\)$!\2<buyer>\1</buyer>\
<subtotal>\3</subtotal>\
<total>\4</total>\
<date>\5</date>\
<items>!

# save to hold space and isolate album info
h 
s!<.*>$!!

# xml-ize album price
s!":\([^,}]*\)\([,}]\)!"<price>\1</price>\2!g

# if no price, add empty price tag
s!"\([,}]\)!"<price></price>\1!g

# xml-ize album title
s!"\([^"]*\)"!<title>\1</title>!g

# put all albums by same band on separate line
s!},!\n!g

# insert newline at beginning of line to facilitate processing album info
s!^!\n!

# xml-ize band name
s!\n\([^{]*\){!\n<band>\1</band>!g

# insert band tag for multiple albums by same band
:loop
s!\(<band>[^>]*>\)\(.*\),<ti!\1\2,\1<ti!
t loop

# put each album on separate line and get rid of closing brace
s!>,!>\n!g
s!}$!!

# add album tag
s!\n<ba!\n<album><ba!g
s!ce>\n!ce></album>\n!g
s!ce>$!ce></album>!

# get rid of extraneous newline, and put all tags on separate line, but keep
# empty price tags together
s!^\n!!
s!><!>\n<!g
s!\n</p!</p!g

# append hold space and delete unprocessed text
H
g
s!^[^<]*\(<.*\)!\1!

# Insert outer tags (each line in old file represents one sale).
i\
  <sale>
a\
    </items>\
  </sale>
$a\
</buyinghistory>

# debug test
#s!^!#SOL#!
#s!$!#EOL#!
#b

# add indenting
 s!<buye!    &!g
 s!<subt!    &!g
 s!<tota!    &!g
  s!<dat!    &!g
 s!<item!    &!g
s!</item!    &!g
 s!<albu!      &!g
s!</albu!      &!g
  s!<ban!        &!g
 s!<titl!        &!g
 s!<pric!        &!g

## END ##
```
And this one.
```sed
# convertbh.sed2
#
# Usage: sed -f convertbh.sed bh.old
#
# A sed script that converts Buyinghistory.txt into XML. 
# New in this version:
# seller, subtotal, total, and date information are stored as attributes of the
# sale element, and album price is stored as attribute of album element--the
# items element is removed.  An xml declaration is also inserted at beginning
# of file.

## BEGIN ##

# First line of file is comment, so have to insert before first line is deleted
1i\
<?xml version="1.0" encoding="US-ASCII" standalone="yes" ?>\
<!DOCTYPE buyinghistory [\
	<!ELEMENT buyinghistory (sale*)>\
	<!ELEMENT sale (album*)>\
	<!ELEMENT album (band, title)>\
	<!ELEMENT band (#PCDATA)>\
	<!ELEMENT title (#PCDATA)>\
	<!ATTLIST sale seller CDATA #REQUIRED\
				   subtotal CDATA #REQUIRED\
				   total CDATA #REQUIRED\
				   date CDATA #REQUIRED\
	>\
	<!ATTLIST album price CDATA #IMPLIED>\
]>\
<?xml-stylesheet type="text/xsl" href="bh.xsl" ?>\
<buyinghistory>

# delete comments
/^#/d

# Extract initial tags, and put all album info at beginning of line, so later
# when the pattern space is appended to the hold space the unprocessed text
# will be at start of line and can be easily erased.
s!\([^:]*\):\(.*\):\([^:]*\):\([^:]*\):\([^:]*\)$!\2<sale seller="\1" subtotal="\3" total="\4" date="\5">!

# convert date from M/D/YY to YYYY-MM-DD, save to hold space and isolate
# album info
s!\([0-9][0-9]\)/\([0-9][0-9]\)/\([0-9][0-9]\)!20\3-\1-\2!
s!\([0-9][0-9]\)/\([0-9]\)/\([0-9][0-9]\)!20\3-\1-0\2!
s!\([0-9]\)/\([0-9][0-9]\)/\([0-9][0-9]\)!20\3-0\1-\2!
s!\([0-9]\)/\([0-9]\)/\([0-9][0-9]\)!20\3-0\1-0\2!
h 
s!<.*>$!!

# xml-ize album price. # is added as a place holder to facilitate converting
# the price element into an album attribute later.
s!":\([^,}]*\)\([,}]\)!"#<price>\1</price>\2!g

# if no price, add empty price tag
s!"\([,}]\)!"#<price></price>\1!g

# xml-ize album title
s!"\([^"]*\)"!<title>\1</title>!g

# put all albums by same band on separate line
s!},!\n!g

# insert newline at beginning of line to facilitate processing album info
s!^!\n!

# xml-ize band name. @ added as place holder to facilitate next step. Necessary
# when there are multiple albums in a sale AND multiple albums by same bands.
s!\n\([^{]*\){!\n@<band>\1</band>!g

# insert band tag for multiple albums by same band, and delete @ place holder
:loop
s!\(<band>[^>]*>\)\([^@]*\),<ti!\1\2,\1<ti!
t loop
s!@!!g

# put each album on separate line and get rid of closing brace
s!>,!>\n!g
s!}$!!

# add album tag
s!\n<ba!\n<album><ba!g
s!ce>\n!ce></album>\n!g
s!ce>$!ce></album>!

# move price from being its own element to being attribute of album element,
# and delete empty price attributes
s!<album>\([^#]*\)#<price>\([^<]*\)</price>!<album price="\2">\1!g
s! price=""!!g

# get rid of extraneous newline, and put all tags on separate line
s!^\n!!
s!><!>\n<!g

# append to hold space, retrieve new hold space, and delete unprocessed text
H
g
s!^[^<]*\(<.*\)!\1!

# Insert ending tags (each line in old file represents one sale). Indenting must
# be added here because the inserted/appended text does not become part of the
# pattern space.
a\
  </sale>
$a\
</buyinghistory>

# add indenting
s!<sal!  &!g
s!<alb!    &!g
s!</al!    &!g
s!<ban!      &!g
s!<tit!      &!g

## END ##
```
I even had a makefile to simplify the process.
```make
bh.xml : bh.old convertbh.sed
	sed -f convertbh.sed bh.old > bh.xml

bh.old : Buyinghistory.txt
	cp Buyinghistory.txt bh.old
```
Then at a certain point I realized a couple things. I wasn't keeping track of the "store" (eBay or Aamazon, etc.), some sales contained multiple tapes in one item, and once in a while I bought a CD. In other words, my schema was inadequate.
At some point I picked up a book about XSLT at a Goodwill and held onto it because I thought it might be useful. Then around the time the Mayans allegedly predicted that the end of the world, I dusted it off, and with a little research into PHP, I converted my XML into HTML.  

![Peter Grace's Tape Buying History](buyinghistory-scrshot-1.png)

I still had to update my XML manually, and because XML is so verbose the upkeep of entering new sales became tiresome, so I wrote a bash script to update my XML file, or bh.xml as I called it. My pride and joy. Unfortunately, I don't have those scripts anymore.  
My goal the whole time was to put it online and have a webpage to add sales. 
![Add Sale to Buying History](buyinghistory-addsale.png)
I tried to use Javascript, but my knowledge of Javascript was limited to an Introduction to Computer Science class I took in 2005. I wasn't even aware of jQuery at the time. This was 2013. For years I kept buying tapes and updating bh.xml manually (utilizing my bash script, of course), until July of 2020 when I was given the opportunity to create a console application in C# using Entity Framework Code First. It should come as no surprise that my first thought was to bring my years-long goal to fruition. And I did.  


I realize that it's purely a vanity piece of software but it means a lot to me.


