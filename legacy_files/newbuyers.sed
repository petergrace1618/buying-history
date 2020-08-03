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
