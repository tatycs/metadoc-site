import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';
import { CursosComponent } from './components/cursos/cursos.component';

const appRoutes: Routes = [
	//{ path: 'home', component: HomeComponent },
	{ path: 'cursos', component: CursosComponent },
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
		CursosComponent
	],
	imports: [
		RouterModule.forRoot(appRoutes),
		BrowserModule
	],
	providers: [],
	bootstrap: [AppComponent]
})
export class AppModule { }
