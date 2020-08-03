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
