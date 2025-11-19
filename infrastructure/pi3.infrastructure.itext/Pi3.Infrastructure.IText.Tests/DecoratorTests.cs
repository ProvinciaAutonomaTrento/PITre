// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.Services.File.Decorators;
using Pi3.Infrastructure.IText.Decorator;
using Pi3.Infrastructure.IText.Decorator.Services;
using System;
using System.Drawing;

namespace Pi3.Infrastructure.IText.Tests
{
    public class Tests
    {
        ServiceProvider _serviceProvider = null!;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddPi3Core()
                .AddInfrastructureITextFileDecorator()
                .BuildServiceProvider();
        }

        [Test, Ignore("verificare")]
        public async Task TestDecoration()
        {
            var decorator = this._serviceProvider.GetService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.cqrs_documents,
                    new FileDecoratorInstructions(
                            new TextLayer()
                            {
                                Text = "Layer 1",
                                //FontName = "Arial",
                                FontName = "COURIER",
                                FontSize = 12f,
                                FontStyle = TextLayerFontStylesEnum.Bold,
                                Position = LayerDefaultPositionsEnum.MiddleLeft,
                                Rotation = LayerRotationsEnum.Degrees90,
                                FontForeColor = new TextLayerFontRgbColor()
                                {
                                    R = 51,
                                    G = 51,
                                    B = 153
                                }
                            },
                            new TextLayer()
                            {
                                Text = "Layer 2",
                                FontName = "Times",
                                FontSize = 21f,
                                FontStyle = TextLayerFontStylesEnum.Italic,
                                Position = LayerDefaultPositionsEnum.MiddleRight,
                                Rotation = LayerRotationsEnum.Degrees90,
                                FontForeColor = new TextLayerFontRgbColor()
                                {
                                    R = 255,
                                    G = 0,
                                    B = 0
                                },
                                PageNumbersToApplyLayer = new int[5] { 1, 3, 5, 7, 9 }
                            }
                        ),
                 FileDecoratorOutputFormatsEnum.ToPdf);

            Assert.Pass();
        }

        [Test]
        public async Task TestDecorationHorizontal()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.report_horizontal,
                    new FileDecoratorInstructions(
                            ////new TextLayer()
                            ////{
                            ////    Text = "Layer 1",
                            ////    FontName = "Arial",
                            ////    FontSize = 12f,
                            ////    FontStyle = TextLayerFontStylesEnum.Bold,
                            ////    Position = LayerDefaultPositionsEnum.MiddleLeft,
                            ////    Rotation = LayerRotationsEnum.Degrees90,
                            ////    FontForeColor = new TextLayerFontRgbColor()
                            ////    {
                            ////        R = 51,
                            ////        G = 51,
                            ////        B = 153
                            ////    }
                            ////},
                            ////new TextLayer()
                            ////{
                            ////    Text = "Layer 2",
                            ////    FontName = "Times",
                            ////    FontSize = 21f,
                            ////    FontStyle = TextLayerFontStylesEnum.Italic,
                            ////    Position = LayerDefaultPositionsEnum.MiddleRight,
                            ////    Rotation = LayerRotationsEnum.Degrees90,
                            ////    FontForeColor = new TextLayerFontRgbColor()
                            ////    {
                            ////        R = 255,
                            ////        G = 0,
                            ////        B = 0
                            ////    },
                            ////    PageNumbersToApplyLayer = new int[5] { 1, 3, 5, 7, 9 }
                            ////},
                            ////new TextLayer()
                            ////{
                            ////    Text = "Layer 3",
                            ////    FontName = "Arial",
                            ////    FontSize = 12f,
                            ////    FontStyle = TextLayerFontStylesEnum.Italic,
                            ////    Position = LayerDefaultPositionsEnum.TopLeft,
                            ////    FontForeColor = new TextLayerFontRgbColor()
                            ////    {
                            ////        R = 0,
                            ////        G = 255,
                            ////        B = 0
                            ////    }
                            ////},
                            ////new TextLayer()
                            ////{
                            ////    Text = "Layer 4",
                            ////    FontName = "Arial",
                            ////    FontSize = 12f,
                            ////    FontStyle = TextLayerFontStylesEnum.Italic,
                            ////    Position = LayerDefaultPositionsEnum.TopRight,
                            ////    FontForeColor = new TextLayerFontRgbColor()
                            ////    {
                            ////        R = 0,
                            ////        G = 255,
                            ////        B = 0
                            ////    }
                            ////},
                            //new TextLayer()
                            //{
                            //    Text = "Layer Custom",
                            //    FontName = "Arial",
                            //    FontSize = 12f,
                            //    FontStyle = TextLayerFontStylesEnum.Italic,
                            //    CustomPosition = new System.Drawing.Point { X = 0, Y = 0 },
                            //    FontForeColor = new TextLayerFontRgbColor()
                            //    {
                            //        R = 0,
                            //        G = 255,
                            //        B = 0
                            //    }
                            //},
                            new TextLayer()
                            {
                                Text = "Layer Courier",
                                FontName = "Courier",
                                FontSize = 12f,
                                FontStyle = TextLayerFontStylesEnum.Italic,
                                CustomPosition = new System.Drawing.Point { X = 0, Y = 0 },
                                FontForeColor = new TextLayerFontRgbColor()
                                {
                                    R = 0,
                                    G = 255,
                                    B = 0
                                }
                            },
                              new TextLayer()
                              {
                                  Text = "140509685  17/06/2024",
                                  FontName = "COURIER",
                                  FontSize = 10,
                                  //FontStyle = TextLayerFontStylesEnum.Bold,
                                  Position = null,
                                  Rotation = LayerRotationsEnum.None,
                                  FontForeColor = new TextLayerFontRgbColor()
                                  {
                                      R = 0,
                                      G = 0,
                                      B = 153
                                  },
                                  PageNumbersToApplyLayer = new int[1] { 1 },
                                  CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                              }

                        ),
                 FileDecoratorOutputFormatsEnum.ToPdf);


            Assert.True(decoration != null);
        }

        [Test]
        public async Task TestDecorationHorizontal2()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.landscape,
                    new FileDecoratorInstructions(
                              ////new TextLayer()
                              ////{
                              ////    Text = "Layer 1",
                              ////    FontName = "Arial",
                              ////    FontSize = 12f,
                              ////    FontStyle = TextLayerFontStylesEnum.Bold,
                              ////    Position = LayerDefaultPositionsEnum.MiddleLeft,
                              ////    Rotation = LayerRotationsEnum.Degrees90,
                              ////    FontForeColor = new TextLayerFontRgbColor()
                              ////    {
                              ////        R = 51,
                              ////        G = 51,
                              ////        B = 153
                              ////    }
                              ////},
                              ////new TextLayer()
                              ////{
                              ////    Text = "Layer 2",
                              ////    FontName = "Times",
                              ////    FontSize = 21f,
                              ////    FontStyle = TextLayerFontStylesEnum.Italic,
                              ////    Position = LayerDefaultPositionsEnum.MiddleRight,
                              ////    Rotation = LayerRotationsEnum.Degrees90,
                              ////    FontForeColor = new TextLayerFontRgbColor()
                              ////    {
                              ////        R = 255,
                              ////        G = 0,
                              ////        B = 0
                              ////    },
                              ////    PageNumbersToApplyLayer = new int[5] { 1, 3, 5, 7, 9 }
                              ////},
                              ////new TextLayer()
                              ////{
                              ////    Text = "Layer 3",
                              ////    FontName = "Arial",
                              ////    FontSize = 12f,
                              ////    FontStyle = TextLayerFontStylesEnum.Italic,
                              ////    Position = LayerDefaultPositionsEnum.TopLeft,
                              ////    FontForeColor = new TextLayerFontRgbColor()
                              ////    {
                              ////        R = 0,
                              ////        G = 255,
                              ////        B = 0
                              ////    }
                              ////},
                              ////new TextLayer()
                              ////{
                              ////    Text = "Layer 4",
                              ////    FontName = "Arial",
                              ////    FontSize = 12f,
                              ////    FontStyle = TextLayerFontStylesEnum.Italic,
                              ////    Position = LayerDefaultPositionsEnum.TopRight,
                              ////    FontForeColor = new TextLayerFontRgbColor()
                              ////    {
                              ////        R = 0,
                              ////        G = 255,
                              ////        B = 0
                              ////    }
                              ////},
                              new TextLayer()
                              {
                                  Text = "140509685  17/06/2024",
                                  FontName = "COURIER",
                                  FontSize = 10,
                                  //FontStyle = TextLayerFontStylesEnum.Bold,
                                  Position = null,
                                  Rotation = LayerRotationsEnum.None,
                                  FontForeColor = new TextLayerFontRgbColor()
                                  {
                                      R = 0,
                                      G = 0,
                                      B = 153
                                  },
                                  PageNumbersToApplyLayer = new int[1] { 1 },
                                  CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                              }

                        ),
                 FileDecoratorOutputFormatsEnum.ToPdf);

            Assert.True(decoration != null);
        }

        [Test]
        public async Task TestPdfaFile()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var result = await decorator.Decorate(
                ".pdf",
                Files.pdfa,
                new FileDecoratorInstructions(
                    new TextLayer()
                    {
                        Text = "140509685  17/06/2024",
                        FontName = "COURIER",
                        FontSize = 10,
                        //FontStyle = TextLayerFontStylesEnum.Bold,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 0,
                            G = 0,
                            B = 153
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                    }

                ),
             FileDecoratorOutputFormatsEnum.ToPdf);
        }

        [Test]
        public async Task TestOrizzontaleAltoSinistra()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.Report_26_06_2024,
                    new FileDecoratorInstructions(
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                      //CustomPosition = new System.Drawing.Point { Y = 1, X = 1 },
                  }
              ),
                 FileDecoratorOutputFormatsEnum.ToPdf);


            Assert.True(decoration != null);
        }

        [Test]
        public async Task TestOrizzontaleAltoDestra()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.Report_26_06_2024,
                    new FileDecoratorInstructions(
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 30, X = 500 },
                  }
              ),
                 FileDecoratorOutputFormatsEnum.ToPdf);


            Assert.True(decoration != null);
        }

        [Test]
        public async Task TestOrizzontaleBassoSinistra()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.Report_26_06_2024,
                    new FileDecoratorInstructions(
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 400, X = 15 },
                  }
              ),
                 FileDecoratorOutputFormatsEnum.ToPdf);


            Assert.True(decoration != null);
        }

        [Test]
        public async Task TestOrizzontaleBassoDestra()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.Report_26_06_2024,
                    new FileDecoratorInstructions(
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 595, X = 400 },
                  }
              ),
                 FileDecoratorOutputFormatsEnum.ToPdf);

            Assert.True(decoration != null);
        }

        [Test]
        public async Task TestOrizzontaleBase()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.cqrs_documents,
                    new FileDecoratorInstructions(
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 400, X = 50 },
                  }
              ),
                 FileDecoratorOutputFormatsEnum.ToPdf);
            Assert.True(decoration != null);
        }

        [Test]
        public async Task TestOrizzontaleLandscapeFull()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.Report_26_06_2024,
                    new FileDecoratorInstructions(
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024 ASX",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                      //CustomPosition = new System.Drawing.Point { Y = 1, X = 1 },
                  },
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024 ADX",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 30, X = 650 },
                  },
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024 BSX",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 550, X = 15 },
                  },
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024 BDX",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 550, X = 650 },
                  }
              ),
                 FileDecoratorOutputFormatsEnum.ToPdf);

            Assert.True(decoration != null);
        }

        [Test]
        public async Task TestOrizzontalePortraitFull()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.cqrs_documents,
                    new FileDecoratorInstructions(
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024 ASX",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                      //CustomPosition = new System.Drawing.Point { Y = 1, X = 1 },
                  },
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024 ADX",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 30, X = 450 },
                  },
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024 BSX",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 770, X = 15 },
                  },
                  new TextLayer()
                  {
                      Text = "140509685  17/06/2024 BDX",
                      FontName = "COURIER",
                      FontSize = 10,
                      FontStyle = TextLayerFontStylesEnum.Regular,
                      Position = null,
                      Rotation = LayerRotationsEnum.None,
                      FontForeColor = new TextLayerFontRgbColor()
                      {
                          R = 255,
                          G = 0,
                          B = 0
                      },
                      PageNumbersToApplyLayer = new int[1] { 1 },
                      CustomPosition = new System.Drawing.Point { Y = 770, X = 450 },
                  }

              ),
                 FileDecoratorOutputFormatsEnum.ToPdf);

            Assert.True(decoration != null);
            File.WriteAllBytes("C:\\temp\\itext.pdf", decoration.Content);
        }

        [Test]
        public async Task TestReverseFile()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.Landscape270,
                    new FileDecoratorInstructions(
                        new TextLayer()
                        {
                            Text = "140509685  17/06/2024",
                            FontName = "COURIER",
                            FontSize = 10,
                            //FontStyle = TextLayerFontStylesEnum.Bold,
                            Position = null,
                            Rotation = LayerRotationsEnum.None,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 0,
                                G = 0,
                                B = 153
                            },
                            PageNumbersToApplyLayer = new int[1] { 1 },
                            CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                        }

                    ),
                 FileDecoratorOutputFormatsEnum.ToPdf);
        }

        [Test]
        public async Task TestReverse2File()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration =
                await decorator.Decorate(
                    ".pdf",
                    Files._142076640,
                    new FileDecoratorInstructions(
                        new TextLayer()
                        {
                            Text = "140509685  17/06/2024",
                            FontName = "COURIER",
                            FontSize = 10,
                            //FontStyle = TextLayerFontStylesEnum.Bold,
                            Position = null,
                            Rotation = LayerRotationsEnum.None,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 0,
                                G = 0,
                                B = 153
                            },
                            PageNumbersToApplyLayer = new int[1] { 1 },
                            CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                        }

                    ),
                 FileDecoratorOutputFormatsEnum.ToPdf);
        }

        [Test]
        public async Task TestAltoBassoSx()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var decoration =
                await decorator.Decorate(
                    ".pdf",
                    Files.Alto_Basso_Sx,
                    new FileDecoratorInstructions(
                        new TextLayer()
                        {
                            Text = "140509685  17/06/2024" + Environment.NewLine + "140509685  17/06/2024",
                            FontName = "COURIER",
                            FontSize = 10,
                            //FontStyle = TextLayerFontStylesEnum.Bold,
                            Position = null,
                            Rotation = LayerRotationsEnum.None,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 0,
                                G = 0,
                                B = 153
                            },
                            PageNumbersToApplyLayer = new int[1] { 1 },
                            CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                        },
                        new TextLayer()
                        {
                            Text = "140649389  03/10/2024 10:38:07",
                            FontName = "HELVETICA_BOLD",
                            //FontName = "COURIER",
                            FontSize = 10,
                            FontStyle = TextLayerFontStylesEnum.Regular,
                            Position = null,
                            Rotation = LayerRotationsEnum.None,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 255,
                                G = 0,
                                B = 0
                            },
                            PageNumbersToApplyLayer = new int[1] { 1 },
                            CustomPosition = new System.Drawing.Point { Y = 775, X = 400 }
                        },
                        new TextLayer()
                        {
                            Text = "140649389  03/10/2024 10:38:07",
                            FontName = "HELVETICA_BOLD",
                            //FontName = "COURIER",
                            FontSize = 10,
                            FontStyle = TextLayerFontStylesEnum.Regular,
                            Position = null,
                            Rotation = LayerRotationsEnum.None,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 255,
                                G = 0,
                                B = 0
                            },
                            PageNumbersToApplyLayer = new int[1] { 1 },
                            CustomPosition = new System.Drawing.Point { Y = 775, X = 15 }
                        }

                    ),
                 FileDecoratorOutputFormatsEnum.ToPdf);
        }

        public static IEnumerable<TestCaseData> DecorationTestCases
        {
            get
            {
                yield return new TestCaseData("aspose-1.pdf", Files.Alto_Basso_Sx);
                yield return new TestCaseData("aspose-2.pdf", Files._142076640);
                yield return new TestCaseData("aspose-3.pdf", Files.Landscape270);
                yield return new TestCaseData("aspose-4.pdf", Files.cqrs_documents);
                yield return new TestCaseData("aspose-5.pdf", Files.Report_26_06_2024);

                yield return new TestCaseData("aspose-md-1.pdf", Files.md_142873019);
                yield return new TestCaseData("aspose-md-2.pdf", Files.md_DestraCentrale);

                yield return new TestCaseData("aspose-se-1.pdf", Files.se_20240914);
                yield return new TestCaseData("aspose-se-2.pdf", Files.se_ConErrore);
                yield return new TestCaseData("aspose-se-3.pdf", Files.se_ConErroreFirmato);

                yield return new TestCaseData("aspose-ss-1.pdf", Files.ss_Doc006);
                yield return new TestCaseData("aspose-ss-2.pdf", Files.ss_Doc007);
                yield return new TestCaseData("aspose-ss-3.pdf", Files.ss_doc03869420240902171921);
                yield return new TestCaseData("firma-pades_2.pdf", Files.firma_pades_2);
                yield return new TestCaseData("email.pdf", Files.email);
                yield return new TestCaseData("big-one.pdf", Files.bog_one);
                yield return new TestCaseData("another-big-one.pdf", Files.strategici);
                yield return new TestCaseData("metaPagina.pdf", Files.metaPagina);
                yield return new TestCaseData("tecnoitalia.pdf", Files.tecnoitalia);

                yield return new TestCaseData("report_horizontal.pdf", Files.report_horizontal);
                yield return new TestCaseData("landscape.pdf", Files.landscape);
                yield return new TestCaseData("pdfa.pdf", Files.pdfa);
                yield return new TestCaseData("verbale.pdf", Files.verbale);
            }
        }

        [Test, TestCaseSource(nameof(DecorationTestCases))]
        public async Task TotalTest(string fileName, byte[] content)
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();
            var asService = decorator as ITextFileDecoratorService;
            asService.FileName = fileName;
            var decoration =
                await decorator.Decorate(
                    ".pdf",
                    content,
                    GetDecoratorSets(),
                 FileDecoratorOutputFormatsEnum.ToPdf);
            Assert.True(decoration != null);
            Assert.True(decoration.Content != null);

            File.WriteAllBytes(Path.Combine(TestContext.CurrentContext.TestDirectory, fileName), decoration.Content);
        }

        private FileDecoratorInstructions GetDecoratorSets()
        {
            var testo = "PAT_TEST/RFD319-18/10/2024-0012540|A\r\r\rverifica didascalia\rENTE CERTIFICATORE: InfoCert\rQualified Electronic� Signature\rCA 3 CL\rSN CERTIFICATO: 3D3C4\rVALIDO DA: 14/03/2024 13:40:56\rVALIDO AL: 13/03/2027 23:00:00\rFIRMATARI: EMANUELA PANICI\r\r\rDocumento firmato\relettronicamente da:\rEmanuela Panici (Segreteria\rDipartimento Organizzazione\rpersonale e affari generali)\ril 18/10/2024 15:20:44";
            return new FileDecoratorInstructions(
                new TextLayer()
                {
                    Text = "140509685  17/06/2024" + Environment.NewLine + "140509685  17/06/2024 TL",
                    FontName = "COURIER",
                    FontSize = 10,
                    //FontStyle = TextLayerFontStylesEnum.Bold,
                    Position = null,
                    Rotation = LayerRotationsEnum.None,
                    FontForeColor = new TextLayerFontRgbColor()
                    {
                        R = 0,
                        G = 0,
                        B = 153
                    },
                    PageNumbersToApplyLayer = new int[1] { 1 },
                    CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                }
                , new TextLayer()
                {
                    Text = "140509685  17/06/2024 TR",
                    FontName = "COURIER_BOLD",
                    FontSize = 12,
                    //FontStyle = TextLayerFontStylesEnum.Bold,
                    Position = null,
                    Rotation = LayerRotationsEnum.None,
                    FontForeColor = new TextLayerFontRgbColor()
                    {
                        R = 0,
                        G = 0,
                        B = 153
                    },
                    PageNumbersToApplyLayer = new int[1] { 1 },
                    CustomPosition = new System.Drawing.Point { Y = 30, X = 400 },
                }
                , new TextLayer()   // Top Right
                {
                    Text = "140649389  03/10/2024 10:38:07 BR",
                    FontName = "HELVETICA",
                    //FontName = "COURIER",
                    FontSize = 10,
                    FontStyle = TextLayerFontStylesEnum.Regular,
                    Position = null,
                    Rotation = LayerRotationsEnum.None,
                    FontForeColor = new TextLayerFontRgbColor()
                    {
                        R = 255,
                        G = 0,
                        B = 0
                    },
                    PageNumbersToApplyLayer = new int[1] { 1 },
                    CustomPosition = new System.Drawing.Point { Y = 775, X = 400 }
                }
                , new TextLayer()           // Bottom Right
                {
                    Text = "140649389  03/10/2024 10:38:07 BL",
                    FontName = "HELVETICA_BOLD",
                    //FontName = "COURIER",
                    FontSize = 10,
                    FontStyle = TextLayerFontStylesEnum.Regular,
                    Position = null,
                    Rotation = LayerRotationsEnum.None,
                    FontForeColor = new TextLayerFontRgbColor()
                    {
                        R = 255,
                        G = 0,
                        B = 0
                    },
                    PageNumbersToApplyLayer = new int[1] { 1 },
                    CustomPosition = new System.Drawing.Point { Y = 775, X = 15 }
                }
                , new TextLayer()
                {
                    CustomPosition = new Point(Int32.Parse("200"), Int32.Parse("30")),
                    Rotation = LayerRotationsEnum.None,
                    Position = null,
                    FontForeColor = new TextLayerFontRgbColor()
                    {
                        R = 0,
                        G = 0,
                        B = 0
                    },
                    FontName = "COURIER",
                    FontSize = 10,
                    Text = testo,
                    PageNumbersToApplyLayer = new int[1] { 1 }
                }
            );
        }

        [Test]
        public async Task TestMultiLine()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var testo = "PAT_TEST/RFD319-18/10/2024-0012540|A\r\r\rverifica didascalia\rENTE CERTIFICATORE: InfoCert\rQualified Electronic Signature\rCA 3 CL\rSN CERTIFICATO: 3D3C4\rVALIDO DA: 14/03/2024 13:40:56\rVALIDO AL: 13/03/2027 23:00:00\rFIRMATARI: EMANUELA PANICI\r\r\rDocumento firmato\relettronicamente da:\rEmanuela Panici (Segreteria\rDipartimento Organizzazione\rpersonale e affari generali)\ril 18/10/2024 15:20:44";

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.firma_pades_2,
                    new FileDecoratorInstructions(
                        new TextLayer()
                        {
                            CustomPosition = new Point(Int32.Parse("400"), Int32.Parse("30")),
                            Rotation = LayerRotationsEnum.None,
                            Position = null,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 0,
                                G = 0,
                                B = 0
                            },
                            FontName = "COURIER",
                            FontSize = 10,
                            Text = testo,
                            PageNumbersToApplyLayer = new int[1] { 1 }
                        }
                    ),
                 FileDecoratorOutputFormatsEnum.ToPdf);
            //File.WriteAllBytes("C:\\temp\\multiline.pdf", decoration.Content);
        }

        [Test]
        public async Task TestEmail()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var testo = "PAT_TEST/RFD319-18/10/2024-0012540|A\r\r\rverifica didascalia\rENTE CERTIFICATORE: InfoCert\rQualified Electronic Signature\rCA 3 CL\rSN CERTIFICATO: 3D3C4\rVALIDO DA: 14/03/2024 13:40:56\rVALIDO AL: 13/03/2027 23:00:00\rFIRMATARI: EMANUELA PANICI\r\r\rDocumento firmato\relettronicamente da:\rEmanuela Panici (Segreteria\rDipartimento Organizzazione\rpersonale e affari generali)\ril 18/10/2024 15:20:44";

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.email,
                    new FileDecoratorInstructions(
                        new TextLayer()
                        {
                            CustomPosition = new Point(Int32.Parse("400"), Int32.Parse("30")),
                            Rotation = LayerRotationsEnum.None,
                            Position = null,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 0,
                                G = 0,
                                B = 0
                            },
                            FontName = "COURIER",
                            FontSize = 10,
                            Text = testo,
                            PageNumbersToApplyLayer = new int[1] { 1 }
                        }
                    ),
                 FileDecoratorOutputFormatsEnum.ToPdf);
            //File.WriteAllBytes("C:\\temp\\email.pdf", decoration.Content);
        }

        [Test]
        public async Task TestBigOne()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var testo = "PAT_TEST/RFD319-18/10/2024-0012540|A\r\r\rverifica didascalia\rENTE CERTIFICATORE: InfoCert\rQualified Electronic Signature\rCA 3 CL\rSN CERTIFICATO: 3D3C4\rVALIDO DA: 14/03/2024 13:40:56\rVALIDO AL: 13/03/2027 23:00:00\rFIRMATARI: EMANUELA PANICI\r\r\rDocumento firmato\relettronicamente da:\rEmanuela Panici (Segreteria\rDipartimento Organizzazione\rpersonale e affari generali)\ril 18/10/2024 15:20:44";

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.bog_one,
                    new FileDecoratorInstructions(
                        new TextLayer()
                        {
                            CustomPosition = new Point(Int32.Parse("400"), Int32.Parse("30")),
                            Rotation = LayerRotationsEnum.None,
                            Position = null,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 0,
                                G = 0,
                                B = 0
                            },
                            FontName = "COURIER",
                            FontSize = 10,
                            Text = testo,
                            PageNumbersToApplyLayer = new int[1] { 1 }
                        }
                    ),
                 FileDecoratorOutputFormatsEnum.ToPdf);
            //File.WriteAllBytes("C:\\temp\\big-one.pdf", decoration.Content);
        }

        [Test]
        public async Task TestStrategici()
        {
            var decorator = this._serviceProvider.GetRequiredService<IFileDecoratorService>();

            var testo = "140509685  17/06/2024 TR"; // "PAT_TEST/RFD319-18/10/2024-0012540|A\r\r\rverifica didascalia\rENTE CERTIFICATORE: InfoCert\rQualified Electronic Signature\rCA 3 CL\rSN CERTIFICATO: 3D3C4\rVALIDO DA: 14/03/2024 13:40:56\rVALIDO AL: 13/03/2027 23:00:00\rFIRMATARI: EMANUELA PANICI\r\r\rDocumento firmato\relettronicamente da:\rEmanuela Panici (Segreteria\rDipartimento Organizzazione\rpersonale e affari generali)\ril 18/10/2024 15:20:44";
            //     FileDecoratorOutputFormatsEnum.ToPdf);
            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.strategici,
                    new FileDecoratorInstructions(
                        new TextLayer()
                        {
                            CustomPosition = new Point(Int32.Parse("400"), Int32.Parse("30")),
                            Rotation = LayerRotationsEnum.None,
                            Position = null,
                            FontForeColor = new TextLayerFontRgbColor()
                            {
                                R = 0,
                                G = 0,
                                B = 0
                            },
                            FontName = "COURIER",
                            FontSize = 10,
                            Text = testo,
                            PageNumbersToApplyLayer = new int[1] { 1 }
                        }
                    ),
                 FileDecoratorOutputFormatsEnum.ToPdf);
            //File.WriteAllBytes("C:\\temp\\another-doc.pdf", decoration.Content);
            //Assert.True(decoration != null);
        }

        [Test]
        public async Task TestDecoration2()
        {
            var decorator = this._serviceProvider.GetService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(
                    ".pdf",
                    Files.Report_21_06_2024,
                    new FileDecoratorInstructions(
                              new TextLayer()
                              {
                                  Text = "140509685  17/06/2024", // + Environment.NewLine + "testo a capo" + Environment.NewLine + "testo a capo" + Environment.NewLine + "testo a capo",
                                  FontName = "Courier",
                                  FontSize = 10,
                                  FontStyle = TextLayerFontStylesEnum.Bold,
                                  Position = null,
                                  Rotation = LayerRotationsEnum.None,
                                  FontForeColor = new TextLayerFontRgbColor()
                                  {
                                      R = 0,
                                      G = 0,
                                      B = 153
                                  },
                                  PageNumbersToApplyLayer = new int[1] { 1 },
                                  CustomPosition = new System.Drawing.Point { Y = 0, X = 400 },
                              },
                            new TextLayer()
                            {
                                Text = "140509685  17/06/2024" + Environment.NewLine + "testo a capo" + Environment.NewLine + "testo a capo" + Environment.NewLine + "testo a capo",
                                FontName = "Helvetica",
                                FontSize = 10,
                                FontStyle = TextLayerFontStylesEnum.Italic,
                                Position = null,
                                Rotation = LayerRotationsEnum.None,
                                FontForeColor = new TextLayerFontRgbColor()
                                {
                                    R = 0,
                                    G = 0,
                                    B = 153
                                },
                                PageNumbersToApplyLayer = new int[1] { 1 },
                                CustomPosition = new System.Drawing.Point { Y = 800, X = 800 },
                            }                       
                        ),
                 FileDecoratorOutputFormatsEnum.ToPdf);

            Assert.Pass();
        }

        [Test]
        public async Task TestMultiStep()
        {
            var testFileName = "multi-step.pdf";
            var testPath = TestContext.CurrentContext.TestDirectory;
            var testo = "PAT_TEST/RFD319-18/10/2024-0012540|A\r\r\rverifica didascalia\rENTE CERTIFICATORE: InfoCert\rQualified Electronic Signature\rCA 3 CL\rSN CERTIFICATO: 3D3C4\rVALIDO DA: 14/03/2024 13:40:56\rVALIDO AL: 13/03/2027 23:00:00\rFIRMATARI: EMANUELA PANICI\r\r\rDocumento firmato\relettronicamente da:\rEmanuela Panici (Segreteria\rDipartimento Organizzazione\rpersonale e affari generali)\ril 18/10/2024 15:20:44";

            File.WriteAllBytes( Path.Combine( testPath, testFileName), Files.verbale);

            var decorator = this._serviceProvider.GetService<IFileDecoratorService>();

            // step 1

            await print_decoration( testFileName, testFileName/*"multi-step-2.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()
                    {
                        Text = "140509685  17/06/2024" + Environment.NewLine + "140509685  17/06/2024 TL",
                        FontName = "COURIER",
                        FontSize = 10,
                        //FontStyle = TextLayerFontStylesEnum.Bold,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 0,
                            G = 0,
                            B = 153
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                    }
                )
            );

            await print_decoration(testFileName/*"multi-step-2.pdf"*/, testFileName/*"multi-step-3.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()
                    {
                        Text = "140509685  17/06/2024 TR",
                        FontName = "COURIER_BOLD",
                        FontSize = 12,
                        //FontStyle = TextLayerFontStylesEnum.Bold,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 0,
                            G = 0,
                            B = 153
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 30, X = 400 },
                    }
                )
            );

            await print_decoration(testFileName/*"multi-step-3.pdf"*/, testFileName/*"multi-step-4.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()   // Top Right
                    {
                        Text = "140649389  03/10/2024 10:38:07 BR",
                        FontName = "HELVETICA",
                        //FontName = "COURIER",
                        FontSize = 10,
                        FontStyle = TextLayerFontStylesEnum.Regular,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 255,
                            G = 0,
                            B = 0
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 775, X = 400 }
                    }
                )
            );

            await print_decoration(testFileName/*"multi-step-4.pdf"*/, testFileName/*"multi-step-5.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()           // Bottom Right
                    {
                        Text = "140649389  03/10/2024 10:38:07 BL",
                        FontName = "HELVETICA_BOLD",
                        //FontName = "COURIER",
                        FontSize = 10,
                        FontStyle = TextLayerFontStylesEnum.Regular,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 255,
                            G = 0,
                            B = 0
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 775, X = 15 }
                    }
                )
            );

            await print_decoration(testFileName/*"multi-step-5.pdf"*/, testFileName/*"multi-step-6.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()
                    {
                        CustomPosition = new Point(Int32.Parse("200"), Int32.Parse("30")),
                        Rotation = LayerRotationsEnum.None,
                        Position = null,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 0,
                            G = 0,
                            B = 0
                        },
                        FontName = "COURIER",
                        FontSize = 10,
                        Text = testo,
                        PageNumbersToApplyLayer = new int[1] { 1 }
                    }
                )
            );

            async Task print_decoration(string inputFileName, string outputFileName, FileDecoratorInstructions instructions) {
                var buffer = File.ReadAllBytes(Path.Combine(testPath, inputFileName));
                var decoration = await decorator.Decorate(
                        ".pdf",
                        buffer,
                        instructions,
                     FileDecoratorOutputFormatsEnum.ToPdf);
                File.WriteAllBytes(Path.Combine(testPath, outputFileName), decoration.Content);
            }

            //instructions = new FileDecoratorInstructions(
            //);
            //instructions = new FileDecoratorInstructions(
            //);
        }


        [Test, TestCaseSource(nameof(DecorationTestCases))]
        public async Task TestTotalMultiStep(string fileName, byte[] content)
        {
            var testFileName = fileName;
            var testPath = TestContext.CurrentContext.TestDirectory;
            var testo = "PAT_TEST/RFD319-18/10/2024-0012540|A\r\r\rverifica didascalia\rENTE CERTIFICATORE: InfoCert\rQualified Electronic Signature\rCA 3 CL\rSN CERTIFICATO: 3D3C4\rVALIDO DA: 14/03/2024 13:40:56\rVALIDO AL: 13/03/2027 23:00:00\rFIRMATARI: EMANUELA PANICI\r\r\rDocumento firmato\relettronicamente da:\rEmanuela Panici (Segreteria\rDipartimento Organizzazione\rpersonale e affari generali)\ril 18/10/2024 15:20:44";

            File.WriteAllBytes(Path.Combine(testPath, testFileName), content);

            var decorator = this._serviceProvider.GetService<IFileDecoratorService>();

            // step 1

            await print_decoration(testFileName, testFileName/*"multi-step-2.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()
                    {
                        Text = "140509685  17/06/2024" + Environment.NewLine + "140509685  17/06/2024 TL",
                        FontName = "COURIER",
                        FontSize = 10,
                        //FontStyle = TextLayerFontStylesEnum.Bold,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 0,
                            G = 0,
                            B = 153
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 30, X = 15 },
                    }
                )
            );

            await print_decoration(testFileName/*"multi-step-2.pdf"*/, testFileName/*"multi-step-3.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()
                    {
                        Text = "140509685  17/06/2024 TR",
                        FontName = "COURIER_BOLD",
                        FontSize = 12,
                        //FontStyle = TextLayerFontStylesEnum.Bold,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 0,
                            G = 0,
                            B = 153
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 30, X = 400 },
                    }
                )
            );

            await print_decoration(testFileName/*"multi-step-3.pdf"*/, testFileName/*"multi-step-4.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()   // Top Right
                    {
                        Text = "140649389  03/10/2024 10:38:07 BR",
                        FontName = "HELVETICA",
                        //FontName = "COURIER",
                        FontSize = 10,
                        FontStyle = TextLayerFontStylesEnum.Regular,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 255,
                            G = 0,
                            B = 0
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 775, X = 400 }
                    }
                )
            );

            await print_decoration(testFileName/*"multi-step-4.pdf"*/, testFileName/*"multi-step-5.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()           // Bottom Right
                    {
                        Text = "140649389  03/10/2024 10:38:07 BL",
                        FontName = "HELVETICA_BOLD",
                        //FontName = "COURIER",
                        FontSize = 10,
                        FontStyle = TextLayerFontStylesEnum.Regular,
                        Position = null,
                        Rotation = LayerRotationsEnum.None,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 255,
                            G = 0,
                            B = 0
                        },
                        PageNumbersToApplyLayer = new int[1] { 1 },
                        CustomPosition = new System.Drawing.Point { Y = 775, X = 15 }
                    }
                )
            );

            await print_decoration(testFileName/*"multi-step-5.pdf"*/, testFileName/*"multi-step-6.pdf"*/,
                new FileDecoratorInstructions(
                    new TextLayer()
                    {
                        CustomPosition = new Point(Int32.Parse("200"), Int32.Parse("30")),
                        Rotation = LayerRotationsEnum.None,
                        Position = null,
                        FontForeColor = new TextLayerFontRgbColor()
                        {
                            R = 0,
                            G = 0,
                            B = 0
                        },
                        FontName = "COURIER",
                        FontSize = 10,
                        Text = testo,
                        PageNumbersToApplyLayer = new int[1] { 1 }
                    }
                )
            );

            async Task print_decoration(string inputFileName, string outputFileName, FileDecoratorInstructions instructions)
            {
                var buffer = File.ReadAllBytes(Path.Combine(testPath, inputFileName));
                var decoration = await decorator.Decorate(
                        ".pdf",
                        buffer,
                        instructions,
                     FileDecoratorOutputFormatsEnum.ToPdf);
                File.WriteAllBytes(Path.Combine(testPath, outputFileName), decoration.Content);
            }
        }

        [Test, TestCaseSource(nameof(DecorationTestCases))]
        public async Task TestEmptyInstructions(string fileName, byte[] content)
        {
            var testFileName = fileName;
            var testPath = TestContext.CurrentContext.TestDirectory;

            var decorator = this._serviceProvider.GetService<IFileDecoratorService>();

            for(int i = 0; i < 10; i++)
            {
                var decoration = await decorator.Decorate(
                        ".pdf",
                        content,
                        new FileDecoratorInstructions(),
                     FileDecoratorOutputFormatsEnum.ToPdf);
                Assert.True(decoration != null);
                Assert.True(decoration.Metadata != null);
                Assert.True(decoration.Metadata.Where(itm => itm.Key == "document.Pages.Count").Select(k => k.Value).FirstOrDefault() != null);
            }
        }

        [Test]
        public async Task TestDecorationEmpty()
        {
            var decorator = this._serviceProvider.GetService<IFileDecoratorService>();

            var decoration = await decorator.Decorate(".pdf", Files.Alto_Basso_Sx, new FileDecoratorInstructions(), FileDecoratorOutputFormatsEnum.ToPdf);

            var decoration1 = await decorator.Decorate(".pdf", Files.Alto_Basso_Sx, new FileDecoratorInstructions(), FileDecoratorOutputFormatsEnum.ToPdf);

            var decoration2 = await decorator.Decorate(".pdf", Files.Alto_Basso_Sx, new FileDecoratorInstructions(), FileDecoratorOutputFormatsEnum.ToPdf);

            var decoration3 = await decorator.Decorate(".pdf", Files.Alto_Basso_Sx, new FileDecoratorInstructions(), FileDecoratorOutputFormatsEnum.ToPdf);
        }
    }
}