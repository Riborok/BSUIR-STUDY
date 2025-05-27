unit GraphUtils;

interface

uses
 System.Generics.Collections, System.Generics.Defaults;

type
  TMatrix = array of array of Integer;
  TPath = record
    Vertices: TList<Integer>;
    Distance: Integer;
  end;
  TPQEntry = record
    Vertex: Integer;
    Distance: Integer;
  end;

  TPQList = TArray<TPQEntry>;
  TPaths = TList<TPath>;

  const
    INFINITY = MaxInt;

  function FindCenter(const AdjacencyMatrix: TMatrix): Integer;
  function FindAllPaths(const AdjacencyMatrix: TMatrix; const startVertex, targetVertex: Integer): TPaths;
  function FindShortestPath(const AdjacencyMatrix: TMatrix; const startVertex, targetVertex: Integer): TPath;
  function FindLongestPath(const AdjacencyMatrix: TMatrix; const startVertex, targetVertex: Integer): TPath;
implementation

  procedure DeleteElement(var AArray: TPQList; Index: Integer);
  var
    i: Integer;
  begin
    if (Index >= 0) and (Index < Length(AArray)) then
    begin
      for i := Index to High(AArray) - 1 do
        AArray[i] := AArray[i + 1];

      SetLength(AArray, Length(AArray) - 1);
    end;
  end;

  function ExtractMin(var PQ: TPQList): Integer;
  var
    i, minIndex: Integer;
  begin
    minIndex := 0;

    for i := 1 to High(PQ) do
      if PQ[i].Distance < PQ[minIndex].Distance then
        minIndex := i;

    Result := PQ[minIndex].Vertex;
    DeleteElement(PQ, minIndex);
  end;

  procedure Relax(u, v, w: Integer; var D: TArray<Integer>; var P: TArray<Integer>);
  begin
    if D[v] > D[u] + w then
    begin
      D[v] := D[u] + w;
      P[v] := u;
    end;
  end;

  function Dijkstra(const AdjacencyMatrix: TMatrix; const startVertex, targetVertex: Integer): TPath;
  var
    D: TArray<Integer>;
    P: TArray<Integer>;
    PQ: TPQList;
    u, v, w, i: Integer;
  begin
    SetLength(D, Length(AdjacencyMatrix));
    SetLength(P, Length(AdjacencyMatrix));
    SetLength(PQ, Length(AdjacencyMatrix));

    for i := 0 to High(AdjacencyMatrix) do
    begin
      D[i] := INFINITY;
      P[i] := -1;
      PQ[i].Vertex := i;
      PQ[i].Distance := INFINITY;
    end;

    D[startVertex] := 0;
    PQ[startVertex].Distance := 0;

    while Length(PQ) > 0 do
    begin
      u := ExtractMin(PQ);

      for v := 0 to High(AdjacencyMatrix) do
        if (AdjacencyMatrix[u, v] <> 0) and (u <> v) and (D[u] <> INFINITY) then
        begin
          w := AdjacencyMatrix[u, v];
          Relax(u, v, w, D, P);
        end;
    end;

    Result.Vertices := TList<Integer>.Create;
    v := targetVertex;

    while (v <> -1) and (v <> startVertex) do
    begin
      Result.Vertices.Insert(0, v);
      v := P[v];
    end;
    Result.Vertices.Insert(0, startVertex);
    Result.Distance := D[targetVertex];
  end;

  function BellmanFord(const AdjacencyMatrix: TMatrix; const startVertex, targetVertex: Integer): TPath;
  var
    D: TArray<Integer>;
    P: TArray<Integer>;
    i, u, v, w: Integer;
  begin
    SetLength(D, Length(AdjacencyMatrix));
    SetLength(P, Length(AdjacencyMatrix));

    for i := 0 to High(AdjacencyMatrix) do
    begin
      D[i] := -INFINITY;
      P[i] := -1;
    end;

    D[startVertex] := 0;

    for i := 1 to High(AdjacencyMatrix) do
      for u := 0 to High(AdjacencyMatrix) do
        for v := 0 to High(AdjacencyMatrix) do
          if (AdjacencyMatrix[u, v] <> 0) and (D[u] <> -INFINITY) then
          begin
            w := AdjacencyMatrix[u, v];
            if D[v] < D[u] + w then
            begin
              D[v] := D[u] + w;
              P[v] := u;
            end;
          end;

    Result.Vertices := TList<Integer>.Create;
    v := targetVertex;

    while (v <> -1) and (v <> startVertex) do
    begin
      Result.Vertices.Insert(0, v);
      v := P[v];
    end;
    Result.Vertices.Insert(0, startVertex);
    Result.Distance := D[targetVertex];
  end;

  function GetFloyd(const AdjacencyMatrix: TMatrix):TMatrix;
  var
    i, j, k:Integer;
  begin
    SetLength(Result, Length(AdjacencyMatrix), Length(AdjacencyMatrix));
    for i := 0 to High(Result) do
      for j := 0 to High(Result) do
        if (AdjacencyMatrix[i, j] <> 0) then
          Result[i, j] := AdjacencyMatrix[i, j]
        else
          Result[i, j] := MaxInt;

    for k := 0 to High(Result) do
      for i := 0 to High(Result) do
        for j := 0 to High(Result) do
          if (Result[i, k] <> MaxInt) and (Result[k, j] <> MaxInt) then
            if Result[i, k] + Result[k, j] < Result[i, j] then
              Result[i, j] := Result[i ,k] + Result[k, j];
  end;

  function FindCenter(const AdjacencyMatrix: TMatrix): Integer;
  var
    Floid: TMatrix;
    MaxWay: array of Integer;
    i, j:Integer;
  begin
    Floid := GetFloyd(AdjacencyMatrix);

    SetLength(MaxWay, Length(Floid));
    for i:= 0 to High(Floid) do
    begin
      MaxWay[i]:= Floid[0,i];
      for j:= 1 to High(Floid) do
        if MaxWay[i] < Floid[j, i] then
          MaxWay[i] := Floid[j, i];
    end;

    Result:=0;
    for i := 0 to High(Floid) do
      if MaxWay[i] < MaxWay[Result] then
        Result := i;
  end;

  function FindShortestPath(const AdjacencyMatrix: TMatrix; const startVertex, targetVertex: Integer): TPath;
  begin
    Result := Dijkstra(AdjacencyMatrix, startVertex, targetVertex);
  end;

  function FindLongestPath(const AdjacencyMatrix: TMatrix; const startVertex, targetVertex: Integer): TPath;
  var
    A: TMatrix;
    Min, i, j: Integer;
  begin
    SetLength(A, Length(AdjacencyMatrix), Length(AdjacencyMatrix));

    for i := 0 to High(A) do
      for j := 0 to High(A) do
        A[i, j] := -AdjacencyMatrix[i, j];

    Min := MaxInt;
    for i := 0 to High(A) do
      for j := 0 to High(A) do
        if Min > A[i, j] then
          Min := A[i, j];
    Dec(Min);

    for i := 0 to High(A) do
      for j := 0 to High(A) do
        A[i, j] := A[i, j] - Min;

    Result := Dijkstra(A, startVertex, targetVertex);

    Result.Distance := 0;
    for i := 0 to Result.Vertices.Count - 2 do
      Result.Distance := Result.Distance + AdjacencyMatrix[Result.Vertices[i], Result.Vertices[i + 1]];
  end;

function FindAllPaths(const AdjacencyMatrix: TMatrix; const startVertex, targetVertex: Integer): TPaths;
var
  Paths: TPaths;
  Path: TPath;

  procedure FindPathsRecursive(currentVertex, targetVertex: Integer; currentPath: TPath);
  var
    i: Integer;
    newPath: TPath;
  begin
    currentPath.Vertices.Add(currentVertex);

    if currentVertex = targetVertex then
    begin
      currentPath.Distance := 0;
      for i := 0 to currentPath.Vertices.Count - 2 do
        currentPath.Distance := currentPath.Distance + AdjacencyMatrix[currentPath.Vertices[i], currentPath.Vertices[i + 1]];

      newPath.Vertices := TList<Integer>.Create;
      newPath.Vertices.AddRange(currentPath.Vertices);
      newPath.Distance := currentPath.Distance;

      Paths.Add(newPath);
    end
    else
    begin
      for i := 0 to High(AdjacencyMatrix) do
        if (AdjacencyMatrix[currentVertex, i] <> 0) and (currentVertex <> i) and (currentPath.Vertices.IndexOf(i) = -1) then
          FindPathsRecursive(i, targetVertex, currentPath);
    end;

    currentPath.Vertices.Remove(currentVertex);
  end;

begin
  Paths := TPaths.Create;

  Path.Vertices := TList<Integer>.Create;
  FindPathsRecursive(startVertex, targetVertex, Path);

  Paths.Sort(TComparer<TPath>.Construct(
    function(const Left, Right: TPath): Integer
    begin
      Result := Left.Distance - Right.Distance;
    end
  ));

  Result := Paths;
end;


end.
