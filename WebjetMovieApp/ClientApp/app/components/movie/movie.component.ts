import { Component, OnInit, Input } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { Inject, EventEmitter } from '@angular/core';
import { Movie, MovieDetail, MoviePrice } from '../../../model/movie';

@Component({
    selector: 'movie-cards',
    templateUrl: './movie.component.html',
    styleUrls: ['../app/app.component.css']
})

export class MovieComponent implements OnInit{
    private movies: Movie[] = [];
    private p: number = 1;
    private pagesize: number = 4;
    private router: Router;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string, router: Router) {
        console.log("movie constructor() called");

        this.router = router;
        let page = "page=0&pagesize=10"; //this.p.toString()

        http.get(baseUrl + 'api/movies?' + page).subscribe(result => {
            this.movies = result.json() as Movie[];
        }, error => console.error(error));
    }

    ngOnInit() {
        console.log("movie onInit() called");
    }

    onClickMovie(movie: Movie) {
        console.log("clicked movie detail " + movie.id);
        this.router.navigate(['/movies', movie.id]);
    }

    pageChanged(page: number) {
         console.log("pageChanged " + page.toString());
    }
}
