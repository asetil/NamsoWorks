import React, { Component } from 'react';
import HomeService from '../service/HomeService'

export class Home extends Component {
    static displayName = Home.name;
    constructor(props) {
        super(props);

        this.state = {
            sliderItems: []
        };

        HomeService.getSliderItems().then(data => {
            this.setState({
                sliderItems: data ? data.results : []
            });
        });
    }

    getSliderHtml() {
        return this.state.sliderItems.map(slider => {
            return (
                <div className={slider.id === 1 ? "carousel-item active" : "carousel-item"} key={slider.id}>
                    <div className="carousel-background"><img src={slider.imagePath} alt={slider.title} /></div>
                    <div className="carousel-container">
                        <div className="carousel-content">
                            <h2>{slider.title}</h2>
                            <p>{slider.description}</p>
                            <a href="#featured-services" className="btn-get-started scrollto">Get Started</a>
                        </div>
                    </div>
                </div>
            )
        });
    }

    render() {
        return (
            <React.Fragment>
                <section id="intro">
                    <div className="intro-container">
                        <div id="introCarousel" className="carousel  slide carousel-fade" data-ride="carousel">
                            <ol className="carousel-indicators"></ol>

                            <div className="carousel-inner" role="listbox">
                                {this.getSliderHtml()}
                            </div>

                            <a className="carousel-control-prev" href="#introCarousel" role="button" data-slide="prev">
                                <span className="carousel-control-prev-icon ion-chevron-left" aria-hidden="true"></span>
                                <span className="sr-only">Previous</span>
                            </a>

                            <a className="carousel-control-next" href="#introCarousel" role="button" data-slide="next">
                                <span className="carousel-control-next-icon ion-chevron-right" aria-hidden="true"></span>
                                <span className="sr-only">Next</span>
                            </a>

                        </div>
                    </div>
                </section>

                <section id="featured-services">
                    <div className="container">
                        <div className="row">

                            <div className="col-lg-4 box">
                                <i className="ion-ios-bookmarks-outline"></i>
                                <h4 className="title"><a href="">Lorem Ipsum Delino</a></h4>
                                <p className="description">Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident</p>
                            </div>

                            <div className="col-lg-4 box box-bg">
                                <i className="ion-ios-stopwatch-outline"></i>
                                <h4 className="title"><a href="">Dolor Sitema</a></h4>
                                <p className="description">Minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat tarad limino ata</p>
                            </div>

                            <div className="col-lg-4 box">
                                <i className="ion-ios-heart-outline"></i>
                                <h4 className="title"><a href="">Sed ut perspiciatis</a></h4>
                                <p className="description">Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur</p>
                            </div>

                        </div>
                    </div>
                </section>

                <main id="main">
                    <section id="about">
                        <div className="container">
                            <header className="section-header">
                                <h3>About Us</h3>
                                <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.</p>
                            </header>

                            <div className="row about-cols">
                                <div className="col-md-4 wow fadeInUp">
                                    <div className="about-col">
                                        <div className="img">
                                            <img src="/res/img/about-mission.jpg" alt="" className="img-fluid" />
                                            <div className="icon"><i className="ion-ios-speedometer-outline"></i></div>
                                        </div>
                                        <h2 className="title"><a href="#">Our Mission</a></h2>
                                        <p>
                                            Lorem ipsum dolor sit amet, consectetur elit, sed do eiusmod tempor ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
                        </p>
                                    </div>
                                </div>

                                <div className="col-md-4 wow fadeInUp" data-wow-delay="0.1s">
                                    <div className="about-col">
                                        <div className="img">
                                            <img src="/res/img/about-plan.jpg" alt="" className="img-fluid" />
                                            <div className="icon"><i className="ion-ios-list-outline"></i></div>
                                        </div>
                                        <h2 className="title"><a href="#">Our Plan</a></h2>
                                        <p>
                                            Sed ut perspiciatis unde omnis iste natus error sit voluptatem  doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.
                        </p>
                                    </div>
                                </div>

                                <div className="col-md-4 wow fadeInUp" data-wow-delay="0.2s">
                                    <div className="about-col">
                                        <div className="img">
                                            <img src="/res/img/about-vision.jpg" alt="" className="img-fluid" />
                                            <div className="icon"><i className="ion-ios-eye-outline"></i></div>
                                        </div>
                                        <h2 className="title"><a href="#">Our Vision</a></h2>
                                        <p>
                                            Nemo enim ipsam voluptatem quia voluptas sit aut odit aut fugit, sed quia magni dolores eos qui ratione voluptatem sequi nesciunt Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet.
                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                </main>

            </React.Fragment>
        );
    }
}
