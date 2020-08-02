# buying-history
I used to have a meager collection of cassette tapes when I was a teenager, but I sold most of them during a turning point in my life. 
I started buying cassette tapes on eBay and Amazon in 2006. I kept track of my purchases in a text file with a format of my own making.
```
_gnasher670:Testament{"Practice What You Preach","The New Order"}:2.37:7.27:1/3/07
millimidian:King Diamond{"Them"}:3.50:6.45:1/16/07
tragedian1:Slayer{"Reign In Blood":7.50},Trouble{"The Skull":2.99},Xentrix{"Shattered Existence":7.16}::21.65:1/19/07
mmusic:Oingo Boingo{"Best Of Boingo":4.99,"Boingo Alive":5.99},Sacrilege{"Within The Prophecy":5.99},Holy Moses{"Queen Of Siam":5.99},Nevermore{"Nevermore":4.99}::33.45:4/24/08
```
Not exactly reader-friendly.  
In 2008 I found a book about XSLT and decided to convert my precious gobbledygook into XML. 
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
</buyinghistory>
```
At a certain point I realized a couple things. I wasn't keeping track of where I bought the tapes (eBay or Aamazon), some sales contained multiple tapes in one item, and occasionally I bought a CD. So I had to alter my schema to reflect that. I also decided to keep all data in elements instead of attributes.  
Around the time the Mayans allegedly predicted that the end of the world, I found a book about XSLT, and with a little research into PHP, I converted my XML file into HTML.  
![Peter Grace's Tape Buying History](buyinghistory-scrshot-1.png)

I created a Bash script to add new sales 

