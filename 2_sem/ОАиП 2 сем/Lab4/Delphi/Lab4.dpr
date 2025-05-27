program Lab4;
{
 The program generates a random group of people, where each person
 has a list of guests they have visited. It then checks if there are
 individuals who have not visited anyone and outputs their names.
 If everyone in the group has visited someone, it displays a message
 indicating so.
}

type
  TName = (Vasya, Volodya, Ira, Lida, Marina, Misha,
          Natasha, Oleg, Olga, Sveta, Julia);
  TGuests = set of TName;
  TGroup = array[TName] of TGuests;
  TVisited = array[TName] of Boolean;
  // TName - enumeration representing the names of individuals in the group
  // TGuests - set of names representing the guests visited by an individual
  // TGroup - array representing the group, where each individual is
  // associated with their visited guests
  // TVisited - array indicating whether an individual has visited
  // someone or not

const
  Name: array[TName] of string = ('Vasya', 'Volodya', 'Ira', 'Lida',
                                  'Marina', 'Misha', 'Natasha', 'Oleg',
                                  'Olga', 'Sveta', 'Julia');
  // Name - array containing the names corresponding to each TName
  // enumeration value

// Finds people who haven't visited anyone
function IsNowhere(const AGr: TGroup; var AVisited: TVisited): Boolean;
var
  Person: TName;
  Guest: TName;
  flag: Boolean;
begin
  // Iterate through each guest and check if they have been visited
  // by any individual in the group
  for Guest := Low(TName) to High(TName) do
  begin
    flag:= true;
    Person := Low(TName);
    while (Person <= High(TName)) and flag do
    begin
      // If the guest has been visited by the current individual, mark them as visited
      if Guest in AGr[Person] then
      begin
        AVisited[Guest] := True;
        flag := False;
      end;
      Inc(Person);
    end;
  end;

  // Iterate through each individual and check if they have not visited anyone
  Result:= False;
  Person := Low(TName);
  while (Person <= High(TName)) and not Result do
  begin
    // If an individual has not visited anyone, set the Result flag to True
    if not AVisited[Person] then
      Result:= True;
    Inc(Person);
  end;
end;

procedure OutputUnvisited(const AVisited: TVisited);
var
  Person: TName;
begin
  // Output the names of individuals who have not visited anyone
  for Person := Low(TName) to High(TName) do
    if not AVisited[Person] then
      Writeln(Name[Person], ' ');
end;

procedure GenerateRandomGroup(var AGr: TGroup);
var
  Person: TName;
  Guest: TName;
begin
  Randomize;

  for Person := Low(TName) to High(TName) do
  begin
    // Output the current individual for whom we are generating the guest list
    Writeln('People who visited ', Name[Person], ':');

    // Clear the set of guests for the current individual
    AGr[Person] := [];
    for Guest := Low(TName) to High(TName) do
    begin
      // Randomly include guests for the current individual
      if (Random(3) = 0) and (Guest <> Person) then
      begin
        Include(AGr[Person], Guest);
        Write(Name[Guest], ' ');
      end;
    end;

    // If there are no guests for the current individual, output "None"
    if AGr[Person] = [] then
      Write('None');
    Writeln;
  end;
end;

var
  Gr: TGroup;
  Visited: TVisited;

begin
  // Generate a random group with guest lists for each individual
  GenerateRandomGroup(Gr);
  Writeln;

  // Check if there are individuals who have not visited anyone and output their names
  if IsNowhere(Gr, Visited) then
  begin
    Writeln('The following people have not visited someone:');
    OutputUnvisited(Visited);
  end

  // If everyone in the group has visited someone, output a message indicating so
  else
    Writeln('All the people in the group have been visiting someone');

  Readln;
end.
