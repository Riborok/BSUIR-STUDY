using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Syntax
{
    public class ElementTypes
    {
        public ElementType this[string index]
        {
            get
            {
                switch (index)
                {
                    case "a1":
                        return new TerminalElementType("a1", new Line(new Point(0, 0), new Point(20, 0)));
                    case "a2":
                        return new TerminalElementType("a2", new Line(new Point(0, 0), new Point(0, 20)));
                    case "a3":
                        return new TerminalElementType("a3", new Line(new Point(0, 0), new Point(20, 20)));
                    case "a4":
                        return new TerminalElementType("a4", new Line(new Point(20, 0), new Point(0, 20)));
                }
                return new ElementType(index);
            }
        }
    }
    
    public class HomeGrammar
    {
        private static readonly ElementTypes ElementTypes = new ElementTypes();
        private readonly List<Rule> rules;
        private readonly ElementType startElementType;

        public HomeGrammar()
        {
            startElementType = new ElementType("Fox");

            rules = new List<Rule>
            {
                // Доп элементы
                new LeftRule(ElementTypes["UpperTriangle"], ElementTypes["a3"], ElementTypes["a4"]),
                new LeftRule(ElementTypes["LowerTriangle"], ElementTypes["a4"], ElementTypes["a3"]),
                new UpRule(ElementTypes["Rhombus"], ElementTypes["UpperTriangle"], ElementTypes["LowerTriangle"]),
                
                // Тело
                new LeftRule(ElementTypes["BodyWithRightHand"], ElementTypes["Rhombus"], ElementTypes["a4"]),
                new LeftRule(ElementTypes["BodyWithHands"], ElementTypes["a3"], ElementTypes["BodyWithRightHand"]),
                new UpRule(ElementTypes["BodyWithArmsLegs"], ElementTypes["BodyWithHands"], ElementTypes["UpperTriangle"]),
                
                // Голова
                new UpRule(ElementTypes["ClosedUpperTriangle"], ElementTypes["UpperTriangle"], ElementTypes["a1"]),
                new LeftRule(ElementTypes["Ears"], ElementTypes["ClosedUpperTriangle"], ElementTypes["ClosedUpperTriangle"]),
                new UpRule(ElementTypes["ClosedLowerTriangle"], ElementTypes["a1"], ElementTypes["LowerTriangle"]),
                new UpRule(ElementTypes["Face"], ElementTypes["Ears"], ElementTypes["ClosedLowerTriangle"]),
                
                new UpRule(startElementType, ElementTypes["Face"], ElementTypes["BodyWithArmsLegs"]),
            };
        }

        public Element GetHome()
        {
            return GetElement(startElementType);
        }

        private Element GetElement(ElementType elementType)
        {
            if (elementType is TerminalElementType terminalElementType)
            {
                return terminalElementType.StandartElement;
            }

            Rule rule = rules.FirstOrDefault(x => x.StartElementType.Name == elementType.Name);
            Debug.Assert(rule != null, "rule != null");
            return rule.TransformConnect(GetElement(rule.FirstArgumentType),
                GetElement(rule.SecondArgumentType));
        }

        public RecognazingResult IsHome(IEnumerable<Element> baseElements)
        {
            var elements = new ConcurrentBag<Element>(baseElements);
            for (int i = 0; i < rules.Count; i++)
            {
                ContainRuleAgrumentsResult result = ContainRuleAgruments(elements, rules[i]);
                elements = result.Elements;
                if (!result.IsElementFound)
                    return new RecognazingResult(rules[i].StartElementType.Name, false);
            }
            return new RecognazingResult("", true);
        }

        private static ContainRuleAgrumentsResult ContainRuleAgruments(
            ConcurrentBag<Element> elements, Rule rule)
        {
            var result = new ContainRuleAgrumentsResult
            {
                Elements = new ConcurrentBag<Element>(elements),
                IsElementFound = false
            };

            foreach (Element firstElement in elements)
            {
                if (firstElement.ElementType.Name == rule.FirstArgumentType.Name)
                {
                    result = ContainRuleAgrumentsForFirstElement(elements, rule, firstElement,
                        result);
                }
            }
            return result;
        }

        private static ContainRuleAgrumentsResult ContainRuleAgrumentsForFirstElement(
            IEnumerable<Element> elements, Rule rule,
            Element firstElement, ContainRuleAgrumentsResult result)
        {
            Element element = firstElement;
            Parallel.ForEach(elements, (Element secondElement) =>
            {
                if (rule.IsRulePare(element, secondElement))
                {
                    result.Elements.Add(rule.Connect(element, secondElement));
                    result.IsElementFound = true;
                }
            });
            return result;
        }

        public static Element GetTerminalElement(Line line)
        {
            String resultName = GetTerminalElementName(line);
            return new Element(ElementTypes[resultName], line);
        }

        private static string GetTerminalElementName(Line line)
        {
            double deltaX = line.From.X - line.To.X;
            double deltaY = line.From.Y - line.To.Y;
            if (Math.Abs(deltaY) < 1) return "a1";
            if (Math.Abs(deltaX) < 1) return "a2";
            if (Math.Abs(deltaX) < 1) return "a2";
            if (Math.Abs(deltaX/deltaY) < 0.2) return "a2";
            if (Math.Abs(deltaY/deltaX) < 0.2) return "a1";
            Point highPoint = line.To.Y > line.From.Y ? line.To : line.From;
            Point lowPoint = line.To.Y < line.From.Y ? line.To : line.From;
            if (highPoint.X < lowPoint.X) return "a4";
            return "a3";
        }

        private class ContainRuleAgrumentsResult
        {
            public ConcurrentBag<Element> Elements { get; set; }
            public bool IsElementFound { get; set; }
        }
    }
}