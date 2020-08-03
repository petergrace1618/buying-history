#!/bin/bash 
#
# printtotal
# Displays total cost of all tapes in bh.xml

awk 'BEGIN { 
	total = 0 
	subtotal = 0
	price = 0
}

/^Total/{ total += $2 }

/^Subtotal/{ subtotal += $2 }

/^Price/{ price += $2 }

END {
	print "\tTOTALS"
	print "Items priced individually " price
	print "Items priced as a group   " subtotal
	print "Total of two above        " price + subtotal
	print "Price after shipping      " total
	print "Shipping                  " total - (price + subtotal)
}' totals
