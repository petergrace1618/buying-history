#n
# oldbuyers.sed
# Extracts the buyer name from bh.old and writes it to buyer.old.  A similar
# script (newbuyers.sed) is also run on the bh.xml which outputs a list of
# buyers to buyers.new. The two output files are then compared with diff
# in order to test the results of convertbh.sed.

/^#/d
s/^\([^:]*\).*/\1/
w buyers.old
