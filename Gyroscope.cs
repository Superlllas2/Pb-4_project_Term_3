using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;

namespace GXPEngine
{
    public class Gyroscope
    {
        private SerialPort arduino;
        public float pitch;
        public float roll;
        private float avgPitch;
        private float avgRoll;
        
        private int rightCount;
        private int leftCount;
        private bool isRollWithinRange;

        private int upCount;
        private int downCount;
        private bool isPitchWithinRange;
        
        private Queue<float> pitchValues = new Queue<float>(5);
        private Queue<float> rollValues = new Queue<float>(5);

        public Gyroscope(string portName, int baudRate)
        {
            arduino = new SerialPort();
            arduino.PortName = portName;
            arduino.BaudRate = baudRate;
            arduino.RtsEnable = true;
            arduino.DtrEnable = true;
            arduino.Open();
        }

        public String GetInput()
        {
            return arduino.ReadExisting();
        }

        public void SerialPort_DataReceived()
        {
            while ( arduino.BytesToRead > 1 )
            {
                ProcessLine( arduino.ReadLine() );
            }
        }
        
        void ProcessLine( string data ) {

            if (data.StartsWith("YPR:"))
            {
                // This already trims semicolons, but it's good to ensure it's applied to each value
                var trimmedData = data.Substring(4).Trim(';').Trim();
                var values = trimmedData.Split(',');
                // Before parsing, trim each value to ensure no trailing semicolons
                if (values.Length == 3 &&
                    float.TryParse(values[0].Trim(';'), NumberStyles.Any, CultureInfo.InvariantCulture, out float yaw) &&
                    float.TryParse(values[1].Trim(';'), NumberStyles.Any, CultureInfo.InvariantCulture, out float pitch) &&
                    float.TryParse(values[2].Trim(';'), NumberStyles.Any, CultureInfo.InvariantCulture, out float roll))
                {
                    this.pitch = pitch;
                    this.roll = roll;
                    Do();
                }
                else
                {
                    Console.WriteLine("Failed to parse YPR values");
                }
            }
                
            else if (data.StartsWith("BTN1:Pressed;"))
            {
                // Handle button 1 press
                Console.WriteLine("Button 1 Pressed");
            }
            else if (data.StartsWith("BTN2:Pressed;"))
            {
                // Handle button 2 press
                Console.WriteLine("Button 2 Pressed");
            }
        }

        private void Do()
        {
            // Add the new pitch value to the queue, ensuring it only keeps the last 3 values
            if (pitchValues.Count == 3)
            {
                pitchValues.Dequeue(); // Remove the oldest value if the queue is full
            }

            if (rollValues.Count == 3)
            {
                rollValues.Dequeue();
            }
            // Add the new values
            pitchValues.Enqueue(pitch);
            rollValues.Enqueue(roll);


            // --- LEFT AND RIGHT ---
            if (roll > 25 && !isRollWithinRange)
            {
                rightCount++; // Increment only if roll enters the range for the first time
                isRollWithinRange = true; // Mark that roll is within the range
            }
            else if (roll <= 25 && roll >= -25)
            {
                // If roll is back outside the range, reset the flag so testCount can be incremented again in the future
                isRollWithinRange = false;
            } else if (roll < -25  && !isRollWithinRange)
            {
                leftCount++;
                isRollWithinRange = true;
            }
            // ----------------------
            
            // --- UP AND DOWN ---
            if (pitch > 15 && !isPitchWithinRange)
            {
                downCount++; 
                isPitchWithinRange = true; 
            }
            else if (pitch <= 15 && pitch >= -20)
            {
                isPitchWithinRange = false;
            } else if (pitch < -20 && !isPitchWithinRange)
            {
                upCount++;
                isPitchWithinRange = true;
            }
            // ----------------------

            
            // ---- TESTING ----
            // Console.WriteLine(pitch + " " + roll);
            // Console.WriteLine("Average pitch is: " + CalculateAverage(pitchValues));
            // Console.WriteLine("Average roll is: " + CalculateAverage(rollValues));
            // Console.WriteLine("Last roll is: " + roll);
            // Console.WriteLine("The ratio is: " + roll / CalculateAverage(rollValues));
            // Console.WriteLine("Right count: " + rightCount);
            // Console.WriteLine("Left count: " + leftCount);
            // Console.WriteLine("Average: " + rollValues.Average());
            // Console.WriteLine("Count up: " + upCount);
            // Console.WriteLine("Count down: " + downCount);
            // -----------------
        }
        
        
        private float CalculateAverage(Queue<float> values)
        {
            if (values.Count == 0) return 0; // Avoid division by zero
            return values.Average(); // Calculate and return the average of the buffered pitch values
        }
    }
}