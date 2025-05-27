program Lab4;
{
 Given a non-empty sequence of words consisting of lowercase
 Russian letters, with commas between adjacent words and
 a period after the last word. Output all voiced consonant
 letters that appear in at least one word, in alphabetical order.
}

uses
  System.SysUtils;

type
  TZvonkSogl = (б, в, г, д, ж, з, л, м, н, р);
  TSetOfZvonkSogl = set of TZvonkSogl;
  // TZvonkSogl - enumerates voiced consonant letters
  // TSetOfZvonkSogl - defines a set of voiced consonant letters

const
  // Voiced consonant letters set
  ZvonkSogl = ['б', 'в', 'г', 'д', 'ж', 'з', 'л', 'м', 'н', 'р'];

function GetTypeOfZvonkSogl(const AChar: AnsiChar): TZvonkSogl;
begin
  // Maps a character to its corresponding voiced consonant letter
  case AChar of
    'б': Result := TZvonkSogl.б;
    'в': Result := TZvonkSogl.в;
    'г': Result := TZvonkSogl.г;
    'д': Result := TZvonkSogl.д;
    'ж': Result := TZvonkSogl.ж;
    'з': Result := TZvonkSogl.з;
    'л': Result := TZvonkSogl.л;
    'м': Result := TZvonkSogl.м;
    'н': Result := TZvonkSogl.н;
    'р': Result := TZvonkSogl.р;
  end;
end;

function GetCharFromZvonkSogl(const AZvonkSogl: TZvonkSogl): AnsiChar;
begin
  // Maps a voiced consonant letter to its corresponding character
  case AZvonkSogl of
    TZvonkSogl.б: Result := 'б';
    TZvonkSogl.в: Result := 'в';
    TZvonkSogl.г: Result := 'г';
    TZvonkSogl.д: Result := 'д';
    TZvonkSogl.ж: Result := 'ж';
    TZvonkSogl.з: Result := 'з';
    TZvonkSogl.л: Result := 'л';
    TZvonkSogl.м: Result := 'м';
    TZvonkSogl.н: Result := 'н';
    TZvonkSogl.р: Result := 'р';
  end;
end;

procedure InstallZvonkSogl(const ASequence: AnsiString; var ASetOfZvonkSogl: TSetOfZvonkSogl);
var
  CurrWord, RemWords: AnsiString;
  CommaIndex: Integer;
  I: Integer;
begin
  // Extracts voiced consonant letters from the given sequence and adds them to the set
  CommaIndex := Pos(',', ASequence);
  if CommaIndex = 0 then
  begin
    // If there are no commas, the current word is the entire sequence
    CurrWord := ASequence;
    RemWords := '';
  end
  else
  begin
    // Extracts the current word and the remaining words from the sequence
    CurrWord := Copy(ASequence, 1, CommaIndex - 1);
    RemWords := Copy(ASequence, CommaIndex + 1, Length(ASequence));
  end;

  for I := 1 to Length(CurrWord) do
    if CurrWord[I] in ZvonkSogl then
      Include(ASetOfZvonkSogl, GetTypeOfZvonkSogl(CurrWord[I]));

  if RemWords <> '' then
    // Recursively process the remaining words in the sequence
    InstallZvonkSogl(RemWords, ASetOfZvonkSogl);
end;

function GetCorrectStr: AnsiString;
var
  isCorrect: Boolean;
  I, StrHigh: Integer;
  function IsValidCharacter(const AChar: AnsiChar): Boolean;
  begin
    // Checks if a character is a valid lowercase Russian letter
    Result := (AChar >= 'а') and (AChar <= 'я');
  end;
begin
  // Reads a string from input and performs validation on the format and characters
  repeat
    isCorrect:= True;
    Readln(Result);
    Result := LowerCase(Result);

    if Result[High(Result)] <> '.' then
    begin
      Writeln('Некорректный ввод');
      isCorrect:= False;
    end;

    I := 1;
    StrHigh := Length(Result) - 2;
    while (I <= StrHigh) and isCorrect do
    begin
      if ((Result[I] = ',') and not IsValidCharacter(Result[I + 1])) or
         (Result[I] <> ',') and not IsValidCharacter(Result[I]) then
      begin
        Writeln('Некорректный ввод');
        isCorrect:= False;
      end;
      Inc(I);
    end;

  until isCorrect;
end;

var
  Sequence: AnsiString;
  SetOfZvonkSogl: TSetOfZvonkSogl;
begin
  // Main program logic
  Writeln('Введите последовательность слов (запятая между соседними словами, точка после последнего слова):');
  Sequence:= GetCorrectStr;
  InstallZvonkSogl(Sequence, SetOfZvonkSogl);

  Writeln('Найдены следующие звонкие согласные буквы, которые входят хотя бы в одно слово:');
  for var I := Low(TZvonkSogl) to High(TZvonkSogl) do
    if I in SetOfZvonkSogl then
      Write(GetCharFromZvonkSogl(I), ' ');
  Writeln;
  Readln;
end.

