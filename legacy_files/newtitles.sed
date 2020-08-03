#n
# newtitles.sed
# Like newbuyers.sed but extracts titles instead of buyers.

/title/{
s/ *<title>\([^<]*\).*/\1/
w titles.new
}
