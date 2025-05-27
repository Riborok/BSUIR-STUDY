program WarmUp2;
{
 This program provides the user with the option to encode or decode text using a
 encryption algorithm. The user is asked to choose between two options:
 to encode text or to decode it
}

uses
  System.SysUtils,
  En_Decoding in 'Unit\En_Decoding.pas';

var
  Choice: Char;
  isCorrect: Boolean;
  TextFileName, KeyFileName: string;
  // Choice - stores the user's choice
  // isCorrect - indicates whether the user's input was correct or not
  // TextFileName, KeyFileName - store the file names entered by the

begin
  Writeln('Choose option number:');
  Writeln('1 - encode text');
  Writeln('2 - decode text');

  isCorrect:= True;
  repeat
    Readln(Choice);
    case Choice of
      '1':
      begin
        // This option is used to encode a text file
        Writeln('Create a txt file in the following folder: Files -> Decoded.');
        Writeln('Write text to a file and then write the name of this file here.');
        Writeln('You will see in the Files -> Encoded folder the key and the encrypted file.');

        // This loop continues until the user enters a valid file name for the decoded text
        repeat
          Writeln('File name for decoded test:');
          Readln(TextFileName);
        until FileExists('..\..\..\Files\Decoded\' + TextFileName + '.txt');
        Encoding(TextFileName);

        Writeln('The operation was successful');
      end;
      '2':
      begin
        // This option is used to decode a text file
        Writeln('Install two text files in the following folder: Files -> Encoded.');
        Writeln('One file should be encoded text and the second should be key');
        Writeln('Enter the file names in sequence. First the encoded text and then the key');
        Writeln('You will see the decoded text in the Files -> Decoded folder');

        // This loop continues until the user enters valid file names
        repeat
          Writeln('File name for encoded text:');
          Readln(TextFileName);
          Writeln('File name for key:');
          Readln(KeyFileName);
        until FileExists('..\..\..\Files\Encoded\' + TextFileName + '.txt') and
              FileExists('..\..\..\Files\Encoded\' + KeyFileName + '.txt');
        Decoding(TextFileName, KeyFileName);

        Writeln('The operation was successful');

      end;
      else
      begin
        // If the user enters an invalid choice, this message will be displayed
        Writeln('Unavailable option! Try again');
        isCorrect := False;
      end;
    end;

  until isCorrect;

  Readln;
end.
