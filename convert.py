from struct import unpack_from, calcsize
import sys
struct_fmt = "@LLL"

in_file = open(sys.argv[1], "rb")
out_file = open(sys.argv[1].rpartition(".")[0]+".txt", "w")
raw_data = in_file.read()
for offset in range(0,len(raw_data), calcsize(struct_fmt)):
	line = unpack_from(struct_fmt,raw_data[offset:])
	out_file.write(str(line[0])+" "+str(line[1])+" "+str(line[2])+"\n")