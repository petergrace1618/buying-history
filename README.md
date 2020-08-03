# buying-history  
As a teenager I had a small collection of Heavy Metal cassette tapes, but I sold them when I was 19; I guess I felt I had outgrown them. Then in 2006 when I discovered eBay and Amazon at 30, I started collecting tapes again. I guess I wanted to rekindle my youth. And because I'm fastidious, I kept track of my purchases in a text file with a format of my own devising.

## Phase 1
```
#SELLER:BAND{"ALBUM"[:PRICE][,"ALBUM"[:PRICE]]}[,BAND{"ALBUM"[:PRICE][,"ALBUM"[:PRICE]]}]:[SUBTOTAL]:TOTAL:DATE
_gnasher670:Testament{"Practice What You Preach","The New Order"}:2.37:7.27:1/3/07
tragedian1:Slayer{"Reign In Blood":7.50},Trouble{"The Skull":2.99},Xentrix{"Shattered Existence":7.16}::21.65:1/19/07
thepollies:Exciter{"Heavy Metal Maniac","Long Live the Loud","Exciter"},Mercyful Fate{"In the Shadows":1.99}::9.73:1/9/07
roundflat:Entombed{"Clandestine"}:1.52:5.52:1/9/07
roger2095:Mercyless{"Abject Offering"}:4.99:6.74:1/11/07
bigmustafa:Helloween{"Judas":3.00},Pantera{"Vulgar Display Of Power":3.00},Testment{"The Legacy":3.00},King Diamond{"Fatal Portrait":3.00}::16.50:1/12/07
millimidian:King Diamond{"Them"}:3.50:6.45:1/16/07
mmusic:Oingo Boingo{"Best Of Boingo":4.99,"Boingo Alive":5.99},Sacrilege{"Within The Prophecy":5.99},Holy Moses{"Queen Of Siam":5.99},Nevermore{"Nevermore":4.99}::33.45:4/24/08
...
```

Not exactly reader-friendly.  
Then in 2008 while I was going to PCC for Computer Science I decided to convert my precious gobbledygook into XML. I did it in Java because I was taking a Java course at the time. I even wrote it in pseudocode first in proper academic fashion. Unfortunately I have neither the Java nor the pseudocode anymore, but it worked beautifully. 

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
     <album price="5.99"
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
  ...
</buyinghistory>
```

Because XML is so verbose I continued tracking my purchases in my own text format and used a sed script to convert it to XML.

```sed
# convertbh.sed
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

I had a makefile to simplify the process. 

```make
bh.xml : bh.old convertbh.sed
	sed -f convertbh.sed bh.old > bh.xml

bh.old : Buyinghistory.txt
	cp Buyinghistory.txt bh.old
```

I wrote some test scripts. 

```sed
#n
# newbuyers.sed
# Extracts the buyer name from bh.xml which was produced by convertbh.sed, and
# writes it to buyers.new.  A similar script (oldbuyers.sed) is also run on the
# bh.old which outputs a list of buyers to buyers.old. The two output files are
# then compared with diff in order to test the results of convertbh.sed.

/buyer/{
s/ *<buyer>\([^<]*\).*/\1/
w buyers.new
}
```

```sed
#n
# oldbuyers.sed
# Extracts the buyer name from bh.old and writes it to buyer.old.  A similar
# script (newbuyers.sed) is also run on the bh.xml which outputs a list of
# buyers to buyers.new. The two output files are then compared with diff
# in order to test the results of convertbh.sed.

/^#/d
s/^\([^:]*\).*/\1/
w buyers.old
```

```sed
#n
# newtitles.sed
# Like newbuyers.sed but extracts titles instead of buyers.

/title/{
s/ *<title>\([^<]*\).*/\1/
w titles.new
}
```

```sed
#n
# oldtitles.sed
# Like oldbuyers.sed but extracts titles instead of buyers.

# delete comments
/^#/d

# put a newline after every title
s!"[^"]*"!&\n!g

# put a newline at start of line to facilitate processing
s!^!\n!

# delete everything before title on the line
s!\n[^"]*!\n!g

# delete newline at SOL and EOL, and delete quotes
s!^\n!!
s!\n$!!
s!"!!g

w titles.old
```

And some ancillary scripts like this one:

```
# albumsby
#
# Usage: albumsby band
#
# Displays all the albums by band specified on command line

if [ $# -eq 0 ]; then
	echo Usage: albumsby BAND
	echo 
	echo Searches bh.xml and displays all albums by BAND specified on command line.
	exit
fi	

if ! grep -qi "<band>$*<" bh.xml; then 
	echo $* not found
	exit 1
fi	

band=`echo $* | sed -e "/.*/y/ABCDEFGHIJKLMNOPQRSTUVWXYZ/abcdefghijklmnopqrstuvwxyz/"`

script="/.*/y/ABCDEFGHIJKLMNOPQRSTUVWXYZ/abcdefghijklmnopqrstuvwxyz/
/<band>$band/{"
script=$script'n
s!^ *<title>\([^<]*\).*!\t\1!
p
}'

echo $*:
sed -n "$script" bh.xml
```

Which produces such glorious output as this: 

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

This was all done on the command line using Cygwin (a Linux emulator for Windows) and the vi text editor, by the way.

In the back of my mind, though, I had a feeling that my XML schema may not be as robust as it could be, but I didn't give it much thought as I was too consumed with the vagaries and vicissitudes of everyday life. Then one day something unexpected happened and I was confronted with a new stark reality.   

On January 9 2010, I broke my schema.

You see, some tapes are sold as a *lot* meaning multiple tapes sold as one item. To account for this, I used a `price` attribute on the `<album>` element for tapes sold individually and a `subtotal` attribute on the `<sale>` element for tapes sold as a lot. This had been adequate until one crisp January afternoon when I decided to buy a single tape **and** a lot from the same seller at the same time. And that was that.  
My Weltanschauung went kaput.

It was time to refactor my XML.

## Phase 2

It was really a simple solution, melodrama notwithstanding.   
- I wrapped each individual album and lot in an `<item>` tag. 
- I dispensed with using attributes altogether and moved everything into its own element to make it easier to process with XSLT.
- I moved `price` from an attribute of `<album>` to a child element of `<item>` thus ridding myself of the dreaded subtotal once and for all. 
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
  <sale>
    <store>eBay</store>
    <seller>tragedian1</seller>
    <date>2007-01-19</date>
    <total>21.65</total>
    <item>
      <price>7.50</price>
      <album>
        <band>Slayer</band>
        <title>Reign In Blood</title>
        <format>CASSETTE</format>
      </album>
    </item>
    <item>
      <price>2.99</price>
      <album>
        <band>Trouble</band>
        <title>The Skull</title>
        <format>CASSETTE</format>
      </album>
    </item>
    <item>
      <price>7.16</price>
      <album>
        <band>Xentrix</band>
        <title>Shattered Existence</title>
        <format>CASSETTE</format>
      </album>
    </item>
  </sale>
  <sale>
    <store>eBay</store>
    <seller>mmusic</seller>
    <date>2008-04-24</date>
    <total>33.45</total>
    <item>
      <price>4.99</price>
      <album>
        <band>Oingo Boingo</band>
        <title>Best Of Boingo</title>
        <format>CASSETTE</format>
      </album>
    </item>
    <item>
      <price>5.99</price>
      <album>
        <band>Oingo Boingo</band>
        <title>Boingo Alive</title>
        <format>CASSETTE</format>
      </album>
    </item>
    <item>
      <price>5.99</price>
      <album>
        <band>Sacrilege</band>
        <title>Within The Prophecy</title>
        <format>CASSETTE</format>
      </album>
    </item>
    <item>
      <price>5.99</price>
      <album>
        <band>Holy Moses</band>
        <title>Queen Of Siam</title>
        <format>CASSETTE</format>
      </album>
    </item>
    <item>
      <price>4.99</price>
      <album>
        <band>Nevermore</band>
        <title>Nevermore</title>
        <format>CASSETTE</format>
      </album>
    </item>
  </sale>
</bh>
```

Around the time of the Mayan apocalypse, I picked up a book called *XSLT: Working with XML and HTML* by Khun Yee Fung which I used to transform my XML into HTML. I also got an account with a web hosting service and with a FileZilla FTP client and a little research into PHP, I brought my fetish to the light of day.

[Peter Grace's Tape Buying History](https://petergrace.site/buying-history/)

Ultimately however, my goal was to store it in a database and create a web form to update it. I started making one using Javascript but my knowledge of Javascript was limited to an Introduction to Computer Science class I took in 2005; I wasn't even aware of jQuery at the time. This was 2013. 

For years I kept buying tapes and updating my XML file manually (utilizing a bash script, of course), until July of 2020 when I had the opportunity to create a console application in C# using Entity Framework Code First. It should come as no surprise that my first thought was to bring my years-long goal to fruition. And so I did.

## Stage Three

I realize that it's purely a vanity piece of software but it means a lot to me.


