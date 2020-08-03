#n
/<subtotal>/{
s! *<subtotal>\([^<]*\).*!Subtotal \1!
p
}

/<total>/{
s! *<total>\([^<]*\).*!Total \1!
p
}

/<price>/{
s! *<price>\([^<]*\).*!Price \1!
p
}

/<\/sale>/{
i\

}
