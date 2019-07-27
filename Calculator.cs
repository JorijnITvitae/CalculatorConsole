using System;
using System.Collections.Generic;
using System.Globalization;

namespace CalculatorConsole
{
    class Calculator
    {
        private enum ItemType
        {
            ANSWER,
            NUMBER,
            SUBTRACT,
            ADD,
            DIVIDE,
            MULTIPLY,
            SQUARE_ROOT,
            ROOT,
            POWER,
        }

        private class Item
        {
            public readonly ItemType type;
            public readonly double number;

            public Item(ItemType itemType)
            {
                type = itemType;
                number = 0;
            }
            
            public Item(double itemNumber)
            {
                type = ItemType.NUMBER;
                number = itemNumber;
            }

            // Return either the number if this is a number item, or compute the operation of x and y.
            public double Compute(double x, double y = 0)
            {
                switch (type)
                {
                    case ItemType.NUMBER:
                        return number;

                    case ItemType.POWER:
                        return Math.Pow(x, y);

                    case ItemType.ROOT:
                        return Math.Pow(y, 1 / x);

                    case ItemType.SQUARE_ROOT:
                        return Math.Sqrt(x);

                    case ItemType.MULTIPLY:
                        return x * y;

                    case ItemType.DIVIDE:
                        return x / y;

                    case ItemType.ADD:
                        return x + y;

                    case ItemType.SUBTRACT:
                        return x - y;

                    default:
                        return 0;
                }
            }
        }

        private double memory;
        private double answer;

        public Calculator()
        {
            memory = 0;
            answer = 0;
        }

        // Fill the list of items, meaning operations (like multiply), and numbers.
        private List<Item> Interpret(string input)
        {
            List<Item> items = new List<Item>(); // The list of items.
            input += ' '; // Empty character at the end so things that rely on the next run of the loop will work.
            string numberString = ""; // Temporary string to store numbers in before adding them to the items list.

            foreach (char c in input)
            {
                // If this character is part of a number, add it to the string.
                if (Char.IsDigit(c) || c == '.' || c == ',')
                {
                    numberString += c;
                }
                // If we reached the end of this number on the previous character, add the number to the list of items and clear the temporary string.
                else if (numberString.Length != 0)
                {
                    // Convert the temporary string into a number.
                    double number = double.Parse(numberString, CultureInfo.InvariantCulture.NumberFormat);

                    // Add the number to the list of items.
                    Item item = new Item(number);
                    items.Add(item);

                    // Reset the temporary string.
                    numberString = "";
                }

                // If the current character is an action, add that to the list of items.
                switch (c)
                {
                    case '^':
                        items.Add(new Item(ItemType.POWER));
                        break;

                    case 'v':
                        items.Add(new Item(ItemType.ROOT));
                        break;

                    case '*':
                        items.Add(new Item(ItemType.MULTIPLY));
                        break;

                    case '/':
                        items.Add(new Item(ItemType.DIVIDE));
                        break;

                    case '+':
                        items.Add(new Item(ItemType.ADD));
                        break;

                    case '-':
                        items.Add(new Item(ItemType.SUBTRACT));
                        break;

                    default:
                        break;
                }
            }

            return items;
        }

        public double Compute(string input)
        {
            // Fill the list of items based on the input string.
            List<Item> items = Interpret(input);

            // If the list is empty, the user used incorrect syntax.
            if (items.Count < 1) return 0;

            // Keep repeating these steps until the list of items has been simplified to one.
            while (items.Count > 1)
            {
                // Temporary variable to store the item highest in the math hiearchy.
                ItemType highestType = ItemType.NUMBER;

                // Temporary variable to store the position of the action to be exectured in the list.
                int itemPosition = -1;

                // For every item still in the list, act based on what type it is.
                for (int i = 0; i < items.Count; i++)
                {
                    // If highestAction is already the highest in the math hiearchy, don't bother checking the rest of the list.
                    if (highestType == ItemType.POWER) break;

                    // If this item is higher in the math hiearchy, make it the new highest action.
                    if (items[i].type > highestType)
                    {
                        highestType = items[i].type;
                        itemPosition = i;
                    }
                }

                // Temporary variables to make the code more readable.
                Item action = items[itemPosition];
                double numX = items[itemPosition - 1].number;
                double numY = items[itemPosition + 1].number;

                // Execute the action and store the result after the second number in the list.
                items.Insert(itemPosition + 2, new Item(action.Compute(numX, numY)));

                // Remove the action and its numbers from the list. The order here matters.
                // If we removed the first item first, then everything after it would shift.
                items.RemoveAt(itemPosition + 1);
                items.RemoveAt(itemPosition);
                items.RemoveAt(itemPosition - 1);
            }

            // Store the answer and return it.
            answer = items[0].number;
            return answer;
        }
    }
}
