﻿using System;
using Autofac;
using MarkovChainGenerator.Views.ContentPages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MarkovChainGenerator
{
    public partial class App : Application
    {
        public static IContainer Container { get; set; }

        public App()
        {
            InitializeComponent();
            Container = AutoFacContainerBuilder.CreateContainer();
            //MainPage = new InitialView();
            //MainPage = new TestView();
            MainPage = new TwitterFeedView();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
