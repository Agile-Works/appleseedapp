﻿/*
Copyright (c) 2003-2009, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.plugins.add('sourcearea',{requires:['editingblock'],init:function(a){var b=CKEDITOR.plugins.sourcearea;a.on('editingBlockReady',function(){var c,d;a.addMode('source',{load:function(e,f){if(CKEDITOR.env.ie&&CKEDITOR.env.version<8)e.setStyle('position','relative');a.textarea=c=new CKEDITOR.dom.element('textarea');c.setAttributes({dir:'ltr',tabIndex:-1});c.addClass('cke_source');var g={width:CKEDITOR.env.ie7Compat?'99%':'100%',height:'100%',resize:'none',outline:'none','text-align':'left'};if(CKEDITOR.env.ie){if(!CKEDITOR.env.ie8Compat){d=function(){c.hide();c.setStyle('height',e.$.clientHeight+'px');c.show();};a.on('resize',d);g.height=e.$.clientHeight+'px';}}else c.on('mousedown',function(i){i=i.data.$;if(i.stopPropagation)i.stopPropagation();});e.setHtml('');e.append(c);c.setStyles(g);a.mayBeDirty=true;this.loadData(f);var h=a.keystrokeHandler;if(h)h.attach(c);setTimeout(function(){a.mode='source';a.fire('mode');},CKEDITOR.env.gecko||CKEDITOR.env.webkit?100:0);},loadData:function(e){c.setValue(e);},getData:function(){return c.getValue();},getSnapshotData:function(){return c.getValue();},unload:function(e){a.textarea=c=null;if(d)a.removeListener('resize',d);if(CKEDITOR.env.ie&&CKEDITOR.env.version<8)e.removeStyle('position');},focus:function(){c.focus();}});});a.addCommand('source',b.commands.source);if(a.ui.addButton)a.ui.addButton('Source',{label:a.lang.source,command:'source'});a.on('mode',function(){a.getCommand('source').setState(a.mode=='source'?CKEDITOR.TRISTATE_ON:CKEDITOR.TRISTATE_OFF);});}});CKEDITOR.plugins.sourcearea={commands:{source:{modes:{wysiwyg:1,source:1},exec:function(a){if(a.mode=='wysiwyg')a.fire('saveSnapshot');a.getCommand('source').setState(CKEDITOR.TRISTATE_DISABLED);a.setMode(a.mode=='source'?'wysiwyg':'source');},canUndo:false}}};
