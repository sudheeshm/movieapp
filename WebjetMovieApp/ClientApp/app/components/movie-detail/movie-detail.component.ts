import { Component, OnInit, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute, ParamMap } from '@angular/router';

@Component({
    selector: 'movie-detail',
    templateUrl: './movie-detail.component.html',
    styleUrls: ['../app/app.component.css']
})

export class MovieDetailComponent implements OnInit
{
    private movieId: any;
    private moviedetail: any;
    private movieprices: any;

    constructor(private route: ActivatedRoute, private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        console.log("movie-detail constructor() called");
    }

    ngOnInit() {
        console.log("movie-detail ngOnInit() called");
        //let id = this.route.snapshot.paramMap.get('id');
        this.route.paramMap.subscribe((params: ParamMap) => {
            let id = params.get('id');
            this.movieId = id;
        });

        this.http.get(this.baseUrl + 'api/movie?id=' + this.movieId.toString()).subscribe(result => {
            this.movieprices = result.json() as MoviePrice;

            console.log(this.movieprices.toString);
        }, error => console.error(error));
    }
}

export interface MovieDetail {
    id: string;
    title: string;
    year: string;
    type: string;
    poster: string;
    rated: string;
    released: string;
    runtime: string;
    director: string;
    writer: string;
    actors: string;
    plot: string;
    language: string;
    country: string;
    awards: string;
    metascore: string;
    rating: string;
    votes: string;
    pricedetail: MoviePrice[];
}
export interface MoviePrice {
    provider: string;
    price: string;
}