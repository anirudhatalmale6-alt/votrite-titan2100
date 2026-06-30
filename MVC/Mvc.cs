// Product:	VotRite
// Module:  Mvc.cs
// Author:  Dmitriy Slipak

// 
// Copyright (c) 2017 - VOTRITE INTERNATIONAL LLC. All rights reserved.
// 
// THIS PRODUCT INCLUDES SOFTWARE AS A PART OF VOTRITE VOTING 
// SYSTEM DEVELOPED AT VOTRITE INTERNATIONAL LLC (http://www.votrite.com).
// THE SOURCE CODE FOR THIS PRODUCT IS SUBJECT TO THE VOTRITE INTERNATIONAL LLC 
// LICENSE. NO ANY PORTION OF THIS PRODUCT OR SOURCE CODE CAN BE 
// REDISTRIBUTED UNDER ANY CIRCUMSTANCES.
// 
using System;
using System.Collections.Generic;
using SpeechLib;

namespace VotRite.MVC
{
    /// <summary>
    /// Observer interface declaration and implementation
    /// </summary>
    interface IObserver: IDisposable
    {
        void Update(ISubject subj);
        // Jim Kapsis.
        //void Dispose();
    }

    /// <summary>
    /// Subject interface declaration and implementation
    /// </summary>
    interface ISubject: IDisposable
    {
        void AddSubscriber(IObserver obsvr);
        void RemoveSubscriber(IObserver obsvr);
        void Notify();
        // Jim Kapsis.
        // void Dispose();
    }

    /// <summary>
    /// Class Model declaration and implementation
    /// </summary>
    class Model : ISubject
    {
        public List<IObserver> observers;

        protected Model()
        {
            observers = new List<IObserver>();
        }

        // Jim Kapsis.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Jim Kapsis.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                observers.ForEach(obs => obs.Dispose());
                observers.Clear();
            }
        }

        public void AddSubscriber(IObserver obsvr)
        {
            observers.Add(obsvr);
        }

        public void RemoveSubscriber(IObserver obsvr)
        {
            observers.Remove(obsvr);
        }

        public void Notify()
        {
            foreach (IObserver observer in observers)
                observer.Update(this);
        }
    }

    /// <summary>
    /// Class VrController declaration and implementation
    /// </summary>
    class Controller : IObserver
    {
        protected Model model;
        private List<ISubject> subjects;
        
        protected Controller(Model m)
        {
            subjects = new List<ISubject>();
            model = m;

            Subscribe(model);
        }

        // Jim Kapsis.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Jim Kapsis.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                subjects.ForEach(sub => sub.Dispose());
                subjects.Clear();
                model.Dispose();
            }
        }

        public void Subscribe(ISubject subj)
        {
            subjects.Add(subj);
            subj.AddSubscriber(this);
        }

        public void Unsubscribe(ISubject subj)
        {
            subjects.Remove(subj);
            subj.RemoveSubscriber(this);
        }

        public virtual void Update(ISubject subj) {}
		
		public virtual void HandleKey(string key) {}
		
		public virtual void HandleTouch(int x, int y) {}

        public virtual void HandleSpeech(string recogWord) { }

        public virtual void HandleMouseUp() { }

        public virtual void HandleMouseDown_Left() { }
        public virtual void HandleMouseDown_Right() { }
    }

    /// <summary>
    /// Class VrView declaration and implementation
    /// </summary>
    class View : IObserver
    {
        protected Model model;
        protected Controller controller;
        private List<ISubject> subjects;
		
        public View(Model m, Controller c)
        {
            subjects = new List<ISubject>();
            model = m;
            controller = c;
			
			Subscribe(m);
        }

        // Jim Kapsis.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Jim Kapsis.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //subjects.ForEach(sub => sub.Dispose());
                subjects.Clear();
                //model.Dispose();
                //controller.Dispose();
            }
        }

        public void Subscribe(ISubject subj)
        {
            subjects.Add(subj);
            subj.AddSubscriber(this);
        }

        public void Unsubscribe(ISubject subj)
        {
            subjects.Remove(subj);
            subj.RemoveSubscriber(this);
        }

        public virtual void Update(ISubject subj) {}
		
		public virtual void ShowView() {}
    }
}