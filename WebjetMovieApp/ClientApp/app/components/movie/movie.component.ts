import { Component, OnInit, Input } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';
import { Inject, EventEmitter } from '@angular/core';

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
        http.get(baseUrl + 'api/movies?page=' + this.p.toString()).subscribe(result => {
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

export interface Movie {
    id: string;
    title: string;
    year: string;
    type: string;
    poster: string;
}




export const MOVIES: Movie[] = [
    { "id": "1", "title": "Movie1", "year": "200", "type": "s", "poster" : "" },
    { "id": "2", "title": "Movie2", "year": "200", "type": "s", "poster" : "" },
    { "id": "3", "title": "Movie3", "year": "200", "type": "s", "poster" : "" },
    { "id": "4", "title": "Movie4", "year": "200", "type": "s", "poster" : "" },
    { "id": "5", "title": "Movie5", "year": "200", "type": "s", "poster" : "" },
    { "id": "6", "title": "Movie6", "year": "200", "type": "s", "poster" : "" },
    { "id": "7", "title": "Movie7", "year": "200", "type": "s", "poster" : "" },
 ]
