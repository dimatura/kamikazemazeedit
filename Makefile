
CC = gmcs
REFERENCES = -r:System.Drawing -r:System.Windows.Forms

all:
	$(CC) *.cs $(REFERENCES) -out:kamikazemazeedit.exe
