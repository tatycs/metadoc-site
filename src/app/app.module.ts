import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';
import { CursoComponent } from './components/curso/curso.component';

const appRoutes: Routes = [
	//{ path: 'home', component: HomeComponent },
	{ path: 'curso', component: CursoComponent },
	{
		path: '',
		//redirectTo: '/home',
		component: HomeComponent,
		pathMatch: 'full'
	}
];

@NgModule({
	declarations: [
		AppComponent,
		HomeComponent,
		CursoComponent
	],
	imports: [
		RouterModule.forRoot(appRoutes),
		BrowserModule
	],
	providers: [],
	bootstrap: [AppComponent]
})
export class AppModule { }
