
using System;
using System.Drawing.Drawing2D;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace oop3.DisplayObjects
{
    internal class GameField : RectangleObject
    {
        // client field coordinates
        public double clientX1, clientY1;
        public double clientX2, clientY2;


        // velocity value ranges
        private readonly int velModuloMin = 1;
        private readonly int velModuloMax = 1;

        // acceleration value ranges
        private readonly int accModuloMin = 1;
        private readonly int accModuloMax = 1;


        // gamefield objects storage
        private readonly int MaxObjects = 100;
        private DisplayObject[] objects;
        private int objCount;

        private Random random;
        public bool isAccelerated;


        // time stage to adjust to CCD
        private double timeNextCollision = 0;
        private int idxCollisionSource = -1;
        private int idxCollisionDest = -1;

        private int lastKnownAmount;
        private static readonly int threadsAmnt = Environment.ProcessorCount;
        private ThreadData[] threads;
        //private ManualResetEvent[] events;
        private object mutexTimeChange = new object();
        private object mutexThreadStatus = new object();
        private CountdownEvent threadsStatus;



        // Initializer for all constructors of the DrawField
        private void InitializeFields(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int borderThickness)
        {

            rectX1 = topLeftX;
            rectY1 = topLeftY;
            rectX2 = bottomRightX;
            rectY2 = bottomRightY;

            clientX1 = rectX1 + borderThickness;
            clientY1 = rectY1 + borderThickness;
            clientX2 = rectX2 - borderThickness;
            clientY2 = rectY2 - borderThickness;

            objects = new DisplayObject[MaxObjects];
            objCount = 0;

            random = new Random();
            isAccelerated = false;
            clickHandler = () =>
            {
                Console.WriteLine("Gamefield clicked");
            };


            lastKnownAmount = objCount;
            threads = new ThreadData[threadsAmnt];
            //events = new ManualResetEvent[threadsAmnt];
            threadsStatus = new CountdownEvent(objCount);
            for(int i=0;i<threadsAmnt;i++){
                threads[i].arrayID1 = 0;
                threads[i].arrayID2 = 0;
                //ManualResetEvent e = new ManualResetEvent(false);
                //threads[i].rst = e;
                //events[i] = e;
            }
        }


        // Constructor for field with fill color
        public GameField(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int borderThickness, Color? fill = null) :
            base((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2,
                bottomRightX - topLeftX, bottomRightY - topLeftY, fill)
        {
            InitializeFields(topLeftX, topLeftY, bottomRightX, bottomRightY, borderThickness);
        }

        // Constructor for field with fill texture
        public GameField(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, int borderThickness, Bitmap bmp) :
            base((topLeftX + bottomRightX) / 2, (topLeftY + bottomRightY) / 2,
                bottomRightX - topLeftX, bottomRightY - topLeftY, bmp)
        {
            InitializeFields(topLeftX, topLeftY, bottomRightX, bottomRightY, borderThickness);
        }

        public override bool Click(int mouseX, int mouseY)
        {
            if (base.Click(mouseX, mouseY))
            {
                // transfer coordinates to relative to gamefield coordinates
                mouseX -= (int)Math.Floor(clientX1);
                mouseY -= (int)Math.Floor(clientY1);

                int i = objCount - 1;
                bool clickHandled = false;
                while (!clickHandled && i >= 0)
                {
                    clickHandled = objects[i].Click(mouseX, mouseY);
                    i--;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public int? AddObject(DisplayObject obj)
        {
            if (objCount != MaxObjects)
            {
                objects[objCount] = obj;
                return objCount++;
            }
            return null;
        }

        public bool DeleteObject(int index)
        {
            if (index < objCount)
            {
                for (int i = index; i < objCount - 1; i++)
                {
                    objects[i] = objects[i + 1];
                }
                objCount--;
                return true;
            }
            return false;

        }
        public bool DeleteObject(DisplayObject obj)
        {
            int i = 0;
            bool flagFound = false;
            while (i < objCount && !flagFound)
            {
                flagFound = objects[i] == obj;
                i++;
            }
            if (flagFound)
            {
                return DeleteObject(i);
            }
            else
            {
                return false;
            }
        }

        public void MoveObjects(double deltaTime)
        {
            DisplayObject obj;


            bool circlesPresent = true;
            if (timeNextCollision <= 0)
            {
                FindNextCollision();
                circlesPresent = timeNextCollision >= 0;
            }

            // while next collision is in the current period
            // of time, process the collisions (and move)
            if (circlesPresent) { 
                while (timeNextCollision < deltaTime) {
                    // move objects to the next collision (by timeNextCollision time)
                    for (int i = 0; i < objCount; i++)
                    {
                        obj = objects[i];
                        obj.Move(timeNextCollision);
                    }

                    // subtract the time from the time left (delta time)
                    deltaTime -= timeNextCollision;

                    CircleObject dest = objects[idxCollisionDest] as CircleObject;

                    // process the collision of the objects
                    if (idxCollisionSource == -1)
                    {
                        // wall collision
                        dest.WallCollide(0, 0, clientX2 - clientX1, clientY2 - clientY1);
                    }
                    else
                    {
                        // circle collision
                        CircleObject src = objects[idxCollisionSource] as CircleObject;
                        dest.CircleCollide(src);
                        src.CircleCollide(dest);
                    }

                    // apply the new velocities
                    for (int i = 0; i < objCount; i++) {
                        if (objects[i].R)
                        {
                            objects[i].velAlpha = objects[i].newVelAlpha;
                            objects[i].velModulo = objects[i].newVelModulo;
                        }
                    }

                    // find next collision
                    FindNextCollision();
                }
            }

            // move by the time left (delta time)
            for (int i = 0; i < objCount; i++)
            {
                obj = objects[i];
                obj.Move(deltaTime);
            }

            // and subtract from the next collision time
            timeNextCollision -= deltaTime;





            // adjust other objects
            for(int i=0; i<objCount; i++) {
                DisplayObject dobj = objects[i];
                if (!(dobj is CircleObject)) {
                    AdjustObjToBounds(dobj);
                }
            }

        }

        struct ThreadData {
            public int arrayID1;
            public int arrayID2;
            public ThreadData(int arrayStart, int arrayEnd) {
                arrayID1 = arrayStart;
                arrayID2 = arrayEnd;
            }
        }

        private void FindNextCollision() {
            // array of ThreadData is required for the threads to work
            
            if (lastKnownAmount != objCount) {
                // recalculate the indices
                int totalIndices = objCount;
                int start = 0;

                int partSize = totalIndices / threadsAmnt;
                int remainder = totalIndices % threadsAmnt;
                for (int i = 0; i < threadsAmnt; i++) {
                    int end = start + partSize - 1;
                    if (remainder > 0) {
                        end++;
                        remainder--;
                    }
                    threads[i].arrayID1 = start;
                    threads[i].arrayID2 = end;

                    start = end + 1;
                }
                lastKnownAmount = objCount;


            }
            
            timeNextCollision = -1;
            threadsStatus = new CountdownEvent(threadsAmnt);
            // now we can start multithreading
            for (int i = 0; i < threadsAmnt; i++) {
                ThreadPool.QueueUserWorkItem(ThreadSeeker, threads[i]);
            }
            threadsStatus.Wait();      
            

            //ThreadSeeker(new ThreadData(0, objCount - 1));
            //foreach (var e in events) {
            //    e.WaitOne();
            //}
            for (int i = 0; i < objCount; i++)
            {
                objects[i].R = false;
            }
        }

        private void ThreadSeeker(object threadContext) {
            ThreadData context = (ThreadData)threadContext;

            double? timeNext = null;
            int idxCollDest = -1;
            int idxCollSrc = -1;

            DisplayObject dobjSrc, dobjDest;
            CircleObject objSrc, objDest;
            for (int i = context.arrayID1; i <= context.arrayID2; i++)
            {
                dobjSrc = objects[i];
                if (!(dobjSrc is CircleObject)) continue;

                objSrc = dobjSrc as CircleObject;

                // find the time for walls
                double time = objSrc.GetCTForBorders(0, 0, clientX2 - clientX1, clientY2 - clientY1);

                if (time >= 0)
                {                 
                    if ((timeNext == null) || (timeNext != null && timeNext.Value > time))
                    {
                        timeNext = time;
                        idxCollDest = i;
                        idxCollSrc = -1;
                    }                    
                }
                
                for (int j = 0; j < objCount; j++)
                {
                    if (i == j) continue;
                    dobjDest = objects[j];
                    if (!(dobjDest is CircleObject)) continue;

                    objDest = dobjDest as CircleObject;

                    // find the time for circles
                    if (objDest.R && objSrc.R) continue;
                    time = objSrc.GetCTForCircle(objDest);
                    if (time >= 0)
                    {
                         
                        if ((timeNext == null) || (timeNext != null && timeNext.Value > time))
                        {
                            timeNext = time;
                            idxCollDest = j;
                            idxCollSrc = i;
                        }
                        
                    }

                }
            }

            if (timeNext != null)
            {
                lock (mutexTimeChange) {
                    if (timeNextCollision <= 0 || timeNextCollision > timeNext.Value) {
                        if (timeNext.Value > 0.000000001) { 
                            timeNextCollision = timeNext.Value;
                            idxCollisionDest = idxCollDest;
                            idxCollisionSource = idxCollSrc;
                        }
                    }
                }
            }
            //context.rst.Set();
            lock (mutexThreadStatus) { 
                threadsStatus.Signal();
            }
        }

        private void AdjustObjToBounds(DisplayObject obj) {
            double shiftX, shiftY;
            double vModulo, vAlpha;
            double aModulo, aAlpha;
            while (!obj.IsInBounds(0, 0, (int)(clientX2 - clientX1), (int)(clientY2 - clientY1)))
            {
                (shiftX, shiftY) = RandomGamefieldPoint();
                shiftX -= obj.anchorX;
                shiftY -= obj.anchorY;
                obj.ShiftObject(shiftX, shiftY);

                (vModulo, vAlpha) = RandomVelocityVector();
                obj.velModulo = vModulo;
                obj.velAlpha = vAlpha;

                if (isAccelerated)
                {
                    (aModulo, aAlpha) = RandomAcceleration();
                    obj.accModulo = aModulo;
                    obj.accAlpha = aAlpha;
                }
            }
        }

        protected override void UpdateFrame()
        {
            // form the point collection 
            double[] coordsX = [rectX1, rectX2, rectX1, rectX2];
            double[] coordsY = [rectY1, rectY1, rectY2, rectY2];

            // pass it to the method
            SetFrameFromPoints(anchorX, anchorY, coordsX, coordsY);
        }

        public override void Draw(Graphics g)
        {
            Brush strokeBrush = GetStrokeBrush();
            Brush fillBrush = GetFillBrush();

            GraphicsState prevState = MatrixRotate(g);
            g.TranslateTransform((float)rectX1, (float)rectY1);

            // Drawing the outer frame
            Rectangle rectOuter = new Rectangle(0, 0, (int)(rectX2 - rectX1), (int)(rectY2 - rectY1));
            g.FillRectangle(strokeBrush, rectOuter);
            g.DrawRectangle(new Pen(strokeBrush), rectOuter);

            // Drawing the client area            
            g.TranslateTransform((float)(clientX1 - rectX1), (float)(clientY1 - rectY1));
            Rectangle rectClient = new Rectangle(0, 0, (int)(clientX2 - clientX1), (int)(clientY2 - clientY1));
            g.FillRectangle(fillBrush, rectClient);
            g.DrawRectangle(new Pen(fillBrush), rectClient);


            // drawing the objects inside the field
            for (int i = 0; i < objCount; i++)
            {
                objects[i].Draw(g);
            }

            g.Restore(prevState);
        }

        protected override void ShiftCoords(double deltaX, double deltaY)
        {
            rectX1 += deltaX;
            rectY1 += deltaY;
            rectX2 += deltaX;
            rectY2 += deltaY;

            clientX1 += deltaX;
            clientY1 += deltaY;
            clientX2 += deltaX;
            clientY2 += deltaY;
        }


        public int RandomAngle () => (random.Next(360));
        public (int, int) RandomGamefieldPoint()
        {
            return (random.Next((int)(clientX2 - clientX1)),
                    random.Next((int)(clientY2 - clientY1)));
        }
        public (int, int) RandomVelocityVector()
        {
            return (random.Next(velModuloMin, velModuloMax), RandomAngle());
        }

        public (int, int) RandomAcceleration()
        {
            return (random.Next(accModuloMin, accModuloMax), RandomAngle());
        }

    }
}
